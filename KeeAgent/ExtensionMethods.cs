using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeePassLib;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Reflection;
using KeePassLib.Security;
using dlech.SshAgentLib;
using System.Security;

namespace KeeAgent
{
  public static class ExtensionMethods
  {
    private const string cStringId = "KeeAgent.Settings";

    private static XmlSerializer mSerializer;

    private static XmlSerializer Serializer
    {
      get
      {
        if (mSerializer == null) {
          mSerializer = new XmlSerializer(typeof(EntrySettings));
        }
        return mSerializer;
      }
    }

    public static EntrySettings GetKeeAgentEntrySettings(this PwEntry aPwEntry)
    {
      var settingsString = aPwEntry.Strings.ReadSafe(cStringId);
      if (settingsString == string.Empty) {
        return new EntrySettings();
      }
      using (var reader = XmlReader.Create(new StringReader(settingsString))) {
        if (Serializer.CanDeserialize(reader)) {
          return Serializer.Deserialize(reader) as EntrySettings;
        }
      }
      return new EntrySettings();
    }

    public static void SetKeeAgentEntrySettings(this PwEntry aPwEntry,
      EntrySettings aSettings)
    {
      using (var writer = new StringWriter()) {
        Serializer.Serialize(writer, aSettings);
        // string is protected just to make UI look cleaner
        aPwEntry.Strings.Set(cStringId, new ProtectedString(true, writer.ToString()));
      }
    }

    public static ISshKey GetSshKey(this PwEntry aPwEntry)
    {
      var settings = aPwEntry.GetKeeAgentEntrySettings();
      if (!settings.HasSshKey) {
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
  }
}
