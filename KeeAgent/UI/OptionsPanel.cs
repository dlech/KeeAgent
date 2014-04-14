//
//  OptionsPanel.cs
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
using dlech.SshAgentLib;
using KeePass.UI;

namespace KeeAgent.UI
{
  public partial class OptionsPanel : UserControl
  {
    private KeeAgentExt mExt;
    private CheckedLVItemDXList mOptionsList;

    public OptionsPanel(KeeAgentExt aExt)
    {
      mExt = aExt;

      InitializeComponent();

      // make transparent so tab styling shows
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;

      modeComboBox.Items.Add(Translatable.OptionAgentModeAuto);
      modeComboBox.Items.Add(Translatable.OptionAgentModeAgent);
      modeComboBox.Items.Add(Translatable.OptionAgentModeClient);
      switch (mExt.Options.AgentMode) {
        case AgentMode.Client:
          modeComboBox.SelectedItem = Translatable.OptionAgentModeClient;
          break;
        case AgentMode.Server:
          modeComboBox.SelectedItem = Translatable.OptionAgentModeAgent;
          break;
        default:
          modeComboBox.SelectedItem = Translatable.OptionAgentModeAuto;
          break;
      }

      // additional configuration of list view
      customListViewEx.UseCompatibleStateImageBehavior = false;
      UIUtil.SetExplorerTheme(customListViewEx, false);

      mOptionsList = new CheckedLVItemDXList(customListViewEx, true);
      var agentModeOptionsGroup =
        new ListViewGroup("agentMode",
                          "Agent Mode Options (no effect in Client Mode)");
      customListViewEx.Groups.Add (agentModeOptionsGroup);
      mOptionsList.CreateItem(aExt.Options, "AlwaysConfirm", agentModeOptionsGroup,
        Translatable.OptionAlwaysConfirm);
      mOptionsList.CreateItem(aExt.Options, "ShowBalloon", agentModeOptionsGroup,
        Translatable.OptionShowBalloon);
      //mOptionsList.CreateItem(aExt.Options, "LoggingEnabled", optionsGroup,
      //  Translatable.optionLoggingEnabled);
      mOptionsList.CreateItem (aExt.Options, "UnlockOnActivity", agentModeOptionsGroup,
       Translatable.OptionUnlockOnActivity);
      columnHeader.Width = customListViewEx.ClientRectangle.Width -
        UIUtil.GetVScrollBarWidth() - 1;
      mOptionsList.UpdateData(false);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (ParentForm != null) {
        ParentForm.FormClosing +=
          delegate(object aSender, FormClosingEventArgs aEventArgs)
          {
            if (ParentForm.DialogResult == DialogResult.OK) {
              SaveChanges();
            }
            mOptionsList.Release();
          };
      }
    }

    private void SaveChanges()
    {
      mOptionsList.UpdateData(true);
      if (modeComboBox.Text == Translatable.OptionAgentModeAgent) {
        mExt.Options.AgentMode = AgentMode.Server;
      } else if (modeComboBox.Text == Translatable.OptionAgentModeClient) {
        mExt.Options.AgentMode = AgentMode.Client;
      } else {
        mExt.Options.AgentMode = AgentMode.Auto;
      }
    }

    private void helpButton_Click(object sender, EventArgs e)
    {
      Process.Start(Properties.Resources.WebHelpGlobalOptions);
    }
  }
}
