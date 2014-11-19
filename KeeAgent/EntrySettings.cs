//
//  EntrySettings.cs
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
      public bool SaveAttachmentToTempFile { get; set; }
      public string FileName { get; set; }

      public LocationData()
      {
        SelectedType = null;
        AttachmentName = string.Empty;
        SaveAttachmentToTempFile = false;
        FileName = string.Empty;
      }

      #region ICloneable implementation

      public object Clone()
      {
        var clone = new LocationData();
        clone.SelectedType = SelectedType;
        clone.AttachmentName = AttachmentName;
        clone.SaveAttachmentToTempFile = SaveAttachmentToTempFile;
        clone.FileName = FileName;
        return clone;
      }

      #endregion

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
    public LocationData Location { get; set; }

    public EntrySettings()
    {
      AllowUseOfSshKey = false;
      AddAtDatabaseOpen = true;
      RemoveAtDatabaseClose = true;
      UseConfirmConstraintWhenAdding = false;
      UseLifetimeConstraintWhenAdding = false;
      LifetimeConstraintDuration = 600;
      Location = new LocationData();
    }

    #region ICloneable implementation


    public object Clone()
    {
      var clone = new EntrySettings();
      clone.AllowUseOfSshKey = AllowUseOfSshKey;
      clone.AddAtDatabaseOpen = AddAtDatabaseOpen;
      clone.RemoveAtDatabaseClose = RemoveAtDatabaseClose;
      clone.UseConfirmConstraintWhenAdding = UseConfirmConstraintWhenAdding;
      clone.UseLifetimeConstraintWhenAdding = UseLifetimeConstraintWhenAdding;
      clone.LifetimeConstraintDuration = LifetimeConstraintDuration;
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
        UseConfirmConstraintWhenAdding == other.UseConfirmConstraintWhenAdding &&
        UseLifetimeConstraintWhenAdding == other.UseLifetimeConstraintWhenAdding &&
        LifetimeConstraintDuration == other.LifetimeConstraintDuration &&
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
