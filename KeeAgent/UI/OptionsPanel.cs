//
//  OptionsPanel.cs
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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using dlech.SshAgentLib;
using KeePass.UI;
using System.IO;
using KeePassLib.Utility;

namespace KeeAgent.UI
{
  public partial class OptionsPanel : UserControl
  {
    private KeeAgentExt ext;
    private CheckedLVItemDXList optionsList;

    public OptionsPanel(KeeAgentExt ext)
    {
      this.ext = ext;

      InitializeComponent();

      // make transparent so tab styling shows
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;

      modeComboBox.Items.Add(Translatable.OptionAgentModeAuto);
      modeComboBox.Items.Add(Translatable.OptionAgentModeAgent);
      modeComboBox.Items.Add(Translatable.OptionAgentModeClient);
      switch (ext.Options.AgentMode) {
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

      optionsList = new CheckedLVItemDXList(customListViewEx, true);
      var agentModeOptionsGroup = new ListViewGroup("agentMode",
                          "Agent Mode Options (no effect in Client Mode)");
      customListViewEx.Groups.Add(agentModeOptionsGroup);
      optionsList.CreateItem(ext.Options, "AlwaysConfirm", agentModeOptionsGroup,
        Translatable.OptionAlwaysConfirm);
      optionsList.CreateItem(ext.Options, "ShowBalloon", agentModeOptionsGroup,
        Translatable.OptionShowBalloon);
      //mOptionsList.CreateItem(aExt.Options, "LoggingEnabled", optionsGroup,
      //  Translatable.optionLoggingEnabled);
      optionsList.CreateItem (ext.Options, "UnlockOnActivity", agentModeOptionsGroup,
       Translatable.OptionUnlockOnActivity);
      columnHeader.Width = customListViewEx.ClientRectangle.Width -
        UIUtil.GetVScrollBarWidth() - 1;
      useCygwinSocketCheckBox.Checked = ext.Options.UseCygwinSocket;
      cygwinSocketPathTextBox.Text = ext.Options.CygwinSocketPath;
      useMsysSocketCheckBox.Checked = ext.Options.UseMsysSocket;
      msysSocketPathTextBox.Text = ext.Options.MsysSocketPath;
      optionsList.UpdateData(false);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (ParentForm != null) {
        ParentForm.FormClosing +=
          delegate(object sender, FormClosingEventArgs e2)
          {
            if (ParentForm.DialogResult == DialogResult.OK) {
              if (useCygwinSocketCheckBox.Checked
                && string.IsNullOrWhiteSpace(cygwinSocketPathTextBox.Text))
              {
                MessageService.ShowWarning("Must specify path for Cygwin socket.");
                e2.Cancel = true;
                return;
              }
              if (useMsysSocketCheckBox.Checked
                && string.IsNullOrWhiteSpace(msysSocketPathTextBox.Text))
              {
                MessageService.ShowWarning("Must specify path for MSYS socket.");
                e2.Cancel = true;
                return;
              }
              SaveChanges();
              if (ext.Options.UseCygwinSocket)
                ext.StartCygwinSocket();
              else
                ext.StopCygwinSocket();
              if (ext.Options.UseMsysSocket)
                ext.StartMsysSocket();
              else
                ext.StopMsysSocket();
            }
            optionsList.Release();
          };
      }
    }

    private void SaveChanges()
    {
      optionsList.UpdateData(true);
      if (modeComboBox.Text == Translatable.OptionAgentModeAgent) {
        ext.Options.AgentMode = AgentMode.Server;
      } else if (modeComboBox.Text == Translatable.OptionAgentModeClient) {
        ext.Options.AgentMode = AgentMode.Client;
      } else {
        ext.Options.AgentMode = AgentMode.Auto;
      }
      ext.Options.UseCygwinSocket = useCygwinSocketCheckBox.Checked;
      ext.Options.CygwinSocketPath = cygwinSocketPathTextBox.Text;
      ext.Options.UseMsysSocket = useMsysSocketCheckBox.Checked;
      ext.Options.MsysSocketPath = msysSocketPathTextBox.Text;
    }

    private void helpButton_Click(object sender, EventArgs e)
    {
      Process.Start(Properties.Resources.WebHelpGlobalOptions);
    }

    private void cygwinPathBrowseButton_Click(object sender, EventArgs e)
    {
      var file = browseForPath();
      if (file != null)
        cygwinSocketPathTextBox.Text = file;
    }

    private void msysPathBrowseButton_Click(object sender, EventArgs e)
    {
      var file = browseForPath();
      if (file != null)
        msysSocketPathTextBox.Text = file;
    }

    private string browseForPath()
    {
      // TODO: Would be nice if we could change the name of the "OK" button from
      // "Save" to "Select".
      var dialog = new SaveFileDialog()
      {
        Title = "Enter Socket File Name",
        Filter = "All files (*.*)|*.*",
        CheckFileExists = false,
        OverwritePrompt = false,
      };
      dialog.FileOk += (s, e) =>
      {
        if (File.Exists(dialog.FileName)) {
          MessageService.ShowWarning("File exists.", "Enter a new file name.");
          e.Cancel = true;
        }
      };
      if (dialog.ShowDialog() == DialogResult.Cancel)
        return null;
      return dialog.FileName;
    }
  }
}
