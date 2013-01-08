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

namespace KeeAgent.UI
{
  public partial class EntryPanel : UserControl
  {

    private PwEntry mPwEntry;

    public EntryPanel(PwEntry aPwEntry)
    {
      mPwEntry = aPwEntry;

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

      entrySettingsBindingSource.DataSource = mPwEntry.GetKeeAgentSettings();

      UpdateControlStates();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (ParentForm != null) {
        ParentForm.FormClosing +=
          delegate(object aSender, FormClosingEventArgs aEventArgs)
          {
            if (ParentForm.DialogResult == DialogResult.OK) {
              var settings = entrySettingsBindingSource.DataSource as EntrySettings;
              mPwEntry.SetKeeAgentSettings(settings);
            }
          };
      }
    }

    private void UpdateControlStates()
    {
      addKeyAtOpenCheckBox.Enabled = hasSshKeyCheckBox.Checked;
      locationGroupBox.Enabled = hasSshKeyCheckBox.Checked;

      attachmentComboBox.Enabled = attachmentRadioButton.Checked;
      fileNameTextBox.Enabled = fileRadioButton.Checked;
      browseButton.Enabled = fileRadioButton.Checked;
    }

    private void hasSshKeyCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if (!hasSshKeyCheckBox.Checked) {
        addKeyAtOpenCheckBox.Checked = false;
      }
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
        foreach (var binary in mPwEntry.Binaries) {
          attachmentComboBox.Items.Add(binary.Key);
        }
      }
    }

    private void browseButton_Click(object sender, EventArgs e)
    {
      try {
        openFileDialog.InitialDirectory = Path.GetDirectoryName(fileNameTextBox.Text);
      } catch (Exception) { }
      var result = openFileDialog.ShowDialog();
      if (result == DialogResult.OK) {
        fileNameTextBox.Text = openFileDialog.FileName;
      }
    }
  }
}
