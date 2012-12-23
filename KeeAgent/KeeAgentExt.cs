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

namespace KeeAgent
{
  public sealed partial class KeeAgentExt : Plugin
  {
    internal IPluginHost mPluginHost;
    internal bool mDebug;
    internal IAgent mAgent;

    private ToolStripMenuItem mKeeAgentMenuItem;
    private List<string> mApprovedKeys;
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
        // TODO make this happen on database load
        var exitFor = false;
        foreach (var entry in mPluginHost.Database.RootGroup.GetEntries(true)) {
          if (exitFor) {
            break;
          }
          var settings = entry.GetKeeAgentEntrySettings();
          if (settings.LoadAtStartup) {
            if (!AddEntry(entry)) {
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
      ManageDialog dialog = new ManageDialog(this);
      DialogResult result = dialog.ShowDialog(mPluginHost.MainWindow);
      dialog.Dispose();
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

    public bool AddEntry(PwEntry aEntry)
    {
      KeyFormatter.GetPassphraseCallback getPassphraseCallback = delegate()
            {
              var securePassphrase = new SecureString();
              var passphrase = Encoding.UTF8.GetChars(aEntry.Strings
                .Get(PwDefs.PasswordField).ReadUtf8());
              foreach (var c in passphrase) {
                securePassphrase.AppendChar(c);
              }
              Array.Clear(passphrase, 0, passphrase.Length);
              return securePassphrase;
            };
      var settings = aEntry.GetKeeAgentEntrySettings();
      switch (settings.Location.SelectedType) {
        case EntrySettings.LocationType.Attachment:
          try {
            var data = aEntry.Binaries.Get(settings.Location.AttachmentName).ReadData();
            using (var reader = new StreamReader(new MemoryStream(data))) {
              var formatter = KeyFormatter.GetFormatter(reader.ReadLine());
              formatter.GetPassphraseCallbackMethod = getPassphraseCallback;
              var key = formatter.Deserialize(data);
              if (Options.AlwasyConfirm) {
                key.addConfirmConstraint();
              }
              return mAgent.AddKey(key);
            }
          } catch (Exception ex) {
            Debug.Fail(ex.ToString());
          }
          break;
        case EntrySettings.LocationType.File:
          var fileName = settings.Location.FileName;
          fileName = Environment.ExpandEnvironmentVariables(fileName);
          fileName = Path.GetFullPath(fileName);
          try {
            var constraints = new List<Agent.KeyConstraint>();
            if (Options.AlwasyConfirm) {
              constraints.addConfirmConstraint();
            }
            return mAgent.AddKeyFromFile(fileName, getPassphraseCallback,
              constraints);
          } catch (Exception ex) {
            if (ex is FileNotFoundException || ex is DirectoryNotFoundException) {
              MessageBox.Show("Could not find file " + fileName);
            } else if (ex is KeyFormatterException || ex is PpkFormatterException) {
              MessageBox.Show("Bad passphrase " + fileName);
            } else {
              Debug.Fail(ex.ToString());
            }
          }
          break;
      }
      return false;
    }
  } // class
} // namespace
