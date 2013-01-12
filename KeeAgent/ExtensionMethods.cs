using System;
using System.Collections.Generic;
using System.Text;
using KeePassLib;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Reflection;
using KeePassLib.Security;
using dlech.SshAgentLib;
using System.Security;
using System.Windows.Forms;
using KeeAgent.Properties;
using System.Drawing;
using System.Diagnostics;
using KeePassLib.Collections;

namespace KeeAgent
{
  public static class ExtensionMethods
  {
    private const string cStringId = "KeeAgent.Settings";

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
      var settingsString = aDatabase.CustomData.Get(cStringId);
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
        aDatabase.CustomData.Set(cStringId, writer.ToString());
      }
    }

    public static EntrySettings GetKeeAgentSettings(this PwEntry aEntry)
    {
      var settingsString = aEntry.Strings.ReadSafe(cStringId);
      if (!string.IsNullOrWhiteSpace(settingsString)) {
        using (var reader = XmlReader.Create(new StringReader(settingsString))) {
          if (EntrySettingsSerializer.CanDeserialize(reader)) {
            return EntrySettingsSerializer.Deserialize(reader) as EntrySettings;
          }
        }
      }
      return new EntrySettings();
    }

    public static void SetKeeAgentSettings(this PwEntry aEntry,
      EntrySettings aSettings)
    {
      aEntry.Strings.SetKeeAgentSettings(aSettings);
    }

    public static void SetKeeAgentSettings(this ProtectedStringDictionary aStringDictionary,
      EntrySettings aSettings)
    {
      using (var writer = new StringWriter()) {
        EntrySettingsSerializer.Serialize(writer, aSettings);
        // string is protected just to make UI look cleaner
        aStringDictionary.Set(cStringId, new ProtectedString(true, writer.ToString()));
      }
    }

    public static ISshKey GetSshKey(this PwEntry aPwEntry)
    {
      var settings = aPwEntry.GetKeeAgentSettings();
      if (!settings.AllowUseOfSshKey) {
        return null;
      }
      KeyFormatter.GetPassphraseCallback getPassphraseCallback =
        delegate()
        {
          var securePassphrase = new SecureString();
          var passphrase = Encoding.UTF8.GetChars(aPwEntry.Strings
            .Get(PwDefs.PasswordField).ReadUtf8());
          foreach (var c in passphrase) {
            securePassphrase.AppendChar(c);
          }
          Array.Clear(passphrase, 0, passphrase.Length);
          return securePassphrase;
        };
      switch (settings.Location.SelectedType) {
        case EntrySettings.LocationType.Attachment:
          if (string.IsNullOrWhiteSpace(settings.Location.AttachmentName)) {
            throw new NoAttachmentException();
          }
          var keyData = aPwEntry.Binaries.Get(settings.Location.AttachmentName);
          return keyData.ReadData().ReadSshKey(getPassphraseCallback);
        case EntrySettings.LocationType.File:
          using (var keyFile = File.OpenRead(settings.Location.FileName)) {
            return keyFile.ReadSshKey(getPassphraseCallback);
          }
        default:
          return null;
      }
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
        }
        var imageIndex = tabControl.ImageList.Images.Add(Resources.KeeAgentIcon, Color.Transparent);
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
  }
}
