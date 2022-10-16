// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2013-2014,2017,2022 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KeePass.App;
using KeePass.Resources;
using KeePass.UI;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeeAgent.UI
{
  [DefaultBindingProperty("KeyLocation")]
  public partial class KeyLocationPanel : UserControl
  {

    /// <summary>
    /// Gets and sets the group box title.
    /// </summary>
    [Description("The group box title."), Category("Appearance")]
    public string Title {
      get {
        return locationGroupBox.Text;
      }

      set {
        locationGroupBox.Text = value;
      }
    }

    public event EventHandler KeyLocationChanged;

    /// <summary>
    /// Gets and sets the attachments data source.
    /// </summary>
    [Category("Data")]
    public AttachmentBindingList Attachments {
      get {
        return attachmentComboBox.DataSource as AttachmentBindingList;
      }
      set {
        if (DesignMode) {
          return;
        }

        attachmentComboBox.DataSource = value;
      }
    }

    /// <summary>
    /// Gets and sets the key location settings data source.
    /// </summary>
    [Category("Data")]
    public EntrySettings.LocationData KeyLocation {
      get {
        return locationSettingsBindingSource.DataSource as EntrySettings.LocationData;
      }
      set {
        if (DesignMode) {
          return;
        }

        if (ReferenceEquals(locationSettingsBindingSource.DataSource, value)) {
          return;
        }

        if (value == null) {
          locationSettingsBindingSource.DataSource = typeof(EntrySettings.LocationData);
        }
        else {
          locationSettingsBindingSource.DataSource = value;

          // on Mono, the bindings do not set the initial value properly
          if (Type.GetType("Mono.Runtime") != null) {
            if (KeyLocation != null && KeyLocation.SelectedType == null)
              KeyLocation.SelectedType = EntrySettings.LocationType.Attachment;
          }
        }

        OnKeyLocationChanged();
      }
    }

    /// <summary>
    /// Gets and sets the error message.
    /// </summary>
    /// <remarks>
    /// Setting to <c>null</c> will hide the error message.
    /// </remarks>
    public string ErrorMessage {
      get { return errorMessage.Text; }
      set {
        errorMessage.Text = value;
        errorMessage.Visible = !string.IsNullOrEmpty(value);
      }
    }

    /// <summary>
    /// Creates a new key location panel control.
    /// </summary>
    public KeyLocationPanel()
    {
      InitializeComponent();

      // make transparent so tab styling shows
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;

      locationGroupBox.DataBindings["SelectedRadioButton"].Format += (s, e) => {
        if (e.DesiredType == typeof(string)) {
          var type = e.Value as EntrySettings.LocationType?;
          switch (type) {
            case EntrySettings.LocationType.Attachment:
              e.Value = attachmentRadioButton.Name;
              break;

            case EntrySettings.LocationType.File:
              e.Value = fileRadioButton.Name;
              break;

            default:
              e.Value = string.Empty;
              break;
          }
        }
        else {
          Debug.Fail("unexpected");
        }
      };

      locationGroupBox.DataBindings["SelectedRadioButton"].Parse += (s, e) => {
        if (e.DesiredType == typeof(EntrySettings.LocationType?) &&
            e.Value is string) {
          var valueString = e.Value as string;

          if (valueString == attachmentRadioButton.Name) {
            e.Value = EntrySettings.LocationType.Attachment;
          }
          else if (valueString == fileRadioButton.Name) {
            e.Value = EntrySettings.LocationType.File;
          }
          else {
            e.Value = null;
          }
        }
        else {
          Debug.Fail("unexpected");
        }
      };

      // workaround for BindingSource.BindingComplete event not working in Mono
      if (Type.GetType("Mono.Runtime") != null) {
        locationGroupBox.SelectedRadioButtonChanged += (s, e) => OnKeyLocationChanged();
        attachmentComboBox.SelectionChangeCommitted += (s, e) => OnKeyLocationChanged();
        saveKeyToTempFileCheckBox.CheckedChanged += (s, e) => OnKeyLocationChanged();
        fileNameTextBox.TextChanged += (s, e) => OnKeyLocationChanged();
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      if (DesignMode) {
        return;
      }

      if (Type.GetType("Mono.Runtime") != null) {
        // fix up layout in Mono
        const int xOffset = -72;
        attachmentComboBox.Width += xOffset;
        fileNameTextBox.Width += xOffset;
      }

      UpdateControlStates();
    }

    private void UpdateControlStates()
    {
      attachmentComboBox.Enabled = attachmentRadioButton.Checked;
      attachButton.Enabled = attachmentRadioButton.Checked;
      saveKeyToTempFileCheckBox.Enabled = attachmentRadioButton.Checked;
      fileNameTextBox.Enabled = fileRadioButton.Checked;
      browseButton.Enabled = fileRadioButton.Checked;
    }

    private void radioButton_CheckedChanged(object sender, EventArgs e)
    {
      UpdateControlStates();
    }

    private void attachButton_Click(object sender, EventArgs e)
    {
      var ofd = UIUtil.CreateOpenFileDialog(KPRes.AttachFiles,
        UIUtil.CreateFileTypeFilter(null, null, true), 1, null, true,
        AppDefs.FileDialogContext.Attachments);

      var result = ofd.ShowDialog(ParentForm);

      if (result == DialogResult.OK) {
        var name = AttachFile(ofd.FileName);

        if (name != null) {
          attachmentComboBox.Text = name;
        }
      }
    }

    private void browseButton_Click(object sender, EventArgs e)
    {
      try {
        openFileDialog.InitialDirectory =
          Path.GetDirectoryName(fileNameTextBox.Text);
      }
      catch (Exception) { }

      var result = openFileDialog.ShowDialog(ParentForm);

      if (result == DialogResult.OK) {
        fileNameTextBox.Text = openFileDialog.FileName;
        fileNameTextBox.Focus();
        browseButton.Focus();
      }
    }

    // MONO BUG: This does not run on Mono
    private void locationSettingsBindingSource_BindingComplete(object sender,
      BindingCompleteEventArgs e)
    {
      if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate) {
        e.Binding.BindingManagerBase.EndCurrentEdit();
        OnKeyLocationChanged();
      }
    }

    private void OnKeyLocationChanged()
    {
      if (KeyLocationChanged != null) {
        KeyLocationChanged(this, new EventArgs());
      }
    }

    /// <summary>
    /// Attaches a new file.
    /// </summary>
    /// <param name="strFile">
    /// The name of the file on the file system.
    /// </param>
    /// <returns>
    /// The name of the attachment on success, otherwise <c>null</c>.
    /// returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <remarks>
    /// This is a copy of KeyPass.Forms.PwEntryForm.BinImportFiles modified for this dialog.
    /// </remarks>
    private string AttachFile(string strFile)
    {
      if (strFile == null) {
        throw new ArgumentNullException("strFile");
      }


      if (string.IsNullOrWhiteSpace(strFile)) {
        throw new ArgumentException("empty file name", "strFile");
      }

      string strItem = UrlUtil.GetFileName(strFile);

      if (Attachments.Any(a => a.Key == strItem)) {
        string strMsg = KPRes.AttachedExistsAlready + MessageService.NewLine +
          strItem + MessageService.NewParagraph + KPRes.AttachNewRename +
          MessageService.NewParagraph + KPRes.AttachNewRenameRemarks0 +
          MessageService.NewLine + KPRes.AttachNewRenameRemarks1 +
          MessageService.NewLine + KPRes.AttachNewRenameRemarks2;
        DialogResult dr = MessageService.Ask(strMsg, null,
          MessageBoxButtons.YesNoCancel);

        if (dr == DialogResult.Cancel) {
          return null;
        }

        if (dr == DialogResult.Yes) {
          string strFileName = UrlUtil.StripExtension(strItem);
          string strExtension = "." + UrlUtil.GetExtension(strItem);

          int nTry = 0;

          while (true) {
            string strNewName = strFileName + nTry.ToString() + strExtension;

            if (!Attachments.Any(a => a.Key == strNewName)) {
              strItem = strNewName;
              break;
            }

            ++nTry;
          }
        }
      }

      try {
        // CheckAttachmentSize is internal, so disable for now
#if false
          if (!FileDialogsEx.CheckAttachmentSize(new FileInfo(strFile), KPRes.AttachFailed +
            MessageService.NewParagraph + strFile)) {
            continue;
          }
#endif

        byte[] vBytes = File.ReadAllBytes(strFile);
        // ConvertAttachment is internal, so disable for now
#if false
          vBytes = DataEditorForm.ConvertAttachment(strItem, vBytes);
#endif

        if (vBytes != null) {
          ProtectedBinary pb = new ProtectedBinary(false, vBytes);
          Attachments.Add(new KeyValuePair<string, ProtectedBinary>(strItem, pb));
        }

        return strItem;
      }
      catch (Exception exAttach) {
        MessageService.ShowWarning(KPRes.AttachFailed, strFile, exAttach);
        return null;
      }
    }
  }
}
