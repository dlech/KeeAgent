using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace KeeAgent
{
  public class EntrySettings
  {
    public enum LocationType
    {
      [XmlEnum(Name="attachment")]
      Attachment,
      [XmlEnum(Name = "file")]
      File
    }

    public class LocationData
    {
      public LocationType? SelectedType { get; set; }
      public string AttachmentName { get; set; }
      public string FileName { get; set; }

      public LocationData()
      {
        SelectedType = null;
        AttachmentName = string.Empty;
        FileName = string.Empty;
      }
    }

    public bool AllowUseOfSshKey { get; set; }
    public bool AddAtDatabaseOpen { get; set; }
    public bool RemoveAtDatabaseClose { get; set; }
    public LocationData Location { get; set; }

    public EntrySettings()
    {
      AllowUseOfSshKey = false;
      AddAtDatabaseOpen = true;
      RemoveAtDatabaseClose = true;
      Location = new LocationData();
    }
  }
}
