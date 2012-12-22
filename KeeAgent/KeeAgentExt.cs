using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using dlech.SshAgentLib;
using KeeAgent.Properties;
using KeeAgent.UI;
using KeePass.App;
using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;
using System.ComponentModel;
using KeePass.UI;
using KeePass.Forms;

namespace KeeAgent
{
  public sealed partial class KeeAgentExt : Plugin
  {
    internal IPluginHost mPluginHost;
    internal bool mDebug;
    internal PageantAgent mPageant;

    private ToolStripMenuItem mKeeAgentMenuItem;
    private List<string> mApprovedKeys;
    private UIHelper mUIHelper;

    private const string cPluginName = "KeeAgent";
    private const string cAlwaysConfirmOptionName = cPluginName + ".AlwaysConfirm";
    private const string cShowBalloonOptionName = cPluginName + ".ShowBalloon";
    private const string cNotificationOptionName = cPluginName + ".Notification";
    private const string cLogginEnabledOptionName = cPluginName + ".LoggingEnabled";
    private const string cLogFileNameOptionName = cPluginName + ".LogFileName";

    public Options Options { get; private set; }

    public override bool Initialize(IPluginHost aHost)
    {
      bool result;

      mPluginHost = aHost;
      mUIHelper = new UIHelper(mPluginHost);
      mDebug = (mPluginHost
          .CommandLineArgs[AppDefs.CommandLineOptions.Debug] != null);

      LoadOptions();
      mApprovedKeys = new List<string>();

      if (mDebug) Log("Loading KeeAgent...");

      result = false;
      try {
        // TODO check OS - currently only works on Windows
        mPageant = new PageantAgent();
        mPageant.Locked += Pageant_Locked;
        mPageant.KeyUsed += Pageant_KeyUsed;
        mPageant.KeyListChanged += Pageant_KeyListChanged;
        result = true;
        if (mDebug) Log("Succeeded");
      } catch (PageantRunningException) {
        ShowPageantRunningErrorMessage();
      } catch (Exception) {
        if (mDebug) Log("Failed");
      }
      if (result) {
        AddMenuItems();
      }

      GlobalWindowManager.WindowAdded += WindowAddedHandler;

      return result;
    }

    public override void Terminate()
    {
      GlobalWindowManager.WindowAdded -= WindowAddedHandler;
      if (mDebug) Log("Terminating KeeAgent");
      if (mPageant != null) {
        // need reference to pageant here so GC doesn't eat it!
        mPageant.Dispose();
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
      /* get Tools menu */
      ToolStripMenuItem toolsMenu = mPluginHost.MainWindow.ToolsMenu;

      /* create parent menu item */
      mKeeAgentMenuItem = new ToolStripMenuItem();
      mKeeAgentMenuItem.Text = Translatable.KeeAgent;

      /* create children menu items */
      ToolStripMenuItem keeAgentListPuttyKeysMenuItem =
          new ToolStripMenuItem();
      keeAgentListPuttyKeysMenuItem.Text =
          Translatable.ManageKeeAgentMenuItem;
      keeAgentListPuttyKeysMenuItem.Click +=
          new EventHandler(manageKeeAgentMenuItem_Click);

      /* add children to parent */
      mKeeAgentMenuItem.DropDownItems.Add(keeAgentListPuttyKeysMenuItem);

      /* add new items to tools menu */
      toolsMenu.DropDownItems.Add(mKeeAgentMenuItem);

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
      ManageDialog dialog = new ManageDialog(mPageant);
      DialogResult result = dialog.ShowDialog(mPluginHost.MainWindow);
      dialog.Dispose();
    }
    
    internal void SaveOptions()
    {
      mPluginHost.CustomConfig.SetString(cAlwaysConfirmOptionName,
          Options.AlwasyConfirm.ToString());
      mPluginHost.CustomConfig.SetString(cShowBalloonOptionName,
          Options.ShowBalloon.ToString());
      mPluginHost.CustomConfig.SetBool(cLogginEnabledOptionName,
          Options.LoggingEnabled);
      mPluginHost.CustomConfig.SetString(cLogFileNameOptionName,
          Options.LogFileName);
    }

    private void LoadOptions()
    {
      Options = new Options();

      /* Always Confirm options */

      bool defaultAlwaysConfirmValue = false;
      bool configFileAlwaysConfirmValue =
          mPluginHost.CustomConfig.GetBool(cAlwaysConfirmOptionName,
          defaultAlwaysConfirmValue);
      Options.AlwasyConfirm = configFileAlwaysConfirmValue;

      /* Show Balloon options */

      bool defaultShowBalloonValue = true;
      bool configFileShowBalloonValue =
          mPluginHost.CustomConfig.GetBool(cShowBalloonOptionName,
          defaultShowBalloonValue);
      Options.ShowBalloon = configFileShowBalloonValue;


      /* Notification Option */

      /* the Notification option is obsolete, so we read it and then clear it. */
      NotificationOptions configFileNotificationValue;
      if (Enum.TryParse<NotificationOptions>(mPluginHost.CustomConfig
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
        mPluginHost.CustomConfig
             .SetString(cNotificationOptionName, string.Empty);
      }

      /* Log File Options */

      bool defaultLoggingEnabledValue = false;
      bool configFileLoggingEnabledValue =
          mPluginHost.CustomConfig.GetBool(cLogginEnabledOptionName,
          defaultLoggingEnabledValue);
      Options.LoggingEnabled = configFileLoggingEnabledValue;

      string defaultLogFileNameValue = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
          "KeeAgent.log");
      string configFileLogFileNameValue =
          mPluginHost.CustomConfig.GetString(cLogFileNameOptionName);
      if (string.IsNullOrEmpty(configFileLogFileNameValue)) {
        Options.LogFileName = defaultLogFileNameValue;
      } else {
        Options.LogFileName = configFileLogFileNameValue;
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
            // TODO replace with new panel
            var optionsPanel = new OptionsPanel(this);
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
        optionsForm.FormClosing += OptionsFormClosingHandler;
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

    private void OptionsFormClosingHandler(object aSender,
      FormClosingEventArgs aEventArgs)
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

  } // class
} // namespace
