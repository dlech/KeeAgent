// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2022 David Lechner <david@lechnology.com>

using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KeePassLib.Security;

namespace KeeAgent.UI
{
  /// <summary>
  /// Dialog box for managing SSH key file locations.
  /// </summary>
  public partial class ManageKeyFilesDialog : Form
  {
    /// <summary>
    /// Creates a new key file dialog.
    /// </summary>
    public ManageKeyFilesDialog()
    {
      InitializeComponent();

      if (Type.GetType("Mono.Runtime") == null) {
        Icon = Properties.Resources.KeeAgent_icon;
      }
      else {
        Icon = Properties.Resources.KeeAgent_icon_mono;
      }

      // Each key panel needs it's own binding context, otherwise
      // the two combo boxes will be magically linked because they
      // share the same data source.
      publicKeyLocationPanel.BindingContext = new BindingContext();
      privateKeyLocationPanel.BindingContext = new BindingContext();

      var getAttachment = new Func<string, ProtectedBinary>(name => {
        if (Attachments == null) {
          return null;
        }

        return Attachments.FirstOrDefault(a => a.Key == name).Value;
      });

      publicKeyLocationPanel.KeyLocationChanged += (s, e) => {
        publicKeyLocationPanel.ErrorMessage = UI.Validate.Location(
          publicKeyLocationPanel.KeyLocation, getAttachment, isPrivate: false);
      };

      privateKeyLocationPanel.KeyLocationChanged += (s, e) => {
        privateKeyLocationPanel.ErrorMessage = UI.Validate.Location(
          privateKeyLocationPanel.KeyLocation, getAttachment, isPrivate: true);
      };
    }

    /// <summary>
    /// Gets and sets the attachments data source.
    /// </summary>
    public AttachmentBindingList Attachments {
      get {
        return publicKeyLocationPanel.Attachments;
      }
      set {
        publicKeyLocationPanel.Attachments = value;
        privateKeyLocationPanel.Attachments = value;
      }
    }

    /// <summary>
    /// Gets and sets the public key location settings data source.
    /// </summary>
    public EntrySettings.LocationData PublicKeyLocation {
      get {
        return publicKeyLocationPanel.KeyLocation;
      }
      set {
        publicKeyLocationPanel.KeyLocation = value;
      }
    }

    /// <summary>
    /// Gets and sets the private key location data source.
    /// </summary>
    public EntrySettings.LocationData PrivateKeyLocation {
      get {
        return privateKeyLocationPanel.KeyLocation;
      }
      set {
        privateKeyLocationPanel.KeyLocation = value;
      }
    }
  }
}
