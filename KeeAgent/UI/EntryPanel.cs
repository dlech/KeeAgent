// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2012-2016,2022 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KeePass.Forms;
using KeePass.Util;
using KeePass.Util.Spr;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;
using dlech.SshAgentLib;
using SshAgentLib.Extension;
using SshAgentLib.Keys;

namespace KeeAgent.UI
{
  public partial class EntryPanel : UserControl
  {
    private PwEntryForm pwEntryForm;
    private readonly KeeAgentExt ext;
    private string editableCommentOriginal;
    private ISshKey pendingOldAgentKey;
    private byte[] pendingNewPrivKeyBytes;
    private byte[] pendingNewPubKeyBytes;

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

        if (Type.GetType("Mono.Runtime") != null) {
          // prevent crash
          destinationConstraintDataGridView.DataSource = null;
        }

        if (CurrentSettings.DestinationConstraints != null) {
          foreach (var c in CurrentSettings.DestinationConstraints) {
            destinationConstraintBindingSource.Add(c);
          }
        }

        if (Type.GetType("Mono.Runtime") != null) {
          destinationConstraintDataGridView.DataSource = destinationConstraintBindingSource;
          // binding complete event doesn't fire on mono
          destinationConstraintDataGridView.CellEndEdit += (s2, e2) => {
            destinationConstraintDataGridView_DataBindingComplete(
              s2, new DataGridViewBindingCompleteEventArgs(System.ComponentModel.ListChangedType.ItemChanged));
          };
        }

        pwEntryForm.FormClosing += (sender2, e2) => {
          while (delayedUpdateKeyInfoTimer.Enabled) {
            Application.DoEvents();
          }
          if (!e2.Cancel && pwEntryForm.DialogResult == DialogResult.OK) {
            ApplyPendingAgentReload();
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
      destinationConstraintCheckBox.Enabled = addKeyAtOpenCheckBox.Enabled;
      destinationConstraintDataGridView.Enabled = destinationConstraintCheckBox.Enabled
        && destinationConstraintCheckBox.Checked;
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

      var commentEditable = false;
      try {
        var key = CurrentSettings.TryGetSshPublicKey(pwEntryForm.EntryBinaries);

        commentTextBox.Text = key.Comment;
        fingerprintTextBox.Text = key.Sha256Fingerprint;
        publicKeyTextBox.Text = key.AuthorizedKeysString;
        copyPublicKeyButton.Enabled = true;

        // Comment is editable when the key is an unencrypted attachment.
        if (CurrentSettings.Location != null &&
            CurrentSettings.Location.SelectedType == EntrySettings.LocationType.Attachment &&
            !string.IsNullOrEmpty(CurrentSettings.Location.AttachmentName)) {
          var attach = pwEntryForm.EntryBinaries.Get(CurrentSettings.Location.AttachmentName);
          if (attach != null) {
            using (var ms = new MemoryStream(attach.ReadData())) {
              commentEditable = !SshPrivateKey.Read(ms).IsEncrypted;
            }
          }
        }
      }
      catch (Exception) {
        commentTextBox.Text = string.Empty;
        fingerprintTextBox.Text = string.Empty;
        publicKeyTextBox.Text = string.Empty;
        copyPublicKeyButton.Enabled = false;
      }

      commentTextBox.ReadOnly = !commentEditable;
      editableCommentOriginal = commentEditable ? commentTextBox.Text : null;
    }

    private void commentTextBox_Leave(object sender, EventArgs e)
    {
      if (commentTextBox.ReadOnly) return;
      if (commentTextBox.Text == editableCommentOriginal) return;
      ApplyCommentChange();
    }

    private void ApplyCommentChange()
    {
      var loc = CurrentSettings.Location;
      if (loc == null ||
          loc.SelectedType != EntrySettings.LocationType.Attachment ||
          string.IsNullOrEmpty(loc.AttachmentName)) {
        return;
      }

      var attachName = loc.AttachmentName;
      var attachment = pwEntryForm.EntryBinaries.Get(attachName);
      if (attachment == null) return;

      // On the first comment edit, capture the currently-loaded agent key so we
      // can replace it when the entry dialog closes with OK.  We defer the live
      // agent swap to FormClosing so a Cancel roll-back doesn't leave the agent
      // in an inconsistent state.
      if (pendingOldAgentKey == null && ext.agent != null) {
        try {
          var pubKey = CurrentSettings.TryGetSshPublicKey(pwEntryForm.EntryBinaries);
          if (pubKey != null) {
            pendingOldAgentKey = ext.agent.ListKeys()
              .FirstOrDefault(k => pubKey.Matches(k.GetPublicKeyBlob()));
          }
        }
        catch (Exception) { }
      }

      byte[] privateKeyBytes, publicKeyBytes;
      try {
        SshKeyGenerator.ChangeComment(
          attachment.ReadData(),
          commentTextBox.Text,
          out privateKeyBytes,
          out publicKeyBytes);
      }
      catch (Exception ex) {
        MessageService.ShowWarning("KeeAgent: Failed to update comment:", ex.Message);
        return;
      }

      pwEntryForm.EntryBinaries.Set(attachName, new ProtectedBinary(false, privateKeyBytes));
      pwEntryForm.EntryBinaries.Set(attachName + ".pub", new ProtectedBinary(false, publicKeyBytes));
      pwEntryForm.UpdateEntryBinaries(false, true);

      editableCommentOriginal = commentTextBox.Text;

      // Track the latest bytes; the FormClosing handler will use the final state.
      pendingNewPrivKeyBytes = privateKeyBytes;
      pendingNewPubKeyBytes = publicKeyBytes;

      UpdateKeyInfoDelayed();
    }

    private void ApplyPendingAgentReload()
    {
      if (pendingOldAgentKey == null || pendingNewPrivKeyBytes == null) return;
      try {
        SshPrivateKey newPrivKey;
        using (var ms = new MemoryStream(pendingNewPrivKeyBytes)) {
          newPrivKey = SshPrivateKey.Read(ms);
        }
        string newComment;
        var privateParam = newPrivKey.Decrypt(() => new byte[0], null, out newComment);

        SshPublicKey newPubKey;
        using (var ms = new MemoryStream(pendingNewPubKeyBytes)) {
          newPubKey = SshPublicKey.Read(ms);
        }

        var newKey = new SshKey(
          newPubKey.Parameter, privateParam, newComment,
          newPubKey.Nonce, newPubKey.Certificate);
        newKey.Source = pendingOldAgentKey.Source;
        newKey.DestinationConstraint = pendingOldAgentKey.DestinationConstraint;
        foreach (var c in pendingOldAgentKey.Constraints) {
          newKey.AddConstraint(c);
        }

        ext.RemoveKey(pendingOldAgentKey);
        ext.agent.AddKey(newKey);
      }
      catch (Exception) {
        // Not critical — old key stays loaded if reload fails.
      }
      finally {
        pendingOldAgentKey = null;
        pendingNewPrivKeyBytes = null;
        pendingNewPubKeyBytes = null;
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

    private void copyPublicKeyButton_Click(object sender, EventArgs e)
    {
      ClipboardUtil.Copy(publicKeyTextBox.Text, false, false, null, null, pwEntryForm.Handle);
    }

    private void delayedUpdateKeyInfoTimer_Tick(object sender, EventArgs e)
    {
      delayedUpdateKeyInfoTimer.Stop();
      UpdateKeyInfo();
    }

    private void openManageFilesDialogButton_Click(object sender, EventArgs e)
    {
      pwEntryForm.UpdateEntryBinaries(true, false);

      var dialog = new ManageKeyFileDialog {
        DefaultComment = pwEntryForm.EntryRef.Strings.ReadSafe(PwDefs.TitleField),
        Passphrase = SprEngine.Compile(
          pwEntryForm.EntryRef.Strings.ReadSafe(PwDefs.PasswordField),
          new SprContext(pwEntryForm.EntryRef, pwEntryForm.EntryRef.GetDatabase(), SprCompileFlags.Deref)),
        Attachments = new AttachmentBindingList(pwEntryForm.EntryBinaries),
        KeyLocation = CurrentSettings.Location.DeepCopy(),
      };

      var result = dialog.ShowDialog(ParentForm);

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

      // probably only needed for mono, but doesn't hurt to call it unconditionally
      UpdateControlStates();
    }

    private void destinationConstraintDataGridView_EnabledChanged(object sender, EventArgs e)
    {
      // update styles to make it look disabled
      var dgv = sender as DataGridView;

      if (dgv.Enabled) {
        dgv.DefaultCellStyle.BackColor = SystemColors.Window;
        dgv.DefaultCellStyle.ForeColor = SystemColors.ControlText;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Window;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
        dgv.ReadOnly = false;
        dgv.EnableHeadersVisualStyles = true;
      }
      else {
        dgv.DefaultCellStyle.BackColor = SystemColors.Control;
        dgv.DefaultCellStyle.ForeColor = SystemColors.GrayText;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.GrayText;
        dgv.CurrentCell = null;
        dgv.ReadOnly = true;
        dgv.EnableHeadersVisualStyles = false;
      }

      if (Type.GetType("Mono.Runtime") != null) {
        // Mono bug: this doesn't happen automatically when changing the
        // properties above like on Windows.
        dgv.Refresh();
      }
    }

    private void destinationConstraintDataGridView_MouseUp(object sender, MouseEventArgs e)
    {
      var hitTest = destinationConstraintDataGridView.HitTest(e.X, e.Y);

      if (hitTest.Type == DataGridViewHitTestType.RowHeader) {
        var clickedRow = destinationConstraintDataGridView.Rows[hitTest.RowIndex];

        destinationConstraintDataGridView.CurrentCell = clickedRow.Cells[0];

        if (destinationConstraintDataGridView.CurrentRow.IsNewRow) {
          // can't remove uncommitted new row
          deleteCurrentRowMenuItem.Enabled = false;
        }
        else {
          deleteCurrentRowMenuItem.Enabled = true;
        }
      }
      else if (hitTest.Type == DataGridViewHitTestType.Cell) {
        var clickedCell = destinationConstraintDataGridView.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex];

        destinationConstraintDataGridView.CurrentCell = clickedCell;

        if (destinationConstraintDataGridView.CurrentRow.IsNewRow) {
          // can't remove uncommitted new row
          deleteCurrentRowMenuItem.Enabled = false;
        }
        else {
          deleteCurrentRowMenuItem.Enabled = true;
        }
      }
      else {
        deleteCurrentRowMenuItem.Enabled = false;
      }
    }

    private void deleteCurrentRowMenuItem_Click(object sender, EventArgs e)
    {
      try {
        destinationConstraintDataGridView.Rows.Remove(destinationConstraintDataGridView.CurrentRow);
      }
      catch {
        // don't crash the program
      }
    }

    private void destinationConstraintDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
    {
      if (CurrentSettings == null) {
        // this is before the form load event
        return;
      }

      var list = new List<EntrySettings.DestinationConstraint>();

      try {
        foreach (DataGridViewRow row in destinationConstraintDataGridView.Rows) {
          if (row.IsNewRow) {
            continue;
          }

          var c = row.DataBoundItem as EntrySettings.DestinationConstraint;
          list.Add(c.DeepCopy());

          try {
            new DestinationConstraint.Constraint(
              new DestinationConstraint.Hop(null, c.FromHost, c.FromHostKeys.Select(
                f => new DestinationConstraint.KeySpec(SshPublicKey.Parse(f.HostKey), f.IsCA)).ToList()),
              new DestinationConstraint.Hop(c.ToUser, c.ToHost, c.ToHostKeys.Select(
                t => new DestinationConstraint.KeySpec(SshPublicKey.Parse(t.HostKey), t.IsCA)).ToList()));

            row.ErrorText = string.Empty;
          }
          catch (Exception ex) {
            row.ErrorText = ex.Message;
          }
        }

        CurrentSettings.DestinationConstraints = list.ToArray();
      }
      catch {
        // data is invalid
      }
    }

    private void destinationConstraintDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0) {
        // clicked the header
        return;
      }

      var grid = sender as DataGridView;

      if (grid.Rows[e.RowIndex].IsNewRow) {
        // don't allow on new rows
        return;
      }

      if (grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn) {
        var dialog = new HostKeysDialog();

        var hostName = grid.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value;
        dialog.Text = string.Format("Host keys for '{0}'", hostName);

        var constraint = grid.Rows[e.RowIndex].DataBoundItem as EntrySettings.DestinationConstraint;
        var property = constraint.GetType().GetProperty(grid.Columns[e.ColumnIndex].DataPropertyName);
        var keySpecs = property.GetValue(constraint) as EntrySettings.DestinationConstraint.KeySpec[];

        if (keySpecs != null) {
          foreach (var k in keySpecs) {
            dialog.AddKeySpec(k);
          }
        }

        var result = dialog.ShowDialog(this);

        if (result == DialogResult.OK) {
          property.SetValue(constraint, dialog.GetKeySpecs());

          // binding complete event doesn't fire on mono
          destinationConstraintDataGridView_DataBindingComplete(
            sender, new DataGridViewBindingCompleteEventArgs(System.ComponentModel.ListChangedType.ItemChanged));
        }
      }
    }

    private void destinationConstraintDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      var grid = sender as DataGridView;

      // don't draw buttons on new rows
      if (e.ColumnIndex >= 0 && grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn
        && e.RowIndex >= 0 && grid.Rows[e.RowIndex].IsNewRow) {
        e.PaintBackground(e.ClipBounds, true);
        e.Handled = true;
      }
    }

    private void destinationConstraintCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      UpdateControlStates();
    }
  }
}
