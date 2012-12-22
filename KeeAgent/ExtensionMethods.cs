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
        aPwEntry.Strings.Set(cStringId, new ProtectedString(false, writer.ToString()));
      }
    }
  }
}
