using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using dlech.SshAgentLib;
using dlech.SshAgentLib.WinForms;
using KeeAgent.Properties;
using KeeAgent.UI;
using KeePass.App;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib.Utility;
using System.Security;
using KeePassLib;
using System.Text;
using System.ComponentModel;

namespace KeeAgent
{
  public sealed partial class KeeAgentExt : Plugin
  {
    internal IPluginHost mPluginHost;
    internal bool mDebug;
    internal IAgent mAgent;

    private ToolStripMenuItem mKeeAgentMenuItem;
    private ToolStripMenuItem mKeeAgentPwEntryContextMenuItem;
    private ToolStripMenuItem mNotifyIconContextMenuItem;
    private List<string> mApprovedKeys;
    private List<ISshKey> mRemoveKeyList;
    private UIHelper mUIHelper;

    private const string cPluginName = "KeeAgent";
    private const string cAlwaysConfirmOptionName = cPluginName + ".AlwaysConfirm";
    private const string cShowBalloonOptionName = cPluginName + ".ShowBalloon";
    private const string cNotificationOptionName = cPluginName + ".Notification";
    private const string cLogginEnabledOptionName = cPluginName + ".LoggingEnabled";
    private const string cLogFileNameOptionName = cPluginName + ".LogFileName";
    private const string cAgentModeOptionName = cPluginName + ".AgentMode";

    public Options Options { get; private set; }

    public override bool Initialize(IPluginHost aHost)
    {
      bool success;

      mPluginHost = aHost;
      mUIHelper = new UIHelper(mPluginHost);
      mRemoveKeyList = new List<ISshKey>();
      mDebug = (mPluginHost
          .CommandLineArgs[AppDefs.CommandLineOptions.Debug] != null);

      LoadOptions();
      mApprovedKeys = new List<string>();

      if (mDebug) Log("Loading KeeAgent...");

      success = false;
      try {
        // TODO check OS - currently only works on Windows
        if (Options.AgentMode != AgentMode.Client) {
          try {
            var pagent = new PageantAgent();
            pagent.Locked += Pageant_Locked;
            pagent.KeyUsed += Pageant_KeyUsed;
            pagent.KeyListChanged += Pageant_KeyListChanged;
            mAgent = pagent;
          } catch (PageantRunningException) {
            if (Options.AgentMode != AgentMode.Auto) {
              throw;
            }
          }
        }
        if (mAgent == null) {
          mAgent = new PageantClient();
        }
        mPluginHost.MainWindow.FileOpened += MainForm_FileOpened;
        mPluginHost.MainWindow.FileClosingPost += MainForm_FileClosing;
        mPluginHost.MainWindow.FileClosed += MainForm_FileClosed;
        // load all database that are already opened
        foreach (var database in mPluginHost.MainWindow.DocumentManager.Documents) {
          MainForm_FileOpened(this, new FileOpenedEventArgs(database.Database));
        }
        success = true;
        if (mDebug) Log("Succeeded");
      } catch (PageantRunningException) {
        ShowPageantRunningErrorMessage();
      } catch (Exception) {
        if (mDebug) Log("Failed");
      }
      if (success) {
        AddMenuItems();
      }
      GlobalWindowManager.WindowAdded += WindowAddedHandler;
      return success;
    }

    public override void Terminate()
    {
      GlobalWindowManager.WindowAdded -= WindowAddedHandler;
      if (mDebug) Log("Terminating KeeAgent");
      var pagent = mAgent as PageantAgent;
      if (pagent != null) {
        // need to shutdown agent or app won't exit
        pagent.Dispose();
      }
      RemoveMenuItems();
    }

    public override Image SmallIcon
    {
      get { return Resources.KeeAgentIcon; }
    }

    private void ShowPageantRunningErrorMessage()
    {
      MessageService.ShowWarning(new object[] {
                Translatable.ErrPageantRunning
            });
    }

    private void AddMenuItems()
    {
      /* add item to Tools menu */
      mKeeAgentMenuItem = new ToolStripMenuItem();
      mKeeAgentMenuItem.Text = Translatable.KeeAgent;
      mKeeAgentMenuItem.ToolTipText = Translatable.KeeAgentMenuItemToolTip;
      mKeeAgentMenuItem.Image = Resources.KeeAgentIcon;
      mKeeAgentMenuItem.Click += manageKeeAgentMenuItem_Click;
      mPluginHost.MainWindow.ToolsMenu.DropDownItems.Add(mKeeAgentMenuItem);

      /* add item to Password Entry context menu */
      var result = mPluginHost.MainWindow.Controls.Find("m_lvEntries", true);
      if (result.Length > 0) {
        var entryListView = result[0] as CustomListViewEx;
        if (entryListView != null) {
          var pwEntryContextMenu = entryListView.ContextMenuStrip;
          if (pwEntryContextMenu != null) {
            mKeeAgentPwEntryContextMenuItem = new ToolStripMenuItem();
            mKeeAgentPwEntryContextMenuItem.Text =
              Translatable.AddToKeeAgentContextMenuItem;
            mKeeAgentPwEntryContextMenuItem.Click +=
              mKeeAgentPwEntryContextMenuItem_Clicked;
            mKeeAgentPwEntryContextMenuItem.Image = Resources.KeeAgentIcon;
            var firstSeparatorIndex =
              pwEntryContextMenu.Items.IndexOfKey("m_ctxEntrySep0");
            pwEntryContextMenu.Items.Insert(firstSeparatorIndex,
              mKeeAgentPwEntryContextMenuItem);
            pwEntryContextMenu.Opening += PwEntry_ContextMenu_Opening;
          }
        }
      }

      /* add item to notification icon context menu */
      mNotifyIconContextMenuItem = new ToolStripMenuItem();
      mNotifyIconContextMenuItem.Text = Translatable.KeeAgent;
      mNotifyIconContextMenuItem.ToolTipText = Translatable.KeeAgentMenuItemToolTip;
      mNotifyIconContextMenuItem.Image = Resources.KeeAgentIcon;
      mNotifyIconContextMenuItem.Click += manageKeeAgentMenuItem_Click;
      var notifyIconContextMenu =
        mPluginHost.MainWindow.MainNotifyIcon.ContextMenuStrip;
      var secondSeparatorIndex =
              notifyIconContextMenu.Items.IndexOfKey("m_ctxTraySep1");
      notifyIconContextMenu.Items.Insert(secondSeparatorIndex,
        mNotifyIconContextMenuItem);
    }

    private void PwEntry_ContextMenu_Opening(object aSender, CancelEventArgs aArgs)
    {
      foreach (var entry in mPluginHost.MainWindow.GetSelectedEntries()) {
        // if any selected entry contains an SSH key then we show the KeeAgent menu item
        if (entry.GetKeeAgentEntrySettings().HasSshKey) {
          mKeeAgentPwEntryContextMenuItem.Visible = true;
          return;
        }
      }
      mKeeAgentPwEntryContextMenuItem.Visible = false;
    }

    private void mKeeAgentPwEntryContextMenuItem_Clicked(object aSender, EventArgs aArgs)
    {
      foreach (var entry in mPluginHost.MainWindow.GetSelectedEntries()) {
        // if any selected entry contains an SSH key then we show the KeeAgent menu item
        if (entry.GetKeeAgentEntrySettings().HasSshKey) {
          AddEntry(entry);
        }
      }
    }

    private void RemoveMenuItems()
    {
      if (mPluginHost != null && mPluginHost.MainWindow != null &&
          mKeeAgentMenuItem != null) {

        /* get Tools menu */
        ToolStripMenuItem toolsMenu = mPluginHost.MainWindow.ToolsMenu;
        /* remove items from tools menu */
        toolsMenu.DropDownItems.Remove(mKeeAgentMenuItem);
      }
    }

    private void manageKeeAgentMenuItem_Click(object aSource, EventArgs aEvent)
    {
      ShowManageDialog();
    }

    private void ShowManageDialog()
    {
      using (ManageDialog dialog = new ManageDialog(this)) {
        dialog.ShowDialog(mPluginHost.MainWindow);
      }
    }

    internal void SaveOptions()
    {
      var config = mPluginHost.CustomConfig;
      config.SetString(cAlwaysConfirmOptionName, Options.AlwasyConfirm.ToString());
      config.SetString(cShowBalloonOptionName, Options.ShowBalloon.ToString());
      config.SetBool(cLogginEnabledOptionName, Options.LoggingEnabled);
      config.SetString(cLogFileNameOptionName, Options.LogFileName);
      config.SetString(cAgentModeOptionName, Options.AgentMode.ToString());
    }

    private void LoadOptions()
    {
      Options = new Options();
      var config = mPluginHost.CustomConfig;

      Options.AlwasyConfirm = config.GetBool(cAlwaysConfirmOptionName, false);
      Options.ShowBalloon = config.GetBool(cShowBalloonOptionName, true);
      Options.LoggingEnabled = config.GetBool(cLogginEnabledOptionName, false);

      string defaultLogFileNameValue = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
          "KeeAgent.log");
      string configFileLogFileNameValue =
          config.GetString(cLogFileNameOptionName);
      if (string.IsNullOrEmpty(configFileLogFileNameValue)) {
        Options.LogFileName = defaultLogFileNameValue;
      } else {
        Options.LogFileName = configFileLogFileNameValue;
      }

      AgentMode configAgentMode;
      if (Enum.TryParse<AgentMode>(config.GetString(cAgentModeOptionName),
        out configAgentMode)) {
        Options.AgentMode = configAgentMode;
      } else {
        Options.AgentMode = AgentMode.Auto;
      }

      /* the Notification option is obsolete, so we read it and then clear it. */
      NotificationOptions configFileNotificationValue;
      if (Enum.TryParse<NotificationOptions>(config
        .GetString(cNotificationOptionName), out configFileNotificationValue)) {

        switch (configFileNotificationValue) {
          case NotificationOptions.AlwaysAsk:
          case NotificationOptions.AskOnce:
            Options.AlwasyConfirm = true;
            break;
          case NotificationOptions.Never:
            Options.ShowBalloon = false;
            break;
        }
        config
             .SetString(cNotificationOptionName, string.Empty);
      }


    }

    /// <summary>
    /// Modifies various Forms in KeePass
    /// </summary>
    ///<remarks>
    /// Kudos to the luckyrat for figuring out how to to this in KeePassRPC
    /// (KeeFox) and open-sourcing the code so I could copy/learn from it.
    ///</remarks>
    private void WindowAddedHandler(object aSender, GwmWindowEventArgs aEventArgs)
    {
      /* Add KeeAgent tab to PwEntryForm dialog */
      var pwEntryForm = aEventArgs.Form as PwEntryForm;
      if (pwEntryForm != null) {
        pwEntryForm.Shown +=
          delegate(object sender, EventArgs args)
          {
            var optionsPanel = new EntryPanel(pwEntryForm.EntryRef);
            AddTab(pwEntryForm, optionsPanel);
          };
      }

      /* Add KeeAgent tab to Options dialog */
      var optionsForm = aEventArgs.Form as OptionsForm;
      if (optionsForm != null) {
        optionsForm.Shown +=
          delegate(object sender, EventArgs args)
          {
            var optionsPanel = new OptionsPanel(this);
            AddTab(optionsForm, optionsPanel);
          };
        optionsForm.FormClosed += OptionsFormClosedHandler;
      }
    }

    private void AddTab(Form aForm, UserControl aPanel)
    {
      try {
        var foundControls = aForm.Controls.Find("m_tabMain", true);
        if (foundControls.Length != 1) {
          return;
        }
        var tabControl = foundControls[0] as TabControl;
        if (tabControl == null) {
          return;
        }
        if (tabControl.ImageList == null) {
          tabControl.ImageList = new ImageList();
        }
        var imageIndex = tabControl.ImageList.Images.Add(Resources.KeeAgentIcon, Color.Transparent);
        var newTab = new TabPage(Translatable.KeeAgent);
        newTab.ImageIndex = imageIndex;
        //newTab.ImageKey = cTabImageKey;
        newTab.UseVisualStyleBackColor = true;
        newTab.Controls.Add(aPanel);
        tabControl.Controls.Add(newTab);
      } catch (Exception ex) {
        // Can't have exception here or KeePass freezes.
        Debug.Fail(ex.ToString());
      }
    }

    private void OptionsFormClosedHandler(object aSender,
      FormClosedEventArgs aEventArgs)
    {
      var optionsForm = aSender as OptionsForm;
      if (optionsForm != null && optionsForm.DialogResult == DialogResult.OK) {
        SaveOptions();
      }
    }

    internal void Log(string aMessage)
    {
      if (Options.LoggingEnabled) {
        try {
          File.AppendAllText(Options.LogFileName,
              DateTime.Now + ": " + aMessage + "\r\n");
        } catch { }
      }
    }

    private void Pageant_Locked(object aSender, Agent.LockEventArgs aEventArgs)
    {
      if (Options.ShowBalloon) {
        string notifyText;
        if (aEventArgs.IsLocked) {
          notifyText = Translatable.NotifyLocked;
        } else {
          notifyText = Translatable.NotifyUnlocked;
        }
        mUIHelper.ShowBalloonNotification(notifyText);
      }
    }

    private void Pageant_KeyListChanged(object aSender,
      Agent.KeyListChangeEventArgs aEventArgs)
    {
      if (Options.AlwasyConfirm &&
          aEventArgs.Action == Agent.KeyListChangeEventAction.Add &&
          !aEventArgs.Key.HasConstraint(
            Agent.KeyConstraintType.SSH_AGENT_CONSTRAIN_CONFIRM)) {

        var constraint = new Agent.KeyConstraint();
        constraint.Type = Agent.KeyConstraintType.SSH_AGENT_CONSTRAIN_CONFIRM;
        aEventArgs.Key.AddConstraint(constraint);
      }
    }

    private void Pageant_KeyUsed(object aSender, Agent.KeyUsedEventArgs aEventArgs)
    {
      if (Options.ShowBalloon) {
        string notifyText = string.Format(Translatable.NotifyKeyFetched,
          aEventArgs.Key.Comment);
        mUIHelper.ShowBalloonNotification(notifyText);
      }
    }

    private void MainForm_FileOpened(object aSender,
      FileOpenedEventArgs aEventArgs)
    {
      try {
        var exitFor = false;
        foreach (var entry in aEventArgs.Database.RootGroup.GetEntries(true)) {
          if (exitFor) {
            break;
          }
          var settings = entry.GetKeeAgentEntrySettings();
          if (settings.AddAtDatabaseOpen) {
            try {
              AddEntry(entry);
            } catch (Exception) {
              // TODO better error handling
              var result = MessageBox.Show(
                "Agent failure. Key could not be added. Do you want to attempt to load additional keys?",
                "KeeAgent", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
              if (result == DialogResult.No) {
                exitFor = true;
              }
            }
          }
        }
      } catch (Exception ex) {
        // can't be crashing KeePass
        Debug.Fail(ex.ToString());
      }
    }

    private void MainForm_FileClosing(object aSender,
      FileClosingEventArgs aEventArgs)
    {
      try {
        mRemoveKeyList.Clear();
        var allKeys = mAgent.GetAllKeys();
        foreach (var entry in aEventArgs.Database.RootGroup.GetEntries(true)) {
          try {
            var settings = entry.GetKeeAgentEntrySettings();
            if (settings.RemoveAtDatabaseClose) {
              var matchKey = entry.GetSshKey();
              if (matchKey == null) {
                continue;
              }
              var removeKey = allKeys.Get(matchKey.Version, matchKey.GetPublicKeyBlob());
              if (removeKey == null) {
                continue;
              }
              mRemoveKeyList.Add(removeKey);
            }
          } catch (Exception ex) {
            // keep trying the rest of the keys
            Debug.Fail(ex.ToString());
          }
        }
      } catch (Exception ex) {
        // can't be crashing KeePass
        Debug.Fail(ex.ToString());
      }
    }

    private void MainForm_FileClosed(object aSender,
      FileClosedEventArgs aEventArgs)
    {
      try {
        foreach (var key in mRemoveKeyList) {
          mAgent.RemoveKey(key);
        }
        mRemoveKeyList.Clear();
      } catch (Exception ex) {
        // can't be crashing KeePass
        Debug.Fail(ex.ToString());
      }
    }

    public ISshKey AddEntry(PwEntry aEntry)
    {
      var settings = aEntry.GetKeeAgentEntrySettings();
      try {
        var key = aEntry.GetSshKey();
        if (Options.AlwasyConfirm) {
          key.addConfirmConstraint();
        }
        mAgent.AddKey(key);
        return key;
      } catch (Exception ex) {
        if (ex is FileNotFoundException || ex is DirectoryNotFoundException) {
          MessageBox.Show("Could not find file " + settings.Location.FileName);
        } else if (ex is KeyFormatterException || ex is PpkFormatterException) {
          MessageBox.Show("Bad passphrase " + settings.Location.FileName);
        } else {
          Debug.Fail(ex.ToString());
        }
        throw;
      }
    }
  } // class
} // namespace
