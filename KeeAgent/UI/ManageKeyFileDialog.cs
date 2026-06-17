// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2022 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KeePassLib.Security;
using KeePassLib.Utility;
using SshAgentLib.Keys;

namespace KeeAgent.UI
{
  /// <summary>
  /// Dialog box for managing SSH key file locations.
  /// </summary>
  public partial class ManageKeyFileDialog : Form
  {
    private string passphrase;

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
        UpdateDecryptEnabled();
        if (privateKeyLocationPanel.IsGenerateSelected ||
            privateKeyLocationPanel.IsDecryptSelected) {
          privateKeyLocationPanel.ErrorMessage = null;
        }
        else {
          privateKeyLocationPanel.ErrorMessage = UI.Validate.Location(
            privateKeyLocationPanel.KeyLocation, getAttachment);
        }
      };
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      UpdateDecryptEnabled();
    }

    private void UpdateDecryptEnabled()
    {
      var canDecrypt = false;

      if (!string.IsNullOrEmpty(passphrase)) {
        try {
          var loc = privateKeyLocationPanel.KeyLocation;
          if (loc != null &&
              loc.SelectedType == EntrySettings.LocationType.Attachment &&
              !string.IsNullOrEmpty(loc.AttachmentName) &&
              Attachments != null) {
            var item = Attachments.FirstOrDefault(a => a.Key == loc.AttachmentName);
            if (item.Value != null) {
              using (var ms = new MemoryStream(item.Value.ReadData())) {
                var key = SshPrivateKey.Read(ms);
                canDecrypt = key.IsEncrypted;
              }
            }
          }
        }
        catch {
          // If we can't read the key, decrypt option stays disabled.
        }
      }

      privateKeyLocationPanel.SetDecryptEnabled(canDecrypt);
    }

    /// <summary>
    /// Sets the passphrase used to decrypt an encrypted key attachment.
    /// </summary>
    public string Passphrase {
      set { passphrase = value; }
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
    /// Sets the default comment pre-filled in the Generate option.
    /// </summary>
    public string DefaultComment {
      set { privateKeyLocationPanel.GenerateComment = value; }
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

    private void okButton_Click(object sender, EventArgs e)
    {
      if (privateKeyLocationPanel.IsDecryptSelected) {
        var attachName = privateKeyLocationPanel.DecryptAttachmentName;

        if (string.IsNullOrEmpty(attachName)) {
          MessageService.ShowWarning("KeeAgent: No key attachment selected.");
          return;
        }

        var item = Attachments == null
          ? default(KeyValuePair<string, ProtectedBinary>)
          : Attachments.FirstOrDefault(a => a.Key == attachName);

        if (item.Value == null) {
          MessageService.ShowWarning("KeeAgent: Selected attachment not found.");
          return;
        }

        byte[] privateKeyBytes, publicKeyBytes;

        try {
          SshKeyGenerator.Decrypt(
            item.Value.ReadData(),
            passphrase,
            out privateKeyBytes,
            out publicKeyBytes);
        }
        catch (Exception ex) {
          MessageService.ShowWarning(
            "KeeAgent: Failed to decrypt SSH key:", ex.Message);
          return;
        }

        var pubAttachName = attachName + ".pub";
        var snapshot = new List<KeyValuePair<string, ProtectedBinary>>(Attachments);
        snapshot.RemoveAll(a => a.Key == attachName || a.Key == pubAttachName);
        snapshot.Add(new KeyValuePair<string, ProtectedBinary>(
          attachName, new ProtectedBinary(false, privateKeyBytes)));
        snapshot.Add(new KeyValuePair<string, ProtectedBinary>(
          pubAttachName, new ProtectedBinary(false, publicKeyBytes)));
        Attachments.Clear();
        foreach (var entry in snapshot) {
          Attachments.Add(entry);
        }

        KeyLocation = new EntrySettings.LocationData {
          SelectedType = EntrySettings.LocationType.Attachment,
          AttachmentName = attachName,
        };

        DialogResult = DialogResult.OK;
        Close();
        return;
      }

      if (privateKeyLocationPanel.IsGenerateSelected) {
        if (Attachments != null && Attachments.Any(a => a.Key == "id_ed25519")) {
          var confirm = MessageBox.Show(
            this,
            "An SSH key attachment already exists. Replacing it will invalidate any " +
            "servers already configured with the current public key. Continue?",
            "Replace Existing Key?",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
          if (confirm != DialogResult.Yes) {
            return;
          }
        }

        byte[] privateKeyBytes, publicKeyBytes;

        try {
          SshKeyGenerator.Generate(
            privateKeyLocationPanel.GenerateComment,
            out privateKeyBytes,
            out publicKeyBytes);
        }
        catch (Exception ex) {
          MessageService.ShowWarning("KeeAgent: Failed to generate SSH key:", ex.Message);
          return;
        }

        // Rebuild Attachments, replacing any existing id_ed25519 pair
        var snapshot = new List<KeyValuePair<string, ProtectedBinary>>(Attachments);
        snapshot.RemoveAll(a => a.Key == "id_ed25519" || a.Key == "id_ed25519.pub");
        snapshot.Add(new KeyValuePair<string, ProtectedBinary>(
          "id_ed25519", new ProtectedBinary(false, privateKeyBytes)));
        snapshot.Add(new KeyValuePair<string, ProtectedBinary>(
          "id_ed25519.pub", new ProtectedBinary(false, publicKeyBytes)));
        Attachments.Clear();
        foreach (var item in snapshot) {
          Attachments.Add(item);
        }

        KeyLocation = new EntrySettings.LocationData {
          SelectedType = EntrySettings.LocationType.Attachment,
          AttachmentName = "id_ed25519",
        };
      }

      DialogResult = DialogResult.OK;
      Close();
    }
  }
}
