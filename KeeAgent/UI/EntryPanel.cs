using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using KeePass.UI;
using KeePassLib;
using KeePass.Forms;

namespace KeeAgent.UI
{
  public partial class EntryPanel : UserControl
  {

    private PwEntryForm mPwEntryForm;

    public EntryPanel()
    {
      InitializeComponent();

      // make transparent so tab styling shows
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;

      locationGroupBox.DataBindings["SelectedRadioButton"].Format +=
        delegate(object aSender, ConvertEventArgs aEventArgs)
        {
          if (aEventArgs.DesiredType == typeof(string)) {
            var type = aEventArgs.Value as EntrySettings.LocationType?;
            switch (type) {
              case EntrySettings.LocationType.Attachment:
                aEventArgs.Value = attachmentRadioButton.Name;
                break;
              case EntrySettings.LocationType.File:
                aEventArgs.Value = fileRadioButton.Name;
                break;
              default:
                aEventArgs.Value = string.Empty;
                break;
            }
          } else {
            Debug.Fail("unexpected");
          }
        };
      locationGroupBox.DataBindings["SelectedRadioButton"].Parse +=
        delegate(object aSender, ConvertEventArgs aEventArgs)
        {
          if (aEventArgs.DesiredType == typeof(EntrySettings.LocationType?) &&
            aEventArgs.Value is string) {
            var valueString = aEventArgs.Value as string;
            if (valueString == attachmentRadioButton.Name) {
              aEventArgs.Value = EntrySettings.LocationType.Attachment;
            } else if (valueString == fileRadioButton.Name) {
              aEventArgs.Value = EntrySettings.LocationType.File;
            } else {
              aEventArgs.Value = null;
            }
          } else {
            Debug.Fail("unexpected");
          }
        };
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      mPwEntryForm = ParentForm as PwEntryForm;
      if (mPwEntryForm != null) {
        entrySettingsBindingSource.DataSource =
          mPwEntryForm.EntryRef.GetKeeAgentSettings();
        mPwEntryForm.FormClosing += mPwEntryForm_FormClosing;
      } else {
        Debug.Fail("Don't have settings to bind to");
      }
      UpdateControlStates();
    }

    private void mPwEntryForm_FormClosing(object aSender,
      FormClosingEventArgs aEventArgs)
    {
      if (mPwEntryForm.DialogResult == DialogResult.OK) {
        var settings = entrySettingsBindingSource.DataSource as EntrySettings;
        if (settings != null) {
          mPwEntryForm.EntryRef.SetKeeAgentSettings(settings);
        } else {
          Debug.Fail("Don't have settings");
        }
      }
    }

    private void UpdateControlStates()
    {
      addKeyAtOpenCheckBox.Enabled = hasSshKeyCheckBox.Checked;
      removeKeyAtCloseCheckBox.Enabled = hasSshKeyCheckBox.Checked;
      locationGroupBox.Enabled = hasSshKeyCheckBox.Checked;

      attachmentComboBox.Enabled = attachmentRadioButton.Checked;
      fileNameTextBox.Enabled = fileRadioButton.Checked;
      browseButton.Enabled = fileRadioButton.Checked;
    }

    private void hasSshKeyCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      UpdateControlStates();
    }

    private void radioButton_CheckedChanged(object sender, EventArgs e)
    {
      UpdateControlStates();
    }

    private void attachmentComboBox_VisibleChanged(object sender, EventArgs e)
    {
      if (attachmentComboBox.Visible) {
        attachmentComboBox.Items.Clear();
        if (mPwEntryForm != null) {
          foreach (var binary in mPwEntryForm.EntryBinaries) {
            attachmentComboBox.Items.Add(binary.Key);
          }
        } else {
          Debug.Fail("Don't have binaries");
        }
      }
    }

    private void browseButton_Click(object sender, EventArgs e)
    {
      try {
        openFileDialog.InitialDirectory = 
          Path.GetDirectoryName(fileNameTextBox.Text);
      } catch (Exception) { }
      var result = openFileDialog.ShowDialog();
      if (result == DialogResult.OK) {
        fileNameTextBox.Text = openFileDialog.FileName;
        fileNameTextBox.Focus();
      }
    }
  }
}
