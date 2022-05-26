// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2012-2013,2022 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace KeeAgent
{
  /// <summary>
  /// Settings for an individual KeePass 2.x password entry.
  /// </summary>
  /// <remarks>
  /// This class is serializable to XML for storage and implements property change notifications
  /// for data binding.
  /// </remarks>
  public sealed class EntrySettings : IEquatable<EntrySettings>, INotifyPropertyChanged
  {
    public enum LocationType
    {
      [XmlEnum(Name = "attachment")]
      Attachment,
      [XmlEnum(Name = "file")]
      File
    }

    public sealed class LocationData : IEquatable<LocationData>
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
        var copy = new LocationData {
          SelectedType = SelectedType,
          AttachmentName = AttachmentName,
          SaveAttachmentToTempFile = SaveAttachmentToTempFile,
          FileName = FileName
        };
        return copy;
      }

      public bool Equals(LocationData other)
      {
        return other != null && SelectedType == other.SelectedType &&
          AttachmentName == other.AttachmentName &&
          SaveAttachmentToTempFile == other.SaveAttachmentToTempFile &&
          FileName == other.FileName;
      }

      public override bool Equals(object obj)
      {
        return Equals(obj as LocationData);
      }

      public override int GetHashCode()
      {
        var hashCode = -220646477;
        hashCode = hashCode * -1521134295 + SelectedType.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AttachmentName);
        hashCode = hashCode * -1521134295 + SaveAttachmentToTempFile.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileName);
        return hashCode;
      }

      public static bool operator ==(LocationData left, LocationData right)
      {
        return EqualityComparer<LocationData>.Default.Equals(left, right);
      }

      public static bool operator !=(LocationData left, LocationData right)
      {
        return !(left == right);
      }
    }

    [XmlType("Constraint")]
    public sealed class DestinationConstraint : INotifyPropertyChanged, IEquatable<DestinationConstraint>
    {
      public sealed class KeySpec : INotifyPropertyChanged, IEquatable<KeySpec>
      {
        private string hostKey;
        private bool isCA;

        public string HostKey {
          get {
            return hostKey ?? string.Empty;
          }
          set {
            hostKey = value;
            NotifyPropertyChanged();
          }
        }

        public bool IsCA {
          get {
            return isCA;
          }
          set {
            isCA = value;
            NotifyPropertyChanged();
          }
        }

        public KeySpec DeepCopy()
        {
          return new KeySpec { HostKey = HostKey, IsCA = IsCA };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
          if (PropertyChanged != null) {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
          }
        }

        public bool Equals(KeySpec other)
        {
          if (other == null) {
            return false;
          }

          return HostKey == other.HostKey && IsCA == other.IsCA;
        }

        public override bool Equals(object obj)
        {
          return Equals(obj as KeySpec);
        }

        public override int GetHashCode()
        {
          var hashCode = -1497596449;
          hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HostKey);
          hashCode = hashCode * -1521134295 + IsCA.GetHashCode();
          return hashCode;
        }

        public static bool operator ==(KeySpec left, KeySpec right)
        {
          return EqualityComparer<KeySpec>.Default.Equals(left, right);
        }

        public static bool operator !=(KeySpec left, KeySpec right)
        {
          return !(left == right);
        }
      }

      private sealed class KeyCountTypeConverter : TypeConverter
      {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
          if (destinationType == typeof(string)) {
            return true;
          }

          return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
          if (destinationType == typeof(string)) {
            return (value as KeySpec[]).Length.ToString();
          }

          return base.ConvertTo(context, culture, value, destinationType);
        }
      }

      private string fromHost;
      private KeySpec[] fromHostKeys;
      private string toUser;
      private string toHost;
      private KeySpec[] toHostKeys;

      public string FromHost {
        get {
          return fromHost;
        }
        set {
          fromHost = value;
          NotifyPropertyChanged();
        }
      }

      // key arrays are represented by the number of keys in grid view
      [TypeConverter(typeof(KeyCountTypeConverter))]
      public KeySpec[] FromHostKeys {
        get {
          return fromHostKeys ?? Array.Empty<KeySpec>();
        }
        set {
          fromHostKeys = value;
          NotifyPropertyChanged();
        }
      }

      public string ToUser {
        get {
          return toUser;
        }
        set {
          toUser = value;
          NotifyPropertyChanged();
        }
      }

      public string ToHost {
        get {
          return toHost;
        }
        set {
          toHost = value;
          NotifyPropertyChanged();
        }
      }

      // key arrays are represented by the number of keys in grid view
      [TypeConverter(typeof(KeyCountTypeConverter))]
      public KeySpec[] ToHostKeys {
        get {
          return toHostKeys ?? Array.Empty<KeySpec>();
        }
        set {
          toHostKeys = value;
          NotifyPropertyChanged();
        }
      }

      public DestinationConstraint DeepCopy()
      {
        return new DestinationConstraint {
          FromHost = FromHost,
          FromHostKeys = FromHostKeys.Select(x => x.DeepCopy()).ToArray(),
          ToUser = ToUser,
          ToHost = ToHost,
          ToHostKeys = ToHostKeys.Select(x => x.DeepCopy()).ToArray(),
        };
      }

      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
      {
        if (PropertyChanged != null) {
          PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
      }

      public bool Equals(DestinationConstraint other)
      {
        return other != null &&
               FromHost == other.FromHost &&
               FromHostKeys.SequenceEqual(other.FromHostKeys) &&
               ToUser == other.ToUser &&
               ToHost == other.ToHost &&
               ToHostKeys.SequenceEqual(other.ToHostKeys);
      }

      public override bool Equals(object obj)
      {
        return Equals(obj as DestinationConstraint);
      }

      public override int GetHashCode()
      {
        var hashCode = 2128332141;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FromHost);
        hashCode = hashCode * -1521134295 + EqualityComparer<KeySpec[]>.Default.GetHashCode(FromHostKeys);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ToUser);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ToHost);
        hashCode = hashCode * -1521134295 + EqualityComparer<KeySpec[]>.Default.GetHashCode(ToHostKeys);
        return hashCode;
      }

      public static bool operator ==(DestinationConstraint left, DestinationConstraint right)
      {
        return EqualityComparer<DestinationConstraint>.Default.Equals(left, right);
      }

      public static bool operator !=(DestinationConstraint left, DestinationConstraint right)
      {
        return !(left == right);
      }
    }

    private bool allowUseOfSshKey;
    private bool addAtDatabaseOpen;
    private bool removeAtDatabaseClose;
    private bool useConfirmConstraintWhenAdding;
    private bool useLifetimeConstraintWhenAdding;
    private uint lifetimeConstraintDuration;
    private bool useDestinationConstraintWhenAdding;
    private DestinationConstraint[] destinationConstraints;
    private LocationData location;

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
      if (PropertyChanged != null) {
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public bool AllowUseOfSshKey {
      get {
        return allowUseOfSshKey;
      }
      set {
        allowUseOfSshKey = value;
        NotifyPropertyChanged();
      }
    }

    public bool AddAtDatabaseOpen {
      get {
        return addAtDatabaseOpen;
      }
      set {
        addAtDatabaseOpen = value;
        NotifyPropertyChanged();
      }
    }

    public bool RemoveAtDatabaseClose {
      get {
        return removeAtDatabaseClose;
      }
      set {
        removeAtDatabaseClose = value;
        NotifyPropertyChanged();
      }
    }

    public bool UseConfirmConstraintWhenAdding {
      get {
        return useConfirmConstraintWhenAdding;
      }
      set {
        useConfirmConstraintWhenAdding = value;
        NotifyPropertyChanged();
      }
    }

    public bool UseLifetimeConstraintWhenAdding {
      get {
        return useLifetimeConstraintWhenAdding;
      }
      set {
        useLifetimeConstraintWhenAdding = value;
        NotifyPropertyChanged();
      }
    }

    public uint LifetimeConstraintDuration {
      get {
        return lifetimeConstraintDuration;
      }
      set {
        lifetimeConstraintDuration = value;
        NotifyPropertyChanged();
      }
    }

    public bool UseDestinationConstraintWhenAdding {
      get {
        return useDestinationConstraintWhenAdding;
      }
      set {
        useDestinationConstraintWhenAdding = value;
        NotifyPropertyChanged();
      }
    }

    public DestinationConstraint[] DestinationConstraints {
      get {
        return destinationConstraints ?? Array.Empty<DestinationConstraint>();
      }
      set {
        destinationConstraints = value;
        NotifyPropertyChanged();
      }
    }

    public LocationData Location {
      get {
        return location;
      }
      set {
        location = value;
        NotifyPropertyChanged();
      }
    }

    public EntrySettings()
    {
      AllowUseOfSshKey = false;
      AddAtDatabaseOpen = true;
      RemoveAtDatabaseClose = true;
      UseConfirmConstraintWhenAdding = false;
      UseLifetimeConstraintWhenAdding = false;
      LifetimeConstraintDuration = 600;
      UseDestinationConstraintWhenAdding = false;
      DestinationConstraints = Array.Empty<DestinationConstraint>();
      Location = new LocationData();
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
        UseDestinationConstraintWhenAdding = UseDestinationConstraintWhenAdding,
        DestinationConstraints = DestinationConstraints.Select(x => x.DeepCopy()).ToArray(),
        Location = Location.DeepCopy(),
      };
    }

    public bool Equals(EntrySettings other)
    {
      return other != null &&
        AllowUseOfSshKey == other.AllowUseOfSshKey &&
        AddAtDatabaseOpen == other.AddAtDatabaseOpen &&
        RemoveAtDatabaseClose == other.RemoveAtDatabaseClose &&
        UseConfirmConstraintWhenAdding == other.UseConfirmConstraintWhenAdding &&
        UseLifetimeConstraintWhenAdding == other.UseLifetimeConstraintWhenAdding &&
        LifetimeConstraintDuration == other.LifetimeConstraintDuration &&
        UseDestinationConstraintWhenAdding == other.UseDestinationConstraintWhenAdding &&
        (DestinationConstraints ?? Array.Empty<DestinationConstraint>()).SequenceEqual(other.DestinationConstraints ?? Array.Empty<DestinationConstraint>()) &&
        Location == other.Location;
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as EntrySettings);
    }

    public override int GetHashCode()
    {
      var hashCode = -1863443851;
      hashCode = hashCode * -1521134295 + AllowUseOfSshKey.GetHashCode();
      hashCode = hashCode * -1521134295 + AddAtDatabaseOpen.GetHashCode();
      hashCode = hashCode * -1521134295 + RemoveAtDatabaseClose.GetHashCode();
      hashCode = hashCode * -1521134295 + UseConfirmConstraintWhenAdding.GetHashCode();
      hashCode = hashCode * -1521134295 + UseLifetimeConstraintWhenAdding.GetHashCode();
      hashCode = hashCode * -1521134295 + LifetimeConstraintDuration.GetHashCode();
      hashCode = hashCode * -1521134295 + UseDestinationConstraintWhenAdding.GetHashCode();
      hashCode = hashCode * -1521134295 + EqualityComparer<DestinationConstraint[]>.Default.GetHashCode(DestinationConstraints);
      hashCode = hashCode * -1521134295 + EqualityComparer<LocationData>.Default.GetHashCode(Location);
      return hashCode;
    }

    public static bool operator ==(EntrySettings left, EntrySettings right)
    {
      return EqualityComparer<EntrySettings>.Default.Equals(left, right);
    }

    public static bool operator !=(EntrySettings left, EntrySettings right)
    {
      return !(left == right);
    }
  }
}
