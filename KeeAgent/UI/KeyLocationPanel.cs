//
//  KeyLocationPanel.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2013  David Lechner
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using KeePass.Forms;

namespace KeeAgent.UI
{
  [DefaultBindingProperty("KeyLocation")]
  public partial class KeyLocationPanel : UserControl
  {

    private PwEntryForm mPwEntryForm;

    public event EventHandler KeyLocationChanged;

    [Category("Data")]
    public EntrySettings.LocationData KeyLocation
    {
      get
      {
        return locationSettingsBindingSource.DataSource as EntrySettings.LocationData;
      }
      set
      {
        if (DesignMode) { return; }
        if (object.ReferenceEquals(locationSettingsBindingSource.DataSource, value)) {
          return;
        }
        if (value == null) {
          locationSettingsBindingSource.DataSource = typeof(EntrySettings.LocationData);
        } else {
          locationSettingsBindingSource.DataSource = value;
        }
        OnKeyLocationChanged();
      }
    }

    public KeyLocationPanel()
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
      if (DesignMode) { return; }
      mPwEntryForm = ParentForm as PwEntryForm;
      UpdateControlStates();
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
      base.OnEnabledChanged(e);
      if (Enabled && !attachmentRadioButton.Checked && !fileRadioButton.Checked)
      {
        // Have to delay execution of this to avoid undesirable binding
        // interaction.
        var delayedInvokeTimer = new Timer();
        delayedInvokeTimer.Tick += (sender, e2) =>
        {
          delayedInvokeTimer.Stop();
          attachmentRadioButton.Checked = true;
          if (attachmentComboBox.Items.Count > 0 &&
            attachmentComboBox.SelectedIndex == -1)
          {
            // select the first .ppk file in the list
            for (var i = 0; i < attachmentComboBox.Items.Count; i++ )
            {
              var itemName = attachmentComboBox.Items[i] as string;
              if (itemName != null &&
                itemName.EndsWith(".ppk", StringComparison.OrdinalIgnoreCase))
              {
                attachmentComboBox.SelectedIndex = i;
                return;
              }
            }
            // or the first file if a .ppk does not exist
            attachmentComboBox.SelectedIndex = 0;
          }
        };
        delayedInvokeTimer.Interval = 100;
        delayedInvokeTimer.Start();
      }
    }

    private void UpdateControlStates()
    {
      attachmentComboBox.Enabled = attachmentRadioButton.Checked;
      saveKeyToTempFileCheckBox.Enabled = attachmentRadioButton.Checked;
      fileNameTextBox.Enabled = fileRadioButton.Checked;
      browseButton.Enabled = fileRadioButton.Checked;
    }

    private void radioButton_CheckedChanged(object sender, EventArgs e)
    {
      UpdateControlStates();
    }

    private void attachmentComboBox_VisibleChanged(object sender, EventArgs e)
    {
      if (DesignMode) { return; }
      if (attachmentComboBox.Visible) {
        attachmentComboBox.Items.Clear();
        if (mPwEntryForm != null) {
          foreach (var binary in mPwEntryForm.EntryBinaries) {
            attachmentComboBox.Items.Add(binary.Key);
          }
        } else {
          Debug.Fail("Don't have mPwEntryForm");
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
  }
}
