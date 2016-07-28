//
//  ManageDialog.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2012-2014  David Lechner
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
using System.Windows.Forms;
using dlech.SshAgentLib;
using KeePassLib.Utility;

namespace KeeAgent.UI
{
  public partial class ManageDialog : Form
  {
    private KeeAgentExt mExt;

    public ManageDialog(KeeAgentExt aExt)
    {
      InitializeComponent();

      if (Type.GetType("Mono.Runtime") == null) {
        Icon = Properties.Resources.KeeAgent_icon;
      } else {
        Icon = Properties.Resources.KeeAgent_icon_mono;

        // on windows, help button is displayed in the title bar
        // on mono, we need to add one in the window
        var helpButton = new Button();
        helpButton.Size = new Size(25, 25);
        helpButton.Image = Properties.Resources.Help_png;
        helpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        helpButton.Location = new Point(10, Height -10);
        helpButton.Click += (sender, e) => OnHelpRequested();
        var foundControls =
          keyInfoView.Controls.Find("buttonTableLayoutPanel", true);
        if (foundControls.Length > 0) {
          var buttonTableLayout = foundControls[0];
          var buttonTableLayoutParent = buttonTableLayout.Parent;
          buttonTableLayoutParent.Controls.Remove(buttonTableLayout);
          var buttonTableLayoutWrapper = new TableLayoutPanel();
          buttonTableLayoutWrapper.RowCount = 1;
          buttonTableLayoutWrapper.ColumnCount = 2;
          buttonTableLayoutWrapper.Anchor =
            AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
          buttonTableLayoutWrapper.Width = keyInfoView.dataGridView.Width;
          buttonTableLayoutWrapper.Height = helpButton.Height + 8;
          buttonTableLayoutWrapper.Controls.Add(helpButton);
          buttonTableLayoutWrapper.Controls.Add(buttonTableLayout);
          buttonTableLayoutParent.Controls.Add(buttonTableLayoutWrapper);
        }
      }

      // update title depending on Agent Mode
      mExt = aExt;
      if (mExt.agent is Agent) {
        Text += Translatable.TitleSuffixAgentMode;
      } else {
        Text += Translatable.TitleSuffixClientMode;
      }
      keyInfoView.SetAgent(mExt.agent);
    }

    protected override void OnShown (EventArgs e)
    {
      base.OnShown (e);
      keyInfoView.dataGridView.ClearSelection();
    }

    private void addButtonFromFileMenuItem_Click(object sender, EventArgs e)
    {
      keyInfoView.ShowFileOpenDialog();
    }

    private void addButtonFromKeePassMenuItem_Click(object sender, EventArgs e)
    {
      var openDatabaseCount =
        mExt.pluginHost.MainWindow.DocumentManager.GetOpenDatabases().Count;
      if (openDatabaseCount == 0) {
        MessageService.ShowWarning("No open databases found.",
          "Please open or unlock a database and then try again.");
        return;
      }

      var showConstraintControls = !(mExt.agent is PageantClient);
      var entryPicker =
        new EntryPickerDialog(mExt, showConstraintControls);
      var result = entryPicker.ShowDialog();
      if (result == DialogResult.OK) {
        try {
          mExt.AddEntry(entryPicker.SelectedEntry, entryPicker.Constraints);
        } catch (Exception) {
          // error message already shown
        }
      }
      if (mExt.agent is AgentClient) {
        keyInfoView.ReloadKeyListView();
      }
    }

    private void keyInfoView_AddFromFileHelpRequested(object sender, EventArgs e)
    {
      Process.Start(Properties.Resources.WebHelpAddFromFile);
    }

    private void ManageDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
    {
      OnHelpRequested();
    }

    private void ManageDialog_HelpButtonClicked(object sender, CancelEventArgs e)
    {
      OnHelpRequested();
      e.Cancel = true;
    }

    private void OnHelpRequested()
    {
      Process.Start(Properties.Resources.WebHelpKeeAgentManager);
    }

    private void ManageDialog_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Escape) {
        e.Handled = true;
        Close();
      }
    }
  }
}
