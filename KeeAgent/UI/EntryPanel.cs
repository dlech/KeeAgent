// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2012-2016,2022 David Lechner <david@lechnology.com>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using KeePass.Forms;
using System.IO;
using dlech.SshAgentLib;
using KeePass.Util;
using KeePass.Util.Spr;
using KeePassLib.Security;
using SshAgentLib.Keys;

namespace KeeAgent.UI
{
  public partial class EntryPanel : UserControl
  {
    PwEntryForm pwEntryForm;
    KeeAgentExt ext;

    public EntrySettings InitialSettings {
      get;
      private set;
    }

    public EntrySettings CurrentSettings {
      get;
      private set;
    }

    public EntryPanel(KeeAgentExt ext)
    {
      this.ext = ext;
      InitializeComponent();

      // make transparent so tab styling shows
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;

      // fix up layout in Mono
      if (Type.GetType("Mono.Runtime") != null) {
        const int xOffset = -24;
        helpButton.Left += xOffset;
        keyInfoGroupBox.Width += xOffset;
        commentTextBox.Width -= 6;
        fingerprintTextBox.Width -= 6;
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      pwEntryForm = ParentForm as PwEntryForm;

      if (pwEntryForm != null) {
        InitialSettings = pwEntryForm.EntryRef.GetKeeAgentSettings();
        CurrentSettings = InitialSettings.DeepCopy();
        entrySettingsBindingSource.DataSource = CurrentSettings;

        pwEntryForm.FormClosing += delegate {
          while (delayedUpdateKeyInfoTimer.Enabled) {
            Application.DoEvents();
          }
        };
      }
      else {
        Debug.Fail("Don't have settings to bind to");
      }
      // replace the confirm constraint checkbox if the global confirm option is enabled
      if (ext.Options.AlwaysConfirm) {
        confirmConstraintCheckBox.ReplaceWithGlobalConfirmMessage();
      }
      UpdateControlStates();
    }

    private void UpdateControlStates()
    {
      addKeyAtOpenCheckBox.Enabled = hasSshKeyCheckBox.Checked;
      removeKeyAtCloseCheckBox.Enabled = hasSshKeyCheckBox.Checked;
      confirmConstraintCheckBox.Enabled = addKeyAtOpenCheckBox.Enabled;
      lifetimeConstraintCheckBox.Enabled = addKeyAtOpenCheckBox.Enabled;
      lifetimeConstraintNumericUpDown.Enabled = lifetimeConstraintCheckBox.Enabled
        && lifetimeConstraintCheckBox.Checked;
      openManageFilesDialogButton.Enabled = hasSshKeyCheckBox.Checked;
      UpdateKeyInfoDelayed();
    }

    void UpdateKeyInfoDelayed()
    {
      // Have to delay execution of this to avoid undesirable binding
      // interaction.
      if (!delayedUpdateKeyInfoTimer.Enabled) {
        delayedUpdateKeyInfoTimer.Start();
      }
    }

    void UpdateKeyInfo()
    {
      if (hasSshKeyCheckBox.Checked) {
        var getAttachment = new Func<string, ProtectedBinary>(name => {
          return pwEntryForm.EntryBinaries.Get(name);
        });

        var isLocationValid = UI.Validate.Location(
          CurrentSettings.Location,
          getAttachment) == null;

        invalidKeyWarningIcon.Visible = !isLocationValid;
      }

      try {
        var key = CurrentSettings.GetSshPrivateKey(pwEntryForm.EntryBinaries);

        commentTextBox.Text = key.PublicKey.Comment;
        fingerprintTextBox.Text = key.PublicKey.Sha256Hash;
        publicKeyTextBox.Text = key.PublicKey.AuthorizedKeysString;
        copyPublicKeybutton.Enabled = true;
      }
      catch (Exception) {
        commentTextBox.Text = string.Empty;
        fingerprintTextBox.Text = string.Empty;
        publicKeyTextBox.Text = string.Empty;
        copyPublicKeybutton.Enabled = false;
      }
    }

    private void hasSshKeyCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      UpdateControlStates();
    }

    private void lifetimeConstraintCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      UpdateControlStates();
    }

    private void helpButton_Click(object sender, EventArgs e)
    {
      Process.Start(Properties.Resources.WebHelpEntryOptions);
    }

    private void copyPublicKeybutton_Click(object sender, EventArgs e)
    {
      ClipboardUtil.Copy(publicKeyTextBox.Text, false, false, null, null, pwEntryForm.Handle);
    }

    private void delayedUpdateKeyIndoTimer_Tick(object sender, EventArgs e)
    {
      delayedUpdateKeyInfoTimer.Stop();
      UpdateKeyInfo();
    }

    private void openManageFilesDialogButton_Click(object sender, EventArgs e)
    {
      pwEntryForm.UpdateEntryBinaries(true, false);

      var dialog = new ManageKeyFileDialog {
        Attachments = new AttachmentBindingList(pwEntryForm.EntryBinaries),
        KeyLocation = CurrentSettings.Location.DeepCopy(),
      };

      var result = dialog.ShowDialog();

      if (result != DialogResult.OK) {
        return;
      }

      entrySettingsBindingSource.SuspendBinding();

      CurrentSettings.Location = dialog.KeyLocation;

      entrySettingsBindingSource.ResumeBinding();

      pwEntryForm.EntryBinaries.Clear();

      foreach (var entry in dialog.Attachments) {
        pwEntryForm.EntryBinaries.Set(entry.Key, entry.Value);
      }

      pwEntryForm.UpdateEntryBinaries(false, true);
      //pwEntryForm.ResizeColumnHeaders();
    }
  }
}
