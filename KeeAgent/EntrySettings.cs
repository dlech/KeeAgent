using System;
using System.Xml.Serialization;

namespace KeeAgent
{
  public class EntrySettings : ICloneable, IEquatable<EntrySettings>
  {
    public enum LocationType
    {
      [XmlEnum(Name="attachment")]
      Attachment,
      [XmlEnum(Name = "file")]
      File
    }

    public class LocationData : ICloneable, IEquatable<LocationData>
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

      #region ICloneable implementation

      public object Clone()
      {
        var clone = new LocationData();
        clone.SelectedType = SelectedType;
        clone.AttachmentName = AttachmentName;
        clone.FileName = FileName;
        return clone;
      }

      #endregion

      #region IEquatable implementation

      public bool Equals (LocationData other)
      {
        return SelectedType == other.SelectedType &&
          AttachmentName == other.AttachmentName &&
          FileName == other.FileName;
      }

      #endregion

      public override bool Equals (object obj)
      {
        if (obj is LocationData) {
          return this == (LocationData)obj;
        }
        return base.Equals (obj);
      }

      public override int GetHashCode ()
      {
        return AttachmentName.GetHashCode () ^ FileName.GetHashCode () ^
          (SelectedType == null ? -1 : (int)SelectedType);
      }

      public static bool operator ==(LocationData data1, LocationData data2) {
        if (ReferenceEquals (data1, data2)) {
          return true;
        }
        if ((object)data1 == null || (object)data2 == null) {
          return false;
        }
        return data1.Equals(data2);
      }

      public static bool operator !=(LocationData data1, LocationData data2) {
        return !(data1 == data2);
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

    #region ICloneable implementation


    public object Clone()
    {
      var clone = new EntrySettings();
      clone.AllowUseOfSshKey = AllowUseOfSshKey;
      clone.AddAtDatabaseOpen = AddAtDatabaseOpen;
      clone.RemoveAtDatabaseClose = RemoveAtDatabaseClose;
      clone.Location = (LocationData)Location.Clone();
      return clone;
    }

    #endregion

    #region IEquatable implementation
    public bool Equals (EntrySettings other)
    {
      return AllowUseOfSshKey == other.AllowUseOfSshKey &&
        AddAtDatabaseOpen == other.AddAtDatabaseOpen &&
        RemoveAtDatabaseClose == other.RemoveAtDatabaseClose &&
        Location == other.Location;
    }
    #endregion

    public override bool Equals (object obj)
    {
      if (obj is EntrySettings) {
        return this == (EntrySettings)obj;
      }
      return base.Equals (obj);
    }

    public override int GetHashCode ()
    {
      return Location.GetHashCode () ^ (AllowUseOfSshKey ? 0x0 : 0x1) ^
        (AddAtDatabaseOpen ? 0x0 : 0x2) ^ (RemoveAtDatabaseClose ? 0x0 : 0x4);
    }

    public static bool operator ==(EntrySettings settings1, EntrySettings settings2) {
      if (ReferenceEquals (settings1, settings2)) {
        return true;
      }
      if ((object)settings1 == null || (object)settings2 == null) {
        return false;
      }
      return settings1.Equals(settings2);
    }

    public static bool operator !=(EntrySettings settings1, EntrySettings settings2) {
      return !(settings1 == settings2);
    }
  }
}
