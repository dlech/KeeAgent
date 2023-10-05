// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2012-2016,2022 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using dlech.SshAgentLib;
using KeeAgent.Properties;
using KeeAgent.UI;
using KeePass.Util.Spr;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using KeePass.UI;
using SshAgentLib.Keys;
using Org.BouncyCastle.Crypto;
using SshAgentLib.Extension;

namespace KeeAgent
{
  public static class ExtensionMethods
  {
    /// <summary>
    /// name of string entry that contains KeeAgent settings (obsolete)
    /// </summary>
    const string settingsStringId = "KeeAgent.Settings";
    /// <summary>
    /// Name of binary attachment that contains keeagent settings
    /// </summary>
    const string settingsBinaryId = "KeeAgent.settings";

    private static XmlSerializer entrySettingsSerializer;
    private static XmlSerializer databaseSettingsSerializer;

    public static DestinationConstraint ToDestinationConstraint(this EntrySettings.DestinationConstraint[] constraints)
    {
      return new DestinationConstraint(constraints.Select(x => new DestinationConstraint.Constraint(
        new DestinationConstraint.Hop(null, x.FromHost, x.FromHostKeys.Select(
          f => new DestinationConstraint.KeySpec(SshPublicKey.Parse(f.HostKey), f.IsCA)).ToList()),
        new DestinationConstraint.Hop(x.ToUser, x.ToHost, x.ToHostKeys.Select(
          t => new DestinationConstraint.KeySpec(SshPublicKey.Parse(t.HostKey), t.IsCA)).ToList())))
        .ToList());
    }

    private static XmlSerializer EntrySettingsSerializer {
      get {
        if (entrySettingsSerializer == null) {
          entrySettingsSerializer = new XmlSerializer(typeof(EntrySettings));
        }

        return entrySettingsSerializer;
      }
    }

    private static XmlSerializer DatabaseSettingsSerializer {
      get {
        if (databaseSettingsSerializer == null) {
          databaseSettingsSerializer = new XmlSerializer(typeof(DatabaseSettings));
        }

        return databaseSettingsSerializer;
      }
    }

    public static DatabaseSettings GetKeeAgentSettings(this PwDatabase aDatabase)
    {
      var settingsString = aDatabase.CustomData.Get(settingsStringId);

      if (!string.IsNullOrWhiteSpace(settingsString)) {
        using (var reader = XmlReader.Create(new StringReader(settingsString))) {
          if (DatabaseSettingsSerializer.CanDeserialize(reader)) {
            return DatabaseSettingsSerializer.Deserialize(reader) as DatabaseSettings;
          }
        }
      }

      return new DatabaseSettings();
    }

    public static void SetKeeAgentSettings(this PwDatabase aDatabase,
      DatabaseSettings aSettings)
    {
      using (var writer = new StringWriter()) {
        DatabaseSettingsSerializer.Serialize(writer, aSettings);
        aDatabase.CustomData.Set(settingsStringId, writer.ToString());
      }
    }

    public static EntrySettings GetKeeAgentSettings(this PwEntry entry)
    {
      var settingsString = entry.Strings.ReadSafe(settingsStringId);

      // move settings from string to binary attachment
      if (settingsString != string.Empty) {
        entry.Binaries.Set(settingsBinaryId,
          new ProtectedBinary(false, Encoding.Unicode.GetBytes(settingsString)));
        entry.Strings.Remove(settingsStringId);
        entry.Touch(true);
      }

      var settingsBinary = entry.Binaries.Get(settingsBinaryId);

      if (settingsBinary != null) {
        settingsString = Encoding.Unicode.GetString(settingsBinary.ReadData());

        if (!string.IsNullOrWhiteSpace(settingsString)) {
          using (var reader = XmlReader.Create(new StringReader(settingsString))) {
            if (EntrySettingsSerializer.CanDeserialize(reader)) {
              return EntrySettingsSerializer.Deserialize(reader) as EntrySettings;
            }
          }
        }
      }

      return new EntrySettings();
    }

    public static void SetKeeAgentSettings(this PwEntry entry,
      EntrySettings settings)
    {
      entry.Binaries.SetKeeAgentSettings(settings);

      // remove old settings string
      if (entry.Strings.GetKeys().Contains(settingsStringId)) {
        entry.Strings.Remove(settingsStringId);
      }
    }

    public static void SetKeeAgentSettings(this ProtectedBinaryDictionary binaries,
      EntrySettings settings)
    {
      // only save if there is an existing entry or AllowUseOfSshKey is checked
      // this way we don't pollute entries that don't have SSH keys
      if (binaries.Get(settingsBinaryId) != null ||
        settings.AllowUseOfSshKey) {
        using (var writer = new StringWriter()) {
          EntrySettingsSerializer.Serialize(writer, settings);
          // string is protected just to make UI look cleaner
          binaries.Set(settingsBinaryId,
            new ProtectedBinary(false, Encoding.Unicode.GetBytes(writer.ToString())));
        }
      }
    }

    /// <summary>
    /// Gets the public key from a database entry.
    ///
    /// In order of preference, this will use <c>&lt;private&gt;-cert.pub</c>,
    /// <c>&lt;private&gt;.pub</c>, or the public key included in the private
    /// key.
    /// </summary>
    /// <param name="entry">The KeePass database entry.</param>
    /// <returns>The SSH key or <c>null</c> if none found.</returns>
    /// <remarks>
    /// Do not call this on entries that are currently being edited. It will
    /// return stale data. Use <see cref="TryGetSshPublicKey(EntrySettings, ProtectedBinaryDictionary)"/>
    /// instead.
    /// </remarks>
    public static SshPublicKey TryGetSshPublicKey(this PwEntry entry)
    {
      var settings = entry.GetKeeAgentSettings();
      return settings.TryGetSshPublicKey(entry.Binaries);
    }

    /// <summary>
    /// Gets the public key from a database entry.
    ///
    /// In order of preference, this will use <c>&lt;private&gt;-cert.pub</c>,
    /// <c>&lt;private&gt;.pub</c>, or the public key included in the private
    /// key.
    /// </summary>
    /// <param name="settings">The KeePass database entry settings.</param>
    /// <param name="binaries">The KeePass database entry binaries.</param>
    /// <returns>The SSH key or <c>null</c> if none found.</returns>
    public static SshPublicKey TryGetSshPublicKey(
      this EntrySettings settings,
      ProtectedBinaryDictionary binaries)
    {
      if (!settings.AllowUseOfSshKey) {
        throw new InvalidOperationException("SSH keys not enabled for this entry");
      }

      switch (settings.Location.SelectedType) {
        case EntrySettings.LocationType.Attachment: {
            var certificateAttachment = binaries.Get(settings.Location.AttachmentName + "-cert.pub");

            if (certificateAttachment != null) {
              return SshPublicKey.Read(new MemoryStream(certificateAttachment.ReadData()));
            }

            var publicKeyAttachment = binaries.Get(settings.Location.AttachmentName + ".pub");

            if (publicKeyAttachment != null) {
              return SshPublicKey.Read(new MemoryStream(publicKeyAttachment.ReadData()));
            }

            if (string.IsNullOrWhiteSpace(settings.Location.AttachmentName)) {
              throw new NoAttachmentException();
            }

            var privateKeyAttachment = binaries.Get(settings.Location.AttachmentName);

            if (privateKeyAttachment == null) {
              throw new NoAttachmentException();
            };

            var privateKey = SshPrivateKey.Read(new MemoryStream(privateKeyAttachment.ReadData()));

            return privateKey.PublicKey;
          }
        case EntrySettings.LocationType.File: {
            var fileName = settings.Location.FileName.ExpandEnvironmentVariables();

            var certificateFileName = fileName + "-cert.pub";

            try {
              return SshPublicKey.Read(File.OpenRead(certificateFileName));
            }
            catch (FileNotFoundException) {
              // the cert key file is optional
            }

            var publicKeyFileName = fileName + ".pub";

            try {
              return SshPublicKey.Read(File.OpenRead(publicKeyFileName));
            }
            catch (FileNotFoundException) {
              // this is optional in most cases
            }

            var privateKey = SshPrivateKey.Read(File.OpenRead(fileName));

            return privateKey.PublicKey;
          }

        default:
          throw new ArgumentException("settings has invalid private key location", "settings");
      }
    }

    /// <summary>
    /// Gets the (possibly encrypted) SSH private key from a database entry.
    /// </summary>
    /// <param name="entry">The KeePass database entry.</param>
    /// <returns>The SSH key.</returns>
    /// <remarks>
    /// Do not call this on entries that are currently being edited. It will
    /// return stale data. Use <see cref="GetSshPrivateKey(EntrySettings, ProtectedBinaryDictionary)"/>
    /// instead.
    /// </remarks>
    public static SshPrivateKey GetSshPrivateKey(this PwEntry entry)
    {
      var settings = entry.GetKeeAgentSettings();
      return settings.GetSshPrivateKey(entry.Binaries);
    }

    /// <summary>
    /// Gets the (possibly encrypted) SSH private key from a database entry.
    /// </summary>
    /// <param name="settings">The KeePass database entry settings.</param>
    /// <param name="binaries">The KeePass database entry binaries.</param>
    /// <returns>The SSH key.</returns>
    public static SshPrivateKey GetSshPrivateKey(
      this EntrySettings settings,
      ProtectedBinaryDictionary binaries)
    {
      if (!settings.AllowUseOfSshKey) {
        throw new InvalidOperationException("SSH keys not enabled for this entry");
      }

      switch (settings.Location.SelectedType) {
        case EntrySettings.LocationType.Attachment: {
            if (string.IsNullOrWhiteSpace(settings.Location.AttachmentName)) {
              throw new NoAttachmentException();
            }

            var attachment = binaries.Get(settings.Location.AttachmentName);

            if (attachment == null) {
              throw new NoAttachmentException();
            };

            var privateKey = SshPrivateKey.Read(new MemoryStream(attachment.ReadData()));

            return privateKey;
          }
        case EntrySettings.LocationType.File: {
            var fileName = settings.Location.FileName.ExpandEnvironmentVariables();

            var privateKey = SshPrivateKey.Read(File.OpenRead(fileName));

            return privateKey;
          }

        default:
          throw new ArgumentException("settings has invalid private key location", "settings");
      }
    }

    public static void TryAddPubFileForLegacyFileFormat(this PwEntry entry)
    {
      if (entry == null) {
        throw new ArgumentNullException("entry");
      }

      try {
        if (entry.TryGetSshPublicKey() == null) {
          try {
            var settings = entry.GetKeeAgentSettings();

            if (settings.Location.SelectedType == EntrySettings.LocationType.Attachment) {
              var privateKey = new MemoryStream(
                entry.Binaries.Get(settings.Location.AttachmentName).ReadData());
              var publicKey = new MemoryStream();

              SshPrivateKey.CreatePublicKeyFromPrivateKey(
                privateKey, publicKey, () => entry.GetPassphrase(), settings.Location.AttachmentName);

              var name = settings.Location.AttachmentName + ".pub";
              entry.Binaries.Set(name, new ProtectedBinary(false, publicKey.ToArray()));
            }
            else if (settings.Location.SelectedType == EntrySettings.LocationType.File) {
              var fileName = settings.Location.FileName.ExpandEnvironmentVariables();

              if (File.Exists(fileName + ".pub")) {
                // file was created since we called TryGetSshPublicKey()?
                throw new InvalidOperationException();
              }

              SshPrivateKey.CreatePublicKeyFromPrivateKey(
                File.OpenRead(fileName), File.OpenWrite(fileName + ".pub"),
                () => entry.GetPassphrase(), settings.Location.AttachmentName);
            }
          }
          catch (Exception ex) {
            Debug.Fail(ex.ToString());
            // ignoring all errors since this the same error will be
            // thrown and handled later
          }
        }
      }
      catch (Exception ex) {
        Debug.Fail(ex.ToString());
        // all other errors ignored
      }
    }

    static byte[] GetPassphrase(this PwEntry entry)
    {
      var context = new SprContext(entry, entry.GetDatabase(), SprCompileFlags.Deref);
      var passphrase = SprEngine.Compile(entry.Strings.ReadSafe(PwDefs.PasswordField), context);
      return Encoding.UTF8.GetBytes(passphrase);
    }

    public static ISshKey GetSshKey(this PwEntry entry, bool disableKeyDecryptionProgressBar)
    {
      var settings = entry.GetKeeAgentSettings();

      if (!settings.AllowUseOfSshKey) {
        return null;
      }

      var publicKey = settings.TryGetSshPublicKey(entry.Binaries);

      if (publicKey == null) {
        throw new PublicKeyRequiredException();
      }

      var privateKey = settings.GetSshPrivateKey(entry.Binaries);

      AsymmetricKeyParameter parameter;
      var comment = string.Empty;

      if (privateKey.HasKdf && !disableKeyDecryptionProgressBar) {
        // if there is a key derivation function, decrypting could be slow,
        // so show a progress dialog
        var dialog = new DecryptProgressDialog();
        dialog.Start((p) => privateKey.Decrypt(() => entry.GetPassphrase(), p, out comment));
        dialog.ShowDialog();

        if (dialog.DialogResult == DialogResult.Abort) {
          throw dialog.Error;
        }

        parameter = (AsymmetricKeyParameter)dialog.Result;
      }
      else {
        parameter = privateKey.Decrypt(() => entry.GetPassphrase(), null, out comment);
      }

      // prefer public key comment if there is one, otherwise fall back to
      // private key comment
      if (!string.IsNullOrWhiteSpace(publicKey.Comment)) {
        comment = publicKey.Comment;
      }

      var key = new SshKey(
        publicKey.Parameter,
        parameter,
        comment,
        publicKey.Nonce,
        publicKey.Certificate);

      return key;
    }

    public static void AddTab(this Form aForm, UserControl aPanel)
    {
      try {
        var foundControls = aForm.Controls.Find("m_tabMain", true);
        if (foundControls.Length != 1) {
          return;
        }
        var tabControl = foundControls[0] as TabControl;
        if (tabControl == null) {
          return;
        }
        if (tabControl.ImageList == null) {
          tabControl.ImageList = new ImageList();
          tabControl.ImageList.ImageSize = new Size(DpiUtil.ScaleIntX(16), DpiUtil.ScaleIntY(16));
        }
        var imageIndex = tabControl.ImageList.Images.Add(Resources.KeeAgentIcon_png, Color.Transparent);
        var newTab = new TabPage(Translatable.KeeAgent);
        newTab.ImageIndex = imageIndex;
        //newTab.ImageKey = cTabImageKey;
        newTab.UseVisualStyleBackColor = true;
        newTab.Controls.Add(aPanel);
        aPanel.Dock = DockStyle.Fill;
        tabControl.Controls.Add(newTab);

      }
      catch (Exception ex) {
        // Can't have exception here or KeePass freezes.
        Debug.Fail(ex.ToString());
      }
    }

    public static IEnumerable<string> ReadAllLines(this Stream stream, Encoding encoding)
    {
      using (var reader = new StreamReader(stream, encoding)) {
        while (!reader.EndOfStream) {
          yield return reader.ReadLine();
        }
      }
    }

    public static string GetFullPath(this PwEntry entry)
    {
      var builder = new StringBuilder(entry.Strings.Get(PwDefs.TitleField).ReadString());
      var parent = entry.ParentGroup;

      while (parent != null) {
        builder.Insert(0, Path.DirectorySeparatorChar);
        builder.Insert(0, parent.Name);
        parent = parent.ParentGroup;
      }

      return builder.ToString();
    }

    public static PwDatabase GetDatabase(this PwEntry entry)
    {
      var rootGroup = entry.ParentGroup;
      while (rootGroup.ParentGroup != null) {
        rootGroup = rootGroup.ParentGroup;
      }
      var db = KeePass.Program.MainForm.DocumentManager.GetOpenDatabases()
        .SingleOrDefault(d => d.RootGroup == rootGroup);
      return db;
    }

    /// <summary>
    /// Replace a control with a label that indicates that the global confirm
    /// constraint option is enabled.
    /// </summary>
    /// <param name="control"></param>
    public static void ReplaceWithGlobalConfirmMessage(this Control control)
    {
      control.Hide();
      control.Parent.Controls.Add(new Label() {
        Text = Translatable.GlobalConfirmEnabled,
        ForeColor = SystemColors.GrayText,
        Location = control.Location,
        Size = control.Size,
      });
    }

    /// <summary>
    /// Expand environment variables in a filename. Also expandes ~/ to %HOME% environment variable.
    /// </summary>
    /// <param name="path"></param>
    public static String ExpandEnvironmentVariables(this String filename)
    {
      if (filename.StartsWith("~/", StringComparison.Ordinal)) {
        filename = Path.Combine("%HOME%", filename.Substring(2));
      }

      return Environment.ExpandEnvironmentVariables(filename);
    }

    /// <summary>
    /// Set window z order to the bottom.
    /// </summary>
    /// <param name="form"></param>
    public static void SetWindowPosBottom(this Form form)
    {
      if (Type.GetType("Mono.Runtime") == null) {
        SetWindowPos(form.Handle, (IntPtr)HWND.BOTTOM, 0, 0, 0, 0,
          SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOACTIVATE);
      }
      else {
        // TODO: handle mono
      }
    }

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, SetWindowPosFlags flags);

    enum HWND
    {
      BOTTOM = 1,
      NOTOPMOST = -2,
      TOP = 0,
      TOPMOST = -1,
    }

    [Flags]
    enum SetWindowPosFlags
    {
      SWP_NOSIZE = 0x0001,
      SWP_NOMOVE = 0x0002,
      SWP_NOZORDER = 0x0004,
      SWP_NOREDRAW = 0x0008,
      SWP_NOACTIVATE = 0x0010,
      SWP_FRAMECHANGED = 0x0020,
      SWP_SHOWWINDOW = 0x0040,
      SWP_HIDEWINDOW = 0x0080,
      SWP_NOCOPYBITS = 0x0100,
      SWP_NOOWNERZORDER = 0x0200,
      SWP_NOSENDCHANGING = 0x0400,
      SWP_DRAWFRAME = 0x0020,
      SWP_NOREPOSITION = 0x0200,
      SWP_DEFERERASE = 0x2000,
      SWP_ASYNCWINDOWPOS = 0x4000
    }
  }
}
