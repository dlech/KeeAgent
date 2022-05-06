// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2022 David Lechner <david@lechnology.com>

using System;
using System.Linq;
using System.Windows.Forms;
using KeePassLib.Security;

namespace KeeAgent.UI
{
  /// <summary>
  /// Dialog box for managing SSH key file locations.
  /// </summary>
  public partial class ManageKeyFileDialog : Form
  {
    /// <summary>
    /// Creates a new key file dialog.
    /// </summary>
    public ManageKeyFileDialog()
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
      privateKeyLocationPanel.BindingContext = new BindingContext();

      var getAttachment = new Func<string, ProtectedBinary>(name => {
        if (Attachments == null) {
          return null;
        }

        return Attachments.FirstOrDefault(a => a.Key == name).Value;
      });

      privateKeyLocationPanel.KeyLocationChanged += (s, e) => {
        privateKeyLocationPanel.ErrorMessage = UI.Validate.Location(
          privateKeyLocationPanel.KeyLocation, getAttachment);
      };
    }

    /// <summary>
    /// Gets and sets the attachments data source.
    /// </summary>
    public AttachmentBindingList Attachments {
      get {
        return privateKeyLocationPanel.Attachments;
      }
      set {
        privateKeyLocationPanel.Attachments = value;
      }
    }

    /// <summary>
    /// Gets and sets the private key location data source.
    /// </summary>
    public EntrySettings.LocationData KeyLocation {
      get {
        return privateKeyLocationPanel.KeyLocation;
      }
      set {
        privateKeyLocationPanel.KeyLocation = value;
      }
    }
  }
}
