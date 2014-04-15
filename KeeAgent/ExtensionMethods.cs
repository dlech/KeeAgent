//
//  ExtensionMethods.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2012-2013  David Lechner
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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using dlech.SshAgentLib;
using KeeAgent.Properties;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;

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
#pragma warning disable 618 // ignore use of obsolete type
          mDatabaseSettingsSerializer = new XmlSerializer(typeof(DatabaseSettings));
#pragma warning restore 618
        }
        return mDatabaseSettingsSerializer;
      }
    }

    [Obsolete ("There are currently no database settings.")]
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

    [Obsolete ("There are currently no database settings.")]
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

    public static ISshKey GetSshKey(this PwEntry aPwEntry)
    {
      var settings = aPwEntry.GetKeeAgentSettings();
      if (!settings.AllowUseOfSshKey) {
        return null;
      }
      KeyFormatter.GetPassphraseCallback getPassphraseCallback =
        delegate(string comment)
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
        var imageIndex = tabControl.ImageList.Images.Add(Resources.KeeAgentIcon_png,
                                                         Color.Transparent);
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
