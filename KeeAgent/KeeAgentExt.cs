//
//  KeeAgentExt.cs
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using dlech.SshAgentLib;
using dlech.SshAgentLib.WinForms;
using KeeAgent.Properties;
using KeeAgent.UI;
using KeePass.App;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;
using KeePassLib;
using KeePassLib.Utility;
using KeePass.Util.Spr;

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
    private List<ISshKey> mRemoveKeyList;
    private UIHelper mUIHelper;
    private bool mSaveBeforeCloseQuestionMessageShown = false;
    Dictionary<string, KeyFileInfo> keyFileMap = new Dictionary<string, KeyFileInfo>();

    private const string cPluginNamespace = "KeeAgent";
    private const string cAlwaysConfirmOptionName = cPluginNamespace + ".AlwaysConfirm";
    private const string cShowBalloonOptionName = cPluginNamespace + ".ShowBalloon";
    private const string cNotificationOptionName = cPluginNamespace + ".Notification";
    private const string cLogginEnabledOptionName = cPluginNamespace + ".LoggingEnabled";
    private const string cLogFileNameOptionName = cPluginNamespace + ".LogFileName";
    private const string cAgentModeOptionName = cPluginNamespace + ".AgentMode";
    private const string cUnlockOnActivityOptionName = cPluginNamespace + ".UnlockOnActivity";
    const string keyFilePathSprPlaceholder = @"{KEEAGENT:KEYFILEPATH}";
    const string identFileOptSprPlaceholder = @"{KEEAGENT:IDENTFILEOPT}";

    class KeyFileInfo
    {
      public KeyFileInfo(string path, bool isTemporary)
      {
        Path = path;
        IsTemporary = isTemporary;
      }

      public string Path { get; private set; }
      public bool IsTemporary { get; private set; }
    }

    public Options Options { get; private set; }

    public override bool Initialize(IPluginHost aHost)
    {
      mPluginHost = aHost;
      mUIHelper = new UIHelper(mPluginHost);
      mRemoveKeyList = new List<ISshKey>();
      mDebug = (mPluginHost
          .CommandLineArgs[AppDefs.CommandLineOptions.Debug] != null);

      LoadOptions();

      if (mDebug) Log("Loading KeeAgent...");

      var isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
      var domainSocketPath = 
        Environment.GetEnvironmentVariable (UnixClient.SSH_AUTHSOCKET_ENV_NAME);
      try {
        // TODO check OS - currently only works on Windows
        if (Options.AgentMode != AgentMode.Client) {
          try {
            if (isWindows) {
              var pagent = new PageantAgent();
              pagent.Locked += Pageant_Locked;
              pagent.KeyUsed += Pageant_KeyUsed;
              pagent.KeyListChanged += Pageant_KeyListChanged;
              pagent.MessageReceived += Pageant_MessageReceived;
              // IMPORTANT: if you change this callback, you need to make sure
              // that it does not block the main event loop.
              pagent.ConfirmUserPermissionCallback = Default.ConfirmCallback;
              mAgent = pagent;
            } else {
              if (string.IsNullOrEmpty (domainSocketPath)) {
              var agent = new UnixAgent();
              mAgent = agent;
              }
            }
          } catch (PageantRunningException) {
            if (Options.AgentMode != AgentMode.Auto) {
              throw;
            }
          }
        }
        if (mAgent == null) {
          if (isWindows) {
          mAgent = new PageantClient();
          } else {
            mAgent = new UnixClient();
          }
        }
        mPluginHost.MainWindow.FileOpened += MainForm_FileOpened;
        mPluginHost.MainWindow.FileClosingPost += MainForm_FileClosing;
        mPluginHost.MainWindow.FileClosed += MainForm_FileClosed;
        // load all database that are already opened
        foreach (var database in mPluginHost.MainWindow.DocumentManager.Documents) {
          MainForm_FileOpened(this, new FileOpenedEventArgs(database.Database));
        }
        AddMenuItems();
        GlobalWindowManager.WindowAdded += WindowAddedHandler;
        MessageService.MessageShowing += MessageService_MessageShowing;
        SprEngine.FilterCompile += SprEngine_FilterCompile;
        SprEngine.FilterPlaceholderHints.Add(keyFilePathSprPlaceholder);
        SprEngine.FilterPlaceholderHints.Add(identFileOptSprPlaceholder);
        if (mDebug) Log("Succeeded");
        return true;
      } catch (PageantRunningException) {
        ShowPageantRunningErrorMessage();
      } catch (Exception ex) {
        if (mDebug) Log("Failed");
        MessageService.ShowWarning("KeeAgent failed to load:", ex.Message);
        // TODO: show stack trace here
        Terminate();
      }
      return false;
    }

    public override void Terminate()
    {
      GlobalWindowManager.WindowAdded -= WindowAddedHandler;
      MessageService.MessageShowing -= MessageService_MessageShowing;
      SprEngine.FilterCompile -= SprEngine_FilterCompile;
      SprEngine.FilterPlaceholderHints.Remove(keyFilePathSprPlaceholder);
      SprEngine.FilterPlaceholderHints.Remove(identFileOptSprPlaceholder);
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
      get { return Resources.KeeAgentIcon_png; }
    }

    private void ShowPageantRunningErrorMessage()
    {
      MessageService.ShowWarning(new object[] {
                Translatable.ErrPageantRunning
            });
    }

    /// <summary>
    /// Returns url for automatic updating of plugin
    /// </summary> 
    public override string UpdateUrl
    {
      // TODO - find a way to implement this using something like Castle
      // DynamicProxy or LinFu so that we can compile against KeePass <= 2.17
      get { return "http://updates.lechnology.com/KeePassPlugins"; }
    }

    private void AddMenuItems()
    {
      /* add item to Tools menu */
      mKeeAgentMenuItem = new ToolStripMenuItem();
      mKeeAgentMenuItem.Text = Translatable.KeeAgent;
      mKeeAgentMenuItem.ToolTipText = Translatable.KeeAgentMenuItemToolTip;
      mKeeAgentMenuItem.Image = Resources.KeeAgentIcon_png;
      mKeeAgentMenuItem.Click += manageKeeAgentMenuItem_Click;
      mPluginHost.MainWindow.ToolsMenu.DropDownItems.Add(mKeeAgentMenuItem);

      /* add item to help menu */
      var foundToolstripItem = mPluginHost.MainWindow.MainMenuStrip.Items.Find("m_menuHelp", true);
      if (foundToolstripItem.Length > 0) {
        var helpMenu = foundToolstripItem[0] as ToolStripMenuItem;
        var keeAgentHelpMenuItem = new ToolStripMenuItem();
        keeAgentHelpMenuItem.Text = "KeeAgent Help";
        keeAgentHelpMenuItem.Image = Resources.KeeAgentIcon_png;
        keeAgentHelpMenuItem.Click += (sender, e) =>
        {
          Process.Start("http://lechnology.com/KeeAgent");
        };
        var firstSeparatorIndex = helpMenu.DropDownItems.IndexOfKey("m_menuHelpSep0");
        helpMenu.DropDownItems.Insert(firstSeparatorIndex, keeAgentHelpMenuItem);
      }

      /* add item to Password Entry context menu */
      var foundControl = mPluginHost.MainWindow.Controls.Find("m_lvEntries", true);
      if (foundControl.Length > 0) {
        var entryListView = foundControl[0] as CustomListViewEx;
        if (entryListView != null) {
          var pwEntryContextMenu = entryListView.ContextMenuStrip;
          if (pwEntryContextMenu != null) {
            mKeeAgentPwEntryContextMenuItem = new ToolStripMenuItem();
            mKeeAgentPwEntryContextMenuItem.Text =
              Translatable.AddToKeeAgentContextMenuItem;
            mKeeAgentPwEntryContextMenuItem.Click +=
              mKeeAgentPwEntryContextMenuItem_Clicked;
            mKeeAgentPwEntryContextMenuItem.Image = Resources.KeeAgentIcon_png;
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
      mNotifyIconContextMenuItem.Image = Resources.KeeAgentIcon_png;
      mNotifyIconContextMenuItem.Click += manageKeeAgentMenuItem_Click;
      var notifyIconContextMenu =
        mPluginHost.MainWindow.TrayContextMenu;
      var secondSeparatorIndex =
              notifyIconContextMenu.Items.IndexOfKey("m_ctxTraySep1");
      notifyIconContextMenu.Items.Insert(secondSeparatorIndex,
        mNotifyIconContextMenuItem);
    }

    private void PwEntry_ContextMenu_Opening(object aSender, CancelEventArgs aArgs)
    {
      var selectedEntries = mPluginHost.MainWindow.GetSelectedEntries();
      if (selectedEntries != null) {
        foreach (var entry in selectedEntries) {
          // if any selected entry contains an SSH key then we show the KeeAgent menu item
          if (entry.GetKeeAgentSettings().AllowUseOfSshKey) {
            mKeeAgentPwEntryContextMenuItem.Visible = true;
            var agent = mAgent as Agent;
            if (agent != null && agent.IsLocked)
            {
              mKeeAgentPwEntryContextMenuItem.Enabled = false;
              mKeeAgentPwEntryContextMenuItem.Text = "KeeAgent Locked";
            } else {
              mKeeAgentPwEntryContextMenuItem.Enabled = true;
              mKeeAgentPwEntryContextMenuItem.Text = "Load Entry in KeeAgent";
            }
            return;
          }
        }
      }
      mKeeAgentPwEntryContextMenuItem.Visible = false;
    }

    private void mKeeAgentPwEntryContextMenuItem_Clicked(object sender, EventArgs e)
    {
      foreach (var entry in mPluginHost.MainWindow.GetSelectedEntries()) {
        // if any selected entry contains an SSH key then we show the KeeAgent menu item
        var settings = entry.GetKeeAgentSettings();
        if (settings.AllowUseOfSshKey) {
          try {
            var constraints = new List<Agent.KeyConstraint>();
            if (!(mAgent is PageantClient)) {
              if (Control.ModifierKeys.HasFlag(Keys.Control)) {
                var dialog = new ConstraintsInputDialog(settings.UseConfirmConstraintWhenAdding);
                dialog.ShowDialog();
                if (dialog.DialogResult == DialogResult.OK) {
                  if (dialog.ConfirmConstraintChecked) {
                    constraints.addConfirmConstraint();
                  }
                  if (dialog.LifetimeConstraintChecked) {
                    constraints.addLifetimeConstraint(dialog.LifetimeDuration);
                  }
                }
              } else {
                if (settings.UseConfirmConstraintWhenAdding) {
                  constraints.addConfirmConstraint();
                }
              }
            }
            AddEntry(entry, constraints);
          } catch (Exception) {
            // AddEntry should have already shown error message
          }
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

    internal void SaveGlobalOptions()
    {
      var config = mPluginHost.CustomConfig;
      config.SetBool(cAlwaysConfirmOptionName, Options.AlwaysConfirm);
      config.SetString(cShowBalloonOptionName, Options.ShowBalloon.ToString());
      config.SetBool(cLogginEnabledOptionName, Options.LoggingEnabled);
      config.SetString(cLogFileNameOptionName, Options.LogFileName);
      config.SetString(cAgentModeOptionName, Options.AgentMode.ToString());
      config.SetBool (cUnlockOnActivityOptionName, Options.UnlockOnActivity);
    }

    private void LoadOptions()
    {
      Options = new Options();
      var config = mPluginHost.CustomConfig;

      Options.AlwaysConfirm = config.GetBool(cAlwaysConfirmOptionName, false);
      Options.ShowBalloon = config.GetBool(cShowBalloonOptionName, true);
      Options.LoggingEnabled = config.GetBool(cLogginEnabledOptionName, false);
      Options.UnlockOnActivity = config.GetBool (cUnlockOnActivityOptionName, true);

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
            Options.AlwaysConfirm = true;
            break;
          case NotificationOptions.Never:
            Options.ShowBalloon = false;
            break;
        }
        config.SetString(cNotificationOptionName, string.Empty);
      }
    }

    /// <summary>
    /// Modifies various Forms in KeePass
    /// </summary>
    ///<remarks>
    /// Kudos to the luckyrat for figuring out how to to this in KeePassRPC
    /// (KeeFox) and open-sourcing the code so I could copy/learn from it.
    ///</remarks>
    private void WindowAddedHandler(object aSender,
                                    GwmWindowEventArgs aEventArgs)
    {
      /* Add KeeAgent tab to PwEntryForm dialog */
      var pwEntryForm = aEventArgs.Form as PwEntryForm;
      if (pwEntryForm != null) {
        var optionsPanel = new EntryPanel();
        pwEntryForm.Shown +=
          delegate(object sender, EventArgs args)
          {
            pwEntryForm.AddTab(optionsPanel);
          };
        var foundControls = pwEntryForm.Controls.Find("m_btnOK", true);
        var okButton = foundControls[0] as Button;
        okButton.GotFocus += (sender, args) =>
        {
          if (optionsPanel.CurrentSettings != optionsPanel.IntialSettings) {
            pwEntryForm.EntryBinaries.SetKeeAgentSettings(optionsPanel.CurrentSettings);
          }
        };
        pwEntryForm.EntrySaving += PwEntryForm_EntrySaving;
        pwEntryForm.FormClosing += PwEntryForm_FormClosing;
      }

      /* Add KeeAgent tab to Database Settings dialog */
      var databaseSettingForm = aEventArgs.Form as DatabaseSettingsForm;
      if (databaseSettingForm != null) {
        databaseSettingForm.Shown +=
          delegate(object sender, EventArgs args)
          {
            var dbSettingsPanel =
              new DatabaseSettingsPanel(mPluginHost.MainWindow.ActiveDatabase);
            databaseSettingForm.AddTab(dbSettingsPanel);
          };
      }

      /* Add KeeAgent tab to Options dialog */
      var optionsForm = aEventArgs.Form as OptionsForm;
      if (optionsForm != null) {
        optionsForm.Shown +=
          delegate(object sender, EventArgs args)
          {
            var optionsPanel = new OptionsPanel(this);
            optionsForm.AddTab(optionsPanel);
          };
        optionsForm.FormClosed += OptionsForm_FormClosed;
      }
    }

    private void PwEntryForm_EntrySaving(object aSender,
      CancellableOperationEventArgs aEventArgs)
    {
      /* Get reference to new settings */

      var entryForm = aSender as PwEntryForm;
      if (entryForm != null && (entryForm.DialogResult == DialogResult.OK ||
        mSaveBeforeCloseQuestionMessageShown)) {
        var foundControls = entryForm.Controls.Find("EntryPanel", true);
        if (foundControls.Length != 1) {
          return;
        }
        var entryPanel = foundControls[0] as EntryPanel;
        if (entryPanel == null) {
          return;
        }
        var settings = entryPanel.CurrentSettings;
        if (settings == null) {
          return;
        }

        /* validate KeeAgent Entry Settings */

        if (settings.AllowUseOfSshKey) {
          string errorMessage = null;
          switch (settings.Location.SelectedType) {
            case EntrySettings.LocationType.Attachment:
              if (string.IsNullOrWhiteSpace(settings.Location.AttachmentName)) {
                errorMessage = "Must specify attachment";
              } else if (entryForm.EntryBinaries
                         .Get(settings.Location.AttachmentName) == null) {
                errorMessage = "Attachment does not exist";
              }
              break;
            case EntrySettings.LocationType.File:
              if (string.IsNullOrWhiteSpace(settings.Location.FileName)) {
                errorMessage = "Must specify file name";
              } else if (!File.Exists(settings.Location.FileName)) {
                errorMessage = "File does not exist";
              }
              break;
            default:
              errorMessage = "Must select attachment or file";
              break;
          }

          /* if there was a problem with a KeeAgent settings, activate the 
           * KeeAgent tab, show error message and cancel the save */

          if (errorMessage != null) {
            // Activate KeeAgent tab
            var keeAgentTab = entryPanel.Parent as TabPage;
            if (keeAgentTab != null) {
              foundControls = entryForm.Controls.Find("m_tabMain", true);
              if (foundControls.Length == 1) {
                var tabControl = foundControls[0] as TabControl;
                if (tabControl != null) {
                  tabControl.SelectTab(keeAgentTab);
                }
              }
            }
            MessageService.ShowWarning(errorMessage);
            aEventArgs.Cancel = true;
          }
        }
      }
      mSaveBeforeCloseQuestionMessageShown = false;
    }

    private void PwEntryForm_FormClosing(object aSender,
      FormClosingEventArgs aEventArgs)
    {
      mSaveBeforeCloseQuestionMessageShown = false;
    }

    private void OptionsForm_FormClosed(object aSender,
      FormClosedEventArgs aEventArgs)
    {
      var optionsForm = aSender as OptionsForm;
      if (optionsForm != null && optionsForm.DialogResult == DialogResult.OK) {
        SaveGlobalOptions();
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
      if (Options.AlwaysConfirm &&
          aEventArgs.Action == Agent.KeyListChangeEventAction.Add &&
          !aEventArgs.Key.HasConstraint(
            Agent.KeyConstraintType.SSH_AGENT_CONSTRAIN_CONFIRM)) {

        var constraint = new Agent.KeyConstraint();
        constraint.Type = Agent.KeyConstraintType.SSH_AGENT_CONSTRAIN_CONFIRM;
        aEventArgs.Key.AddConstraint(constraint);
      }      
      if (aEventArgs.Action == Agent.KeyListChangeEventAction.Remove) {
        var fingerprint = aEventArgs.Key.GetMD5Fingerprint().ToHexString();
        if (keyFileMap.ContainsKey(fingerprint) && keyFileMap[fingerprint].IsTemporary) {
          try {
            File.Delete(keyFileMap[fingerprint].Path);
            keyFileMap.Remove(fingerprint);
          } catch (Exception ex) {
            Debug.Fail(ex.Message, ex.StackTrace);
          }
        }
      }
    }

    private void Pageant_MessageReceived(object aSender,
      Agent.MessageReceivedEventArgs aEventArgs)
    {
      var mainWindow = mPluginHost.MainWindow;

      var thread = new Thread(
        delegate()
        {
          mainWindow.Invoke(
            (MethodInvoker)delegate()
          {
            // don't do anything - we are just seeing if the thread is blocked
          });
        });
      thread.Name = "Check";
      thread.Start();
      // only try to unlock databases if main thread is not blocked
      if (thread.Join(1000)) {
        mainWindow.Invoke((MethodInvoker)delegate()
        {
          if (Options.UnlockOnActivity) {
            foreach (var document in mainWindow.DocumentManager.Documents) {
            if (mainWindow.IsFileLocked(document)) {
                mainWindow.OpenDatabase(document.LockedIoc, null, false);
              }
            }
          }
        });
      } else {
        thread.Abort();
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

    private void MainForm_FileOpened(object sender, FileOpenedEventArgs e)
    {
      try {
        if (e.Database.RootGroup == null) {
          return;
        }
        var exitFor = false;
        foreach (var entry in e.Database.RootGroup.GetEntries(true)) {
          if (exitFor) {
            break;
          }
          if (entry.Expires && entry.ExpiryTime <= DateTime.Now
            && !e.Database.GetKeeAgentSettings().AllowAutoLoadExpiredEntryKey) {
            continue;
          }
          var settings = entry.GetKeeAgentSettings();
          if (settings.AllowUseOfSshKey && settings.AddAtDatabaseOpen) {
            try {
              AddEntry(entry, null);
            } catch (Exception) {
              if (MessageService.AskYesNo ("Do you want to attempt to load additional keys?")) {
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
            var settings = entry.GetKeeAgentSettings();
            if (settings.AllowUseOfSshKey && settings.RemoveAtDatabaseClose) {
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

    private void MessageService_MessageShowing(object aSender,
                                               MessageServiceEventArgs aEventArgs)
    {
      if (aEventArgs.Title == PwDefs.ShortProductName &&
        aEventArgs.Text == KPRes.SaveBeforeCloseQuestion) {
        mSaveBeforeCloseQuestionMessageShown = true;
      }
    }

    private void SprEngine_FilterCompile(object sender, SprEventArgs e)
    {
      if (!e.Context.Flags.HasFlag(SprCompileFlags.ExtNonActive))
        return;
      var path = string.Empty;
      try {
        using (var key = e.Context.Entry.GetSshKey()) {
          var fingerprint = key.GetMD5Fingerprint().ToHexString();
          if (keyFileMap.ContainsKey(fingerprint))
            path = keyFileMap[fingerprint].Path;
        }
      } catch (Exception) { }
      e.Text = StrUtil.ReplaceCaseInsensitive(e.Text,
        keyFilePathSprPlaceholder, path);
      e.Text = StrUtil.ReplaceCaseInsensitive(e.Text,
        identFileOptSprPlaceholder, string.Format("-i \"{0}\"", path));
    }

    public ISshKey AddEntry(PwEntry entry,
                            ICollection<Agent.KeyConstraint> constraints)
    {
      var settings = entry.GetKeeAgentSettings();
      try {
        var key = entry.GetSshKey();

        if (mAgent is PageantClient) {
          // Pageant errors if you try to add a key that is already loaded
          // so try to remove the key first so that it behaves like other agents
          try {
            mAgent.RemoveKey(key);
          } catch (Exception) {
            // ignore failure
          }
        } else {
          // also, Pageant does not support constraints
          if (constraints != null) {
            foreach (var constraint in constraints) {
              key.AddConstraint(constraint);
            }
          } else {
            if (settings.UseConfirmConstraintWhenAdding) {
              key.addConfirmConstraint();
            }
          }
          if (Options.AlwaysConfirm &&
              !key.HasConstraint(Agent.KeyConstraintType.SSH_AGENT_CONSTRAIN_CONFIRM))
          {
            key.addConfirmConstraint();
          }
        }
        mAgent.AddKey(key);
        if (settings.Location.SelectedType == EntrySettings.LocationType.Attachment
          && settings.Location.SaveAttachmentToTempFile)
        {
          try {
            var data = entry.Binaries.Get(settings.Location.AttachmentName).ReadData();
            var tempPath = Path.Combine(UrlUtil.GetTempPath(), "KeeAgent");
            if (!Directory.Exists(tempPath))
              Directory.CreateDirectory(tempPath);
            var fileName = Path.Combine(tempPath, settings.Location.AttachmentName);
            File.WriteAllBytes(fileName, data);
            keyFileMap[key.GetMD5Fingerprint().ToHexString()] = new KeyFileInfo(fileName, true);
          } catch (Exception ex) {
            Debug.Fail(ex.Message, ex.StackTrace);
          }
        }
        if (settings.Location.SelectedType == EntrySettings.LocationType.File) {
          keyFileMap[key.GetMD5Fingerprint().ToHexString()] =
            new KeyFileInfo(settings.Location.FileName, false);
        }
        return key;
      } catch (Exception ex) {
        if (ex is NoAttachmentException) {
          MessageService.ShowWarning(new string[] {
             "KeeAgent Error - No attachment specified in KeePass entry"
           });
        } else if (ex is FileNotFoundException || ex is DirectoryNotFoundException) {
          MessageService.ShowWarning(new string[] {
            "KeeAgent Error - Could not find file",
            settings.Location.FileName
           });
        } else if (ex is KeyFormatterException || ex is PpkFormatterException) {
          MessageService.ShowWarning(new string[] {
            "KeeAgent Error - Could not load file",
            settings.Location.FileName,
            "Possible causes:",
            "- Passphrase was entered incorrectly",
            "- File is corrupt or has been tampered"
           });
        } else if (ex is AgentFailureException) {
          MessageService.ShowWarning(new string[] {
            "KeeAgent Error - Agent Failure",
            "Possible causes:",
            "- Key is already loaded in agent",
            "- Agent is locked"
          });
        } else {
          MessageService.ShowWarning(new string[] {
            "KeeAgent Error - Unexpected error",
            ex.ToString()
          });
          Debug.Fail(ex.ToString());
        }
        throw;
      }
    }
  } // class
} // namespace
