//
//  EntryPanel.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2012-2013  David Lechner
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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using KeePass.Forms;
using System.IO;

namespace KeeAgent.UI
{
  public partial class EntryPanel : UserControl
  {
    private PwEntryForm mPwEntryForm;

    public EntrySettings IntialSettings {
      get;
      private set;
    }

    public EntrySettings CurrentSettings {
      get;
      private set;
    }

    public EntryPanel()
    {
      InitializeComponent();

      // make transparent so tab styling shows
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      mPwEntryForm = ParentForm as PwEntryForm;
      if (mPwEntryForm != null) {
        IntialSettings =
          mPwEntryForm.EntryRef.GetKeeAgentSettings();
        CurrentSettings = (EntrySettings)IntialSettings.Clone ();
        entrySettingsBindingSource.DataSource = CurrentSettings;
      } else {
        Debug.Fail("Don't have settings to bind to");
      }
      UpdateControlStates();
    }

    private void UpdateControlStates()
    {
      addKeyAtOpenCheckBox.Enabled = hasSshKeyCheckBox.Checked;
      removeKeyAtCloseCheckBox.Enabled = hasSshKeyCheckBox.Checked;
      keyLocationPanel.Enabled = hasSshKeyCheckBox.Checked;
      if (hasSshKeyCheckBox.Checked) {
        switch (keyLocationPanel.KeyLocation.SelectedType) {
          case EntrySettings.LocationType.Attachment:
            try {

            } catch (Exception) {
              commentTextBox.Text = "Error loading key from attachment";
              fingerprintTextBox.Text = string.Empty;
              publicKeyTextBox.Text = string.Empty;
            }
            break;
          case EntrySettings.LocationType.File:
            try {              

            } catch (Exception) {
              commentTextBox.Text = string.Format("Error loading key from {0}",
                Path.GetFullPath(keyLocationPanel.KeyLocation.FileName));
              fingerprintTextBox.Text = string.Empty;
              publicKeyTextBox.Text = string.Empty;
            }
            break;
          default:
            commentTextBox.Text = "No key selected";
            fingerprintTextBox.Text = string.Empty;
            publicKeyTextBox.Text = string.Empty;
            break;
        }
      } else {
        commentTextBox.Text = string.Empty;
        fingerprintTextBox.Text = string.Empty;
        publicKeyTextBox.Text = string.Empty;
      }
    }

    private void hasSshKeyCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      UpdateControlStates();
    }

    private void helpButton_Click(object sender, EventArgs e)
    {
      Process.Start(Properties.Resources.WebHelpEntryOptions);
    }
  }
}
