//
//  KeyLocationPanel.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2013-2014,2017  David Lechner
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
          // on Mono, the bindings do not set the inital value properly
          if (Type.GetType("Mono.Runtime") != null) {
            if (KeyLocation != null && KeyLocation.SelectedType == null)
              KeyLocation.SelectedType = EntrySettings.LocationType.Attachment;
          }
        }
        FireKeyLocationChanged();
      }
    }

    public KeyLocationPanel()
    {
      InitializeComponent();

      // make transparent so tab styling shows
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;

      locationGroupBox.DataBindings["SelectedRadioButton"].Format += (s, e) =>
        {
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
          } else {
            Debug.Fail("unexpected");
          }
        };
      locationGroupBox.DataBindings["SelectedRadioButton"].Parse += (s, e) =>
        {
          if (e.DesiredType == typeof(EntrySettings.LocationType?) &&
              e.Value is string) {
            var valueString = e.Value as string;
            if (valueString == attachmentRadioButton.Name) {
              e.Value = EntrySettings.LocationType.Attachment;
            } else if (valueString == fileRadioButton.Name) {
              e.Value = EntrySettings.LocationType.File;
            } else {
              e.Value = null;
            }
          } else {
            Debug.Fail("unexpected");
          }
      };
      // workaround for BindingSource.BindingComplete event not working in Mono
      if (Type.GetType("Mono.Runtime") != null) {
        locationGroupBox.SelectedRadioButtonChanged += (s, e) => FireKeyLocationChanged();
        attachmentComboBox.SelectionChangeCommitted += (s, e) => FireKeyLocationChanged();
        saveKeyToTempFileCheckBox.CheckedChanged += (s, e) => FireKeyLocationChanged();
        fileNameTextBox.TextChanged += (s, e) => FireKeyLocationChanged();
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
      mPwEntryForm = ParentForm as PwEntryForm;
      ParentForm.FormClosing += delegate {
        while (populateComboBoxTimer.Enabled)
          Application.DoEvents();
      };
      UpdateControlStates();
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
      base.OnEnabledChanged(e);
      if (Enabled && !attachmentRadioButton.Checked && !fileRadioButton.Checked)
      {
        // Have to delay execution of this to avoid undesirable binding
        // interaction.
        PopulateComboBoxDelayed();
      }
    }

    void PopulateComboBoxDelayed()
    {
      populateComboBoxTimer.Start();
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
        browseButton.Focus();
      }
    }

    // MONO BUG: This does not run on Mono
    private void locationSettingsBindingSource_BindingComplete(object sender,
      BindingCompleteEventArgs e)
    {
      if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate) {
        e.Binding.BindingManagerBase.EndCurrentEdit();
        FireKeyLocationChanged();
      }
    }

    private void FireKeyLocationChanged()
    {
      if (KeyLocationChanged != null) {
        KeyLocationChanged(this, new EventArgs());
      }
    }

    private void populateComboBoxTimer_Tick(object sender, EventArgs e)
    {
      populateComboBoxTimer.Stop();
      attachmentRadioButton.Checked = true;
      if (attachmentComboBox.Items.Count > 0 &&
        attachmentComboBox.SelectedIndex == -1) {
        // select the first .ppk file in the list
        for (var i = 0; i < attachmentComboBox.Items.Count; i++) {
          var itemName = attachmentComboBox.Items[i] as string;
          if (itemName != null &&
            itemName.EndsWith(".ppk", StringComparison.OrdinalIgnoreCase)) {
            attachmentComboBox.SelectedIndex = i;
            return;
          }
        }
        // or the first file if a .ppk does not exist
        attachmentComboBox.SelectedIndex = 0;
      }
    }
  }
}
