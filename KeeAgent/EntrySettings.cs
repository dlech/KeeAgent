// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2012-2013,2022 David Lechner <david@lechnology.com>

using System;
using System.Xml.Serialization;

namespace KeeAgent
{
  public class EntrySettings : IEquatable<EntrySettings>
  {
    public enum LocationType
    {
      [XmlEnum(Name="attachment")]
      Attachment,
      [XmlEnum(Name = "file")]
      File
    }

    public class LocationData : IEquatable<LocationData>
    {
      public LocationType? SelectedType { get; set; }
      public string AttachmentName { get; set; }
      public bool SaveAttachmentToTempFile { get; set; }
      public string FileName { get; set; }

      public LocationData()
      {
        SelectedType = null;
        AttachmentName = string.Empty;
        SaveAttachmentToTempFile = false;
        FileName = string.Empty;
      }

      public LocationData DeepCopy()
      {
        var copy = new LocationData();
        copy.SelectedType = SelectedType;
        copy.AttachmentName = AttachmentName;
        copy.SaveAttachmentToTempFile = SaveAttachmentToTempFile;
        copy.FileName = FileName;
        return copy;
      }

      #region IEquatable implementation

      public bool Equals (LocationData other)
      {
        return SelectedType == other.SelectedType &&
          AttachmentName == other.AttachmentName &&
          SaveAttachmentToTempFile == other.SaveAttachmentToTempFile &&
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
    public bool UseConfirmConstraintWhenAdding { get; set; }
    public bool UseLifetimeConstraintWhenAdding { get; set; }
    public uint LifetimeConstraintDuration { get; set; }
    public LocationData PublicKeyLocation { get; set; }
    public LocationData PrivateKeyLocation { get; set; }

    public EntrySettings()
    {
      AllowUseOfSshKey = false;
      AddAtDatabaseOpen = true;
      RemoveAtDatabaseClose = true;
      UseConfirmConstraintWhenAdding = false;
      UseLifetimeConstraintWhenAdding = false;
      LifetimeConstraintDuration = 600;
      PublicKeyLocation = new LocationData();
      PrivateKeyLocation = new LocationData();
    }

    public EntrySettings DeepCopy()
    {
      return new EntrySettings {
        AllowUseOfSshKey = AllowUseOfSshKey,
        AddAtDatabaseOpen = AddAtDatabaseOpen,
        RemoveAtDatabaseClose = RemoveAtDatabaseClose,
        UseConfirmConstraintWhenAdding = UseConfirmConstraintWhenAdding,
        UseLifetimeConstraintWhenAdding = UseLifetimeConstraintWhenAdding,
        LifetimeConstraintDuration = LifetimeConstraintDuration,
        PublicKeyLocation = PublicKeyLocation.DeepCopy(),
        PrivateKeyLocation = PrivateKeyLocation.DeepCopy()
      };
    }

    #region IEquatable implementation
    public bool Equals (EntrySettings other)
    {
      return AllowUseOfSshKey == other.AllowUseOfSshKey &&
        AddAtDatabaseOpen == other.AddAtDatabaseOpen &&
        RemoveAtDatabaseClose == other.RemoveAtDatabaseClose &&
        UseConfirmConstraintWhenAdding == other.UseConfirmConstraintWhenAdding &&
        UseLifetimeConstraintWhenAdding == other.UseLifetimeConstraintWhenAdding &&
        LifetimeConstraintDuration == other.LifetimeConstraintDuration &&
        PublicKeyLocation == other.PublicKeyLocation &&
        PrivateKeyLocation == other.PrivateKeyLocation;
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
      return  PublicKeyLocation.GetHashCode() ^ PrivateKeyLocation.GetHashCode () ^ (AllowUseOfSshKey ? 0x0 : 0x1) ^
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
