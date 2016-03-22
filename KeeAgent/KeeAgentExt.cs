//
//  KeeAgentExt.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2012-2015  David Lechner <david@lechnology.com>
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
using System.Linq;
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
using KeePass.Util.Spr;
using KeePassLib;
using KeePassLib.Utility;

namespace KeeAgent
{
  public sealed partial class KeeAgentExt : Plugin
  {
    internal IPluginHost pluginHost;
    internal bool debug;
    internal IAgent agent;

    ToolStripMenuItem keeAgentMenuItem;
    ToolStripMenuItem pwEntryContextMenuLoadKeyMenuItem;
    ToolStripMenuItem pwEntryContextMenuLoadKeyOpenUrlMenuItem;
    ToolStripMenuItem notifyIconContextMenuItem;
    ToolStripMenuItem pwEntryContextMenuUrlOpenMenuItem;
    List<ISshKey> removeKeyList;
    UIHelper uiHelper;
    bool saveBeforeCloseQuestionMessageShown = false;
    Dictionary<string, KeyFileInfo> keyFileMap = new Dictionary<string, KeyFileInfo>();
    KeeAgentColumnProvider columnProvider;

    const string pluginNamespace = "KeeAgent";
    const string alwaysConfirmOptionName = pluginNamespace + ".AlwaysConfirm";
    const string showBalloonOptionName = pluginNamespace + ".ShowBalloon";
    const string notificationOptionName = pluginNamespace + ".Notification";
    const string logginEnabledOptionName = pluginNamespace + ".LoggingEnabled";
    const string logFileNameOptionName = pluginNamespace + ".LogFileName";
    const string agentModeOptionName = pluginNamespace + ".AgentMode";
    const string unlockOnActivityOptionName = pluginNamespace + ".UnlockOnActivity";
    const string useCygwinSocketOptionName = pluginNamespace + ".UseCygwinSocket";
    const string cygwinSocketPathOptionName = pluginNamespace + ".CygwinSocketPath";
    const string useMsysSocketOptionName = pluginNamespace + ".UseMsysSocket";
    const string msysSocketPathOptionName = pluginNamespace + ".MsysSocketPath";
    const string unixSocketPathOptionName = pluginNamespace + ".UnixSocketPath";
    const string userPicksKeyOnRequestIdentitiesOptionName =
      pluginNamespace + ".UserPicksKeyOnRequestIdentities";
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

    public override bool Initialize(IPluginHost host)
    {
      pluginHost = host;
      uiHelper = new UIHelper(pluginHost);
      removeKeyList = new List<ISshKey>();
      debug = (pluginHost
          .CommandLineArgs[AppDefs.CommandLineOptions.Debug] != null);

      LoadOptions();

      if (debug) Log("Loading KeeAgent...");

      var isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
      var domainSocketPath = 
        Environment.GetEnvironmentVariable (UnixClient.SshAuthSockName);
      try {
        if (Options.AgentMode != AgentMode.Client) {
          if (isWindows) {
            // In windows, try to start an agent. If Pageant is running, we will
            // get an exception.
            try {
              var pagent = new PageantAgent();
              pagent.Locked += PageantAgent_Locked;
              pagent.KeyUsed += PageantAgent_KeyUsed;
              pagent.KeyAdded += PageantAgent_KeyAdded;
              pagent.KeyRemoved += PageantAgent_KeyRemoved;
              pagent.MessageReceived += PageantAgent_MessageReceived;
              // IMPORTANT: if you change either of these callbacks, you need
              // to make sure that they do not block the main event loop.
              pagent.FilterKeyListCallback = FilterKeyList;
              pagent.ConfirmUserPermissionCallback = Default.ConfirmCallback;
              agent = pagent;
              if (Options.UseCygwinSocket) {
                StartCygwinSocket();
              }
              if (Options.UseMsysSocket) {
                StartMsysSocket();
              }
            } catch (PageantRunningException) {
              if (Options.AgentMode != AgentMode.Auto) {
                throw;
              }
            }
          } else {
            // In Unix, we only try to start an agent if Agent mode was explicitly
            // selected or there is no agent running (indicated by environment variable).
            if (Options.AgentMode == AgentMode.Server || string.IsNullOrWhiteSpace (domainSocketPath)) {
              var unixAgent = new UnixAgent();
              unixAgent.Locked += PageantAgent_Locked;
              unixAgent.KeyUsed += PageantAgent_KeyUsed;
              unixAgent.KeyAdded += PageantAgent_KeyAdded;
              unixAgent.KeyRemoved += PageantAgent_KeyRemoved;
              unixAgent.MessageReceived += PageantAgent_MessageReceived;
              // IMPORTANT: if you change either of these callbacks, you need
              // to make sure that they do not block the main event loop.
              unixAgent.FilterKeyListCallback = FilterKeyList;
              unixAgent.ConfirmUserPermissionCallback = Default.ConfirmCallback;
              agent = unixAgent;
              try {
                var socketPath = Options.UnixSocketPath;
                if (socketPath.StartsWith("~/", StringComparison.Ordinal)) {
                  socketPath = Path.Combine("%HOME%", socketPath.Substring (2));
                }
                socketPath = Environment.ExpandEnvironmentVariables(socketPath);
                unixAgent.StartUnixSocket (socketPath);
              } catch (ArgumentNullException) {
                var autoModeMessage = Options.AgentMode == AgentMode.Auto
                  ? " to use KeeAgent in Agent mode or enable an external SSH agent in your " +
                  "desktop session manager to use KeeAgent in Client mode."
                  : ".";
                MessageService.ShowWarning("KeeAgent: No path specified for Agent socket file.",
                  "Please enter a file in the KeeAgent options (Tools > Options... > KeeAgent tab)" +
                  autoModeMessage);
              } catch (Exception ex) {
                MessageService.ShowWarning(ex.Message);
              }
            }
          }
        }
        if (agent == null) {
          if (isWindows) {
            agent = new PageantClient();
          } else {
            agent = new UnixClient();
          }
        }
        pluginHost.MainWindow.FileOpened += MainForm_FileOpened;
        pluginHost.MainWindow.FileClosingPost += MainForm_FileClosing;
        pluginHost.MainWindow.FileClosed += MainForm_FileClosed;
        // load all database that are already opened
        foreach (var database in pluginHost.MainWindow.DocumentManager.Documents) {
          MainForm_FileOpened(this, new FileOpenedEventArgs(database.Database));
        }
        AddMenuItems();
        GlobalWindowManager.WindowAdded += WindowAddedHandler;
        MessageService.MessageShowing += MessageService_MessageShowing;
        columnProvider = new KeeAgentColumnProvider(this);
        host.ColumnProviderPool.Add(columnProvider);
        SprEngine.FilterCompile += SprEngine_FilterCompile;
        SprEngine.FilterPlaceholderHints.Add(keyFilePathSprPlaceholder);
        SprEngine.FilterPlaceholderHints.Add(identFileOptSprPlaceholder);
        return true;
      } catch (PageantRunningException) {
        ShowPageantRunningErrorMessage();
      } catch (Exception ex) {
        MessageService.ShowWarning("KeeAgent failed to load:", ex.Message);
      }
      Terminate();
      return false;
    }

    public override void Terminate()
    {
      GlobalWindowManager.WindowAdded -= WindowAddedHandler;
      MessageService.MessageShowing -= MessageService_MessageShowing;
      SprEngine.FilterCompile -= SprEngine_FilterCompile;
      SprEngine.FilterPlaceholderHints.Remove(keyFilePathSprPlaceholder);
      SprEngine.FilterPlaceholderHints.Remove(identFileOptSprPlaceholder);
      if (columnProvider != null) {
        pluginHost.ColumnProviderPool.Remove(columnProvider);
        columnProvider.Dispose();
        columnProvider = null;
      }
      if (debug) Log("Terminating KeeAgent");
      var agentModeAgent = agent as Agent;
      if (agentModeAgent != null) {
        // need to shutdown agent or app won't exit
        agentModeAgent.Dispose();
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
      get { return "http://updates.lechnology.com/KeePassPlugins"; }
    }

    public void StartCygwinSocket()
    {
      var pagent = agent as PageantAgent;
      if (pagent == null)
        return;
      try {
        pagent.StopCygwinSocket();
        pagent.StartCygwinSocket(Environment.ExpandEnvironmentVariables(
          Options.CygwinSocketPath));
      } catch (Exception ex) {
        MessageService.ShowWarning("Failed to start Cygwin socket:",
          ex.Message);
        // TODO: show better explanation of common errors.
      }
    }

    public void StopCygwinSocket()
    {
      var pagent = agent as PageantAgent;
      if (pagent == null)
        return;
      pagent.StopCygwinSocket();
    }

    public void StartMsysSocket()
    {
      var pagent = agent as PageantAgent;
      if (pagent == null)
        return;
      try {
        pagent.StopMsysSocket();
        pagent.StartMsysSocket(Environment.ExpandEnvironmentVariables(
          Options.MsysSocketPath));
      } catch (Exception ex) {
        MessageService.ShowWarning("Failed to start MSYS socket:",
          ex.Message);
        // TODO: show better explanation of common errors.
      }
    }

    public void StopMsysSocket()
    {
      var pagent = agent as PageantAgent;
      if (pagent == null)
        return;
      pagent.StopMsysSocket();
    }

    public void StartUnixSocket()
    {
      var unixAgent = agent as UnixAgent;
      if (unixAgent == null)
        return;
      try {
        unixAgent.StopUnixSocket();
        var socketPath = Options.UnixSocketPath;
        if (socketPath.StartsWith("~/", StringComparison.Ordinal)) {
          socketPath = Path.Combine("%HOME%", socketPath.Substring (2));
        }
        unixAgent.StartUnixSocket(Environment.ExpandEnvironmentVariables(
          socketPath));
      } catch (Exception ex) {
        MessageService.ShowWarning("Failed to start Unix socket:",
          ex.Message);
        // TODO: show better explanation of common errors.
      }
    }

    public void StopUnixSocket()
    {
      var unixAgent = agent as UnixAgent;
      if (unixAgent == null)
        return;
      unixAgent.StopUnixSocket();
    }

    private void AddMenuItems()
    {
      /* add item to Tools menu */
      keeAgentMenuItem = new ToolStripMenuItem();
      keeAgentMenuItem.Text = Translatable.KeeAgent;
      keeAgentMenuItem.ToolTipText = Translatable.KeeAgentMenuItemToolTip;
      keeAgentMenuItem.Image = Resources.KeeAgentIcon_png;
      keeAgentMenuItem.Click += manageKeeAgentMenuItem_Click;
      pluginHost.MainWindow.ToolsMenu.DropDownItems.Add(keeAgentMenuItem);

      /* add item to help menu */
      var foundToolstripItem = pluginHost.MainWindow.MainMenuStrip.Items.Find("m_menuHelp", true);
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
      var foundControl = pluginHost.MainWindow.Controls.Find("m_lvEntries", true);
      if (foundControl.Length > 0) {
        var entryListView = foundControl[0] as CustomListViewEx;
        if (entryListView != null) {
          var pwEntryContextMenu = entryListView.ContextMenuStrip;
          if (pwEntryContextMenu != null) {
            pwEntryContextMenuLoadKeyMenuItem = new ToolStripMenuItem() {
              Text = Translatable.LoadKeyContextMenuItem,
              Image = Resources.KeeAgentIcon_png,
              ShortcutKeys = Keys.Control | Keys.L,
            };
            pwEntryContextMenuLoadKeyMenuItem.Click +=
              PwEntryContextMenuLoadKeyItem_Clicked;
            pwEntryContextMenuLoadKeyOpenUrlMenuItem = new ToolStripMenuItem()
            {
              Text = Translatable.LoadKeyContextMenuItem,
              Image = Resources.KeeAgentIcon_png,
              ShortcutKeys = Keys.Control | Keys.Shift | Keys.L,
              Visible = false
            };
            pwEntryContextMenuLoadKeyOpenUrlMenuItem.Click +=
              PwEntryContextMenuLoadKeyItem_Clicked;
            var firstSeparatorIndex =
              pwEntryContextMenu.Items.IndexOfKey("m_ctxEntrySep0");
            pwEntryContextMenu.Items.Insert(firstSeparatorIndex,
              pwEntryContextMenuLoadKeyMenuItem);
            pwEntryContextMenu.Items.Insert(firstSeparatorIndex,
              pwEntryContextMenuLoadKeyOpenUrlMenuItem);
            pwEntryContextMenu.Opening += PwEntry_ContextMenu_Opening;
            pwEntryContextMenuUrlOpenMenuItem =
              pwEntryContextMenu.Items.Find("m_ctxEntryOpenUrl", true).SingleOrDefault() as ToolStripMenuItem;
          }
        }
      }

      /* add item to notification icon context menu */
      notifyIconContextMenuItem = new ToolStripMenuItem();
      notifyIconContextMenuItem.Text = Translatable.KeeAgent;
      notifyIconContextMenuItem.ToolTipText = Translatable.KeeAgentMenuItemToolTip;
      notifyIconContextMenuItem.Image = Resources.KeeAgentIcon_png;
      notifyIconContextMenuItem.Click += manageKeeAgentMenuItem_Click;
      var notifyIconContextMenu =
        pluginHost.MainWindow.TrayContextMenu;
      var secondSeparatorIndex =
              notifyIconContextMenu.Items.IndexOfKey("m_ctxTraySep1");
      notifyIconContextMenu.Items.Insert(secondSeparatorIndex,
        notifyIconContextMenuItem);
    }

    private void PwEntry_ContextMenu_Opening(object sender, CancelEventArgs e)
    {
      var selectedEntries = pluginHost.MainWindow.GetSelectedEntries();
      if (selectedEntries != null) {
        foreach (var entry in selectedEntries) {
          // if any selected entry contains an SSH key then we show the KeeAgent menu item
          if (entry.GetKeeAgentSettings().AllowUseOfSshKey) {
            pwEntryContextMenuLoadKeyMenuItem.Visible = true;
            var agentModeAgent = this.agent as Agent;
            if (agentModeAgent != null && agentModeAgent.IsLocked)
            {
              pwEntryContextMenuLoadKeyMenuItem.Enabled = false;
              pwEntryContextMenuLoadKeyMenuItem.Text = Translatable.StatusLocked;
            } else {
              pwEntryContextMenuLoadKeyMenuItem.Enabled = true;
              pwEntryContextMenuLoadKeyMenuItem.Text = Translatable.LoadKeyContextMenuItem;
            }
            return;
          }
        }
      }
      pwEntryContextMenuLoadKeyMenuItem.Visible = false;
    }

    private void PwEntryContextMenuLoadKeyItem_Clicked(object sender, EventArgs e)
    {
      foreach (var entry in pluginHost.MainWindow.GetSelectedEntries()) {
        // if any selected entry contains an SSH key then we show the KeeAgent menu item
        var settings = entry.GetKeeAgentSettings();
        if (settings.AllowUseOfSshKey) {
          try {
            AddEntry(entry, null);
            if (Control.ModifierKeys.HasFlag(Keys.Shift)
              && pwEntryContextMenuUrlOpenMenuItem != null)
            {
              pwEntryContextMenuUrlOpenMenuItem.PerformClick();  
            }
          } catch (Exception) {
            // AddEntry should have already shown error message
          }
        }
      }
    }

    private void RemoveMenuItems()
    {
      if (pluginHost != null && pluginHost.MainWindow != null &&
          keeAgentMenuItem != null) {

        /* get Tools menu */
        ToolStripMenuItem toolsMenu = pluginHost.MainWindow.ToolsMenu;
        /* remove items from tools menu */
        toolsMenu.DropDownItems.Remove(keeAgentMenuItem);
      }
    }

    private void manageKeeAgentMenuItem_Click(object aSource, EventArgs aEvent)
    {
      ShowManageDialog();
    }

    private void ShowManageDialog()
    {
      using (ManageDialog dialog = new ManageDialog(this)) {
        dialog.ShowDialog(pluginHost.MainWindow);
      }
    }

    internal void SaveGlobalOptions()
    {
      var config = pluginHost.CustomConfig;
      config.SetBool(alwaysConfirmOptionName, Options.AlwaysConfirm);
      config.SetString(showBalloonOptionName, Options.ShowBalloon.ToString());
      config.SetBool(logginEnabledOptionName, Options.LoggingEnabled);
      config.SetString(logFileNameOptionName, Options.LogFileName);
      config.SetString(agentModeOptionName, Options.AgentMode.ToString());
      config.SetBool(unlockOnActivityOptionName, Options.UnlockOnActivity);
      config.SetBool(useCygwinSocketOptionName, Options.UseCygwinSocket);
      config.SetString(cygwinSocketPathOptionName, Options.CygwinSocketPath);
      config.SetBool(useMsysSocketOptionName, Options.UseMsysSocket );
      config.SetString(msysSocketPathOptionName, Options.MsysSocketPath);
      config.SetString(unixSocketPathOptionName, Options.UnixSocketPath);
      config.SetBool(userPicksKeyOnRequestIdentitiesOptionName,
        Options.UserPicksKeyOnRequestIdentities);
    }

    private void LoadOptions()
    {
      Options = new Options();
      var config = pluginHost.CustomConfig;

      Options.AlwaysConfirm = config.GetBool(alwaysConfirmOptionName, false);
      Options.ShowBalloon = config.GetBool(showBalloonOptionName, true);
      Options.LoggingEnabled = config.GetBool(logginEnabledOptionName, false);
      Options.UnlockOnActivity = config.GetBool(unlockOnActivityOptionName, true);
      Options.UseCygwinSocket = config.GetBool(useCygwinSocketOptionName, false);
      Options.CygwinSocketPath = config.GetString(cygwinSocketPathOptionName);
      Options.UseMsysSocket = config.GetBool(useMsysSocketOptionName, false);
      Options.MsysSocketPath = config.GetString(msysSocketPathOptionName);
      Options.UnixSocketPath = config.GetString(unixSocketPathOptionName);
      Options.UserPicksKeyOnRequestIdentities =
        config.GetBool(userPicksKeyOnRequestIdentitiesOptionName, false);

      string defaultLogFileNameValue = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
          "KeeAgent.log");
      string configFileLogFileNameValue =
          config.GetString(logFileNameOptionName);
      if (string.IsNullOrEmpty(configFileLogFileNameValue)) {
        Options.LogFileName = defaultLogFileNameValue;
      } else {
        Options.LogFileName = configFileLogFileNameValue;
      }

      AgentMode configAgentMode;
      if (Enum.TryParse<AgentMode>(config.GetString(agentModeOptionName),
        out configAgentMode)) {
        Options.AgentMode = configAgentMode;
      } else {
        Options.AgentMode = AgentMode.Auto;
      }

      /* the Notification option is obsolete, so we read it and then clear it. */
      NotificationOptions configFileNotificationValue;
      if (Enum.TryParse<NotificationOptions>(config
        .GetString(notificationOptionName), out configFileNotificationValue)) {

        switch (configFileNotificationValue) {
          case NotificationOptions.AlwaysAsk:
          case NotificationOptions.AskOnce:
            Options.AlwaysConfirm = true;
            break;
          case NotificationOptions.Never:
            Options.ShowBalloon = false;
            break;
        }
        config.SetString(notificationOptionName, string.Empty);
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
        var optionsPanel = new EntryPanel(this);
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
              new DatabaseSettingsPanel(pluginHost.MainWindow.ActiveDatabase);
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
        saveBeforeCloseQuestionMessageShown)) {
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
      saveBeforeCloseQuestionMessageShown = false;
    }

    private void PwEntryForm_FormClosing(object aSender,
      FormClosingEventArgs aEventArgs)
    {
      saveBeforeCloseQuestionMessageShown = false;
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

    private void PageantAgent_Locked(object aSender, Agent.LockEventArgs aEventArgs)
    {
      if (Options.ShowBalloon) {
        string notifyText;
        if (aEventArgs.IsLocked) {
          notifyText = Translatable.NotifyLocked;
        } else {
          notifyText = Translatable.NotifyUnlocked;
        }
        uiHelper.ShowBalloonNotification(notifyText);
      }
    }

    private void PageantAgent_KeyAdded(object sender,SshKeyEventArgs e)
    {
      if (Options.AlwaysConfirm && !e.Key.HasConstraint(
            Agent.KeyConstraintType.SSH_AGENT_CONSTRAIN_CONFIRM))
      {
        var constraint = new Agent.KeyConstraint();
        constraint.Type = Agent.KeyConstraintType.SSH_AGENT_CONSTRAIN_CONFIRM;
        e.Key.AddConstraint(constraint);
      }
    }

    private void PageantAgent_KeyRemoved(object aSender, SshKeyEventArgs aEventArgs)
    {
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

    private void PageantAgent_MessageReceived(object aSender,
      Agent.MessageReceivedEventArgs aEventArgs)
    {
      var mainWindow = pluginHost.MainWindow;

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

    private void PageantAgent_KeyUsed(object sender, Agent.KeyUsedEventArgs e)
    {
      if (Options.ShowBalloon) {
          var appText = e.OtherProcess == null
              ? Translatable.NotifyKeyFetchedUnknownApplication
              : string.Format("{0} ({1})", e.OtherProcess.MainWindowTitle, e.OtherProcess.ProcessName);
        string notifyText = string.Format(Translatable.NotifyKeyFetched,
          e.Key.Comment, appText);
        uiHelper.ShowBalloonNotification(notifyText);
      }
    }

    private void MainForm_FileOpened(object sender, FileOpenedEventArgs e)
    {
      try {
        var agentModeAgent = agent as Agent;
        if (agentModeAgent != null && agentModeAgent.IsLocked) {
            // don't do anything if agent is locked
            return;
        }
        if (e.Database.RootGroup == null) {
          return;
        }
        var exitFor = false;
        foreach (var entry in e.Database.RootGroup.GetEntries(true)) {
          if (exitFor) {
            break;
          }
          if (e.Database.RecycleBinEnabled) {
            var recylceBin = e.Database.RootGroup.FindGroup(e.Database.RecycleBinUuid, true);
            if (recylceBin != null && entry.IsContainedIn(recylceBin)) {
              continue;
            }
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
              if (!MessageService.AskYesNo("Do you want to attempt to load additional keys?")) {
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
        removeKeyList.Clear();
        var allKeys = agent.GetAllKeys();
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
              removeKeyList.Add(removeKey);
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
        foreach (var key in removeKeyList) {
          agent.RemoveKey(key);
        }
        removeKeyList.Clear();
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
        saveBeforeCloseQuestionMessageShown = true;
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
        string db_name = "<Unknown database>";
        try {
          var database = pluginHost.MainWindow.DocumentManager.GetOpenDatabases()
            .Where((db) => db.RootGroup.FindEntry(entry.Uuid, true) != null).Single();
          db_name = database.Name;
        } catch (Exception) {
          Debug.Fail("Duplicate UUIDs?");
        }
        key.Source = string.Format("{0}: {1}", db_name, entry.GetFullPath());
        if (agent is PageantClient) {
          // Pageant errors if you try to add a key that is already loaded
          // so try to remove the key first so that it behaves like other agents
          try {
            agent.RemoveKey(key);
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
            if (settings.UseLifetimeConstraintWhenAdding) {
              key.addLifetimeConstraint(settings.LifetimeConstraintDuration);
            }
          }
          if (Options.AlwaysConfirm &&
              !key.HasConstraint(Agent.KeyConstraintType.SSH_AGENT_CONSTRAIN_CONFIRM))
          {
            key.addConfirmConstraint();
          }
        }
        agent.AddKey(key);
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
        var firstLine = string.Format("KeeAgent: Error while loading key from entry '{0}'",
          entry.GetFullPath());
        if (ex is NoAttachmentException) {
          MessageService.ShowWarning(new string[] {
            firstLine,
             "No attachment specified in KeePass entry"
           });
        } else if (ex is FileNotFoundException || ex is DirectoryNotFoundException) {
          MessageService.ShowWarning(new string[] {
            firstLine,
            "Could not find file",
            settings.Location.FileName
           });
        } else if (ex is KeyFormatterException || ex is PpkFormatterException) {
          MessageService.ShowWarning(new string[] {
            firstLine,
            string.Format ("Could not load file {0}",
              settings.Location.SelectedType == EntrySettings.LocationType.File
                ? string.Format("'{0}'", settings.Location.FileName)
                : string.Format("from attachment '{0}'", settings.Location.AttachmentName)),
            "Possible causes:",
            "- Passphrase was entered incorrectly",
            "- File is corrupt or has been tampered"
           });
        } else if (ex is AgentFailureException) {
          MessageService.ShowWarning(new string[] {
            firstLine,
            "Agent Failure",
            "Possible causes:",
            "- Key is already loaded in agent",
            "- Agent is locked"
          });
        } else if (ex is AgentNotRunningException) {
          MessageService.ShowWarning(new string[] {
            firstLine,
            "Could not add key because no SSH agent was found.",
            "Please make sure your SSH agent program is running (e.g. Pageant)."
          });
        } else {
          MessageService.ShowWarning(new string[] {
            firstLine,
            "Unexpected error",
            ex.ToString()
          });
          Debug.Fail(ex.ToString());
        }
        throw;
      }
    }

    ICollection<ISshKey> FilterKeyList(ICollection<ISshKey> list)
    {
      if (!Options.UserPicksKeyOnRequestIdentities || list.Count <= 1) {
        return list;
      }

      // TODO: Using the main thread here will cause a lockup with IOProtocolExt
      pluginHost.MainWindow.Invoke((MethodInvoker)delegate
      {
        var dialog = new KeyPicker(list);
        dialog.Shown += (sender, e) => dialog.Activate();
        dialog.TopMost = true;
        dialog.ShowDialog(pluginHost.MainWindow);
        if (dialog.DialogResult == DialogResult.OK) {
          list = dialog.SelectedKeys.ToList();
        } else {
          list.Clear();
        }
      });
      return list;
    }
  } // class
} // namespace
