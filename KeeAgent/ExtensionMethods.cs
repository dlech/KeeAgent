//
//  ExtensionMethods.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2012-2016 David Lechner
//
//  This program is free software; you can redistribute it and/or
//  modify it under the terms of the GNU General Public License
//  as published by the Free Software Foundation; either version 2
//  of the License, or (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, see <http://www.gnu.org/licenses>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using dlech.SshAgentLib;
using KeeAgent.Properties;
using KeePass.Util.Spr;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using System.Runtime.InteropServices;
using KeePass.UI;

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

    private static XmlSerializer mEntrySettingsSerializer;
    private static XmlSerializer mDatabaseSettingsSerializer;

    private static XmlSerializer EntrySettingsSerializer
    {
      get
      {
        if (mEntrySettingsSerializer == null) {
          mEntrySettingsSerializer = new XmlSerializer(typeof(EntrySettings));
        }
        return mEntrySettingsSerializer;
      }
    }

    private static XmlSerializer DatabaseSettingsSerializer
    {
      get
      {
        if (mDatabaseSettingsSerializer == null) {
          mDatabaseSettingsSerializer = new XmlSerializer(typeof(DatabaseSettings));
        }
        return mDatabaseSettingsSerializer;
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
        settings.AllowUseOfSshKey)
      {
        using (var writer = new StringWriter()) {
          EntrySettingsSerializer.Serialize(writer, settings);
          // string is protected just to make UI look cleaner
          binaries.Set(settingsBinaryId,
            new ProtectedBinary(false, Encoding.Unicode.GetBytes(writer.ToString())));
        }
      }
    }

    public static ISshKey GetSshKey(this PwEntry entry)
    {
      var settings = entry.GetKeeAgentSettings();
      var context = new SprContext(entry, entry.GetDatabase(), SprCompileFlags.Deref);
      return settings.GetSshKey(entry.Strings, entry.Binaries, context);
    }

    public static ISshKey GetSshKey(this EntrySettings settings,
      ProtectedStringDictionary strings, ProtectedBinaryDictionary binaries,
      SprContext sprContext)
    {      
      if (!settings.AllowUseOfSshKey) {
        return null;
      }
      KeyFormatter.GetPassphraseCallback getPassphraseCallback =
        delegate(string comment)
        {
          var securePassphrase = new SecureString();
            var passphrase = SprEngine.Compile(strings.ReadSafe(
                          PwDefs.PasswordField), sprContext);
          foreach (var c in passphrase) {
            securePassphrase.AppendChar(c);
          }
          return securePassphrase;
        };
      Func<Stream> getPrivateKeyStream;
      Func<Stream> getPublicKeyStream = null;
      switch (settings.Location.SelectedType) {
        case EntrySettings.LocationType.Attachment:
          if (string.IsNullOrWhiteSpace(settings.Location.AttachmentName)) {
            throw new NoAttachmentException();
          }
          var privateKeyData = binaries.Get(settings.Location.AttachmentName);
          var publicKeyData = binaries.Get(settings.Location.AttachmentName + ".pub");
          getPrivateKeyStream = () => new MemoryStream(privateKeyData.ReadData());
          if (publicKeyData != null)
            getPublicKeyStream = () => new MemoryStream(publicKeyData.ReadData());
          return GetSshKey(getPrivateKeyStream, getPublicKeyStream,
                           settings.Location.AttachmentName, getPassphraseCallback);
        case EntrySettings.LocationType.File:
          var filename = settings.Location.FileName.ExpandEnvironmentVariables();
          getPrivateKeyStream = () => File.OpenRead(filename);
          var publicKeyFile = filename + ".pub";
          if (File.Exists(publicKeyFile))
            getPublicKeyStream = () => File.OpenRead(publicKeyFile);
          return GetSshKey(getPrivateKeyStream, getPublicKeyStream,
                           settings.Location.AttachmentName, getPassphraseCallback);
        default:
          return null;
      }
    }

    static ISshKey GetSshKey(Func<Stream> getPrivateKeyStream,
                             Func<Stream> getPublicKeyStream,
                             string fallbackComment,
                             KeyFormatter.GetPassphraseCallback getPassphrase)
    {
      ISshKey key;
      using (var privateKeyStream = getPrivateKeyStream())
        key = privateKeyStream.ReadSshKey(getPassphrase);
      if (string.IsNullOrWhiteSpace(key.Comment) && getPublicKeyStream != null) {
        using (var stream = getPublicKeyStream())
          key.Comment = KeyFormatter.GetComment(stream.ReadAllLines(Encoding.UTF8));
      }
      if (string.IsNullOrWhiteSpace(key.Comment)) {
        key.Comment = fallbackComment;
      }
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

      } catch (Exception ex) {
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
      var builder = new StringBuilder(entry.Strings.Get(PwDefs.TitleField).ReadString ());
      var parent = entry.ParentGroup;
      while (parent != null) {
        builder.Insert(0, Path.DirectorySeparatorChar);
        builder.Insert(0, parent.Name);
        parent = parent.ParentGroup;
      }
      return builder.ToString ();
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
        if (filename.StartsWith("~/", StringComparison.Ordinal))
        {
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
      } else {
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
