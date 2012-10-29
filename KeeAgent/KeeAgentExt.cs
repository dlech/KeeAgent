using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using dlech.PageantSharp;
using KeeAgent.Properties;
using KeeAgent.UI;
using KeePass.App;
using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeeAgent
{
  public sealed partial class KeeAgentExt : Plugin
  {
    internal IPluginHost mPluginHost;
    internal Options mOptions;
    internal bool mDebug;
    internal HashSet<PpkKey> mInMemoryKeys;

    private WinPageant mPageant;
    private ApplicationContext mPageantContext;
    private Thread mPageantThread;
    private ToolStripMenuItem mKeeAgentMenuItem;
    private List<PwUuid> mApprovedKeys;
    private UIHelper MUIHelper;

    private const string cPluginName = "KeeAgent";
    private const string cNotificationOptionName = cPluginName + ".Notification";
    private const string cLogginEnabledOptionName = cPluginName + ".LoggingEnabled";
    private const string cLogFileNameOptionName = cPluginName + ".LogFileName";

    public override bool Initialize(IPluginHost aHost)
    {
      bool result;

      mPluginHost = aHost;
      MUIHelper = new UIHelper(mPluginHost);
      mDebug = (mPluginHost
          .CommandLineArgs[AppDefs.CommandLineOptions.Debug] != null);
      mInMemoryKeys = new HashSet<PpkKey>();

      loadOptions();
      mApprovedKeys = new List<PwUuid>();

      if (mDebug) Log("Loading KeeAgent...");

      try {
        // TODO check OS - currently only works on Windows

        mPageantThread = new Thread(delegate()
        {
          try {
            Agent.Callbacks callbacks = new Agent.Callbacks();
            callbacks.getSSH2KeyList = GetPpkKeyList;
            callbacks.getSSH2Key = GetSSH2Key;
            callbacks.addSSH2Key = AddKey;
            callbacks.removeSSH2Key = RemoveKey;
            callbacks.removeAllSSH2Keys = RemoveAllKeys;
            mPageant = new WinPageant(callbacks);
            mPageantContext = new ApplicationContext();
            Application.Run(mPageantContext);
          } catch (Exception ex) {
            ShowPageantRunningErrorMessage();
            if (mDebug) Log(ex.ToString());
          }
        });
        mPageantThread.Name = "PageantSharp";
        mPageantThread.SetApartmentState(ApartmentState.STA);
        mPageantThread.Start();
        if (mDebug) Log("Succeeded");
        result = mPageantThread.IsAlive;
      } catch (Exception) {
        if (mDebug) Log("Failed");
        result = false;
      }
      if (result) {
        AddMenuItems();
      }

      return result;
    }

    public override void Terminate()
    {
      if (mDebug) Log("Terminating KeeAgent");
      if (mPageant != null) {
        // need reference to pageant here so GC doesn't eat it!
        mPageant.Dispose();
      }
      if (mPageantThread.IsAlive) {
        mPageantContext.ExitThread();
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

      if (mPageantThread.IsAlive) {
        /* create children menu items */
        ToolStripMenuItem keeAgentListPuttyKeysMenuItem =
            new ToolStripMenuItem();
        keeAgentListPuttyKeysMenuItem.Text =
            Translatable.ShowPuttyKeysMenuItem;
        keeAgentListPuttyKeysMenuItem.ToolTipText =
            Translatable.ShowPuttyKeysMenuItemToolTip;
        keeAgentListPuttyKeysMenuItem.Click +=
            new EventHandler(keeAgentListPuttyKeysMenuItem_Click);

        ToolStripMenuItem keeAgentOptionsMenuItem =
            new ToolStripMenuItem();
        keeAgentOptionsMenuItem.Text = Translatable.OptionsMenuItem;
        keeAgentOptionsMenuItem.Click +=
            new EventHandler(keeAgentOptionsMenuItem_Click);

        /* add children to parent */
        mKeeAgentMenuItem.DropDownItems
            .Add(keeAgentListPuttyKeysMenuItem);
        mKeeAgentMenuItem.DropDownItems.Add(keeAgentOptionsMenuItem);
      } else {
        mKeeAgentMenuItem.Enabled = false;
      }

      /* add new items to tools menu */
      toolsMenu.DropDownItems.Add(mKeeAgentMenuItem);

    }

    private void RemoveMenuItems()
    {
      if (mPluginHost != null &&
          mPluginHost.MainWindow != null &&
          mKeeAgentMenuItem != null) {

        /* get Tools menu */
        ToolStripMenuItem toolsMenu =
            mPluginHost.MainWindow.ToolsMenu;
        /* remove items from tools menu */
        toolsMenu.DropDownItems.Remove(mKeeAgentMenuItem);
      }
    }

    private void keeAgentListPuttyKeysMenuItem_Click(
        object aSource, EventArgs aEvent)
    {
      KeyListDialog dialog = new KeyListDialog(this);
      DialogResult result = dialog.ShowDialog(mPluginHost.MainWindow);
      dialog.Dispose();
    }

    private void keeAgentOptionsMenuItem_Click(object aSource, EventArgs aEvent)
    {
      OptionsDialog dialog = new OptionsDialog(this);
      DialogResult result = dialog.ShowDialog(mPluginHost.MainWindow);
      dialog.Dispose();
    }

    internal IEnumerable<PpkKey> GetPpkKeyList()
    {
      return (IEnumerable<PpkKey>)GetKeeAgentKeyList(true);
    }

    internal IEnumerable<KeeAgentKey> GetKeeAgentKeyList()
    {
      return GetKeeAgentKeyList(false);
    }
    
    internal IEnumerable<KeeAgentKey> GetKeeAgentKeyList(
        bool aSuppressErrorMessage)
    {
      if (mDebug) Log("Getting Key List...");
      if (mDebug) Log("Called from " + new StackTrace().GetFrame(2)
          .GetMethod().Name);
      mPluginHost.MainWindow.NotifyUserActivity();

      List<KeeAgentKey> keyList = new List<KeeAgentKey>();
      List<PwDatabase> databases;
      databases = mPluginHost.MainWindow.DocumentManager
          .GetOpenDatabases();

      foreach (PwDatabase database in databases) {
        foreach (PwEntry entry in database.RootGroup
            .GetEntries(true)) {

          if (database.RecycleBinEnabled) {
            bool skipEntry = false;
            PwGroup testGroup = entry.ParentGroup;
            while (testGroup != null) {
              if (testGroup.Uuid.EqualsValue(
                  database.RecycleBinUuid)) {
                // ignore entries in recycle bin
                skipEntry = true;
                break;
              }
              testGroup = testGroup.ParentGroup;
            }
            if (skipEntry) {
              continue;
            }
          }

          foreach (KeyValuePair<string, ProtectedBinary> bin
              in entry.Binaries) {

            /* handle PuTTY Private Key files */

            if (bin.Key.EndsWith(".ppk")) {
              try {
                SecureString ssPassphrase = null;
                ProtectedString psPassphrase =
                  entry.Strings.Get(PwDefs.PasswordField);
                if (psPassphrase != null) {
                  string passphrase = psPassphrase.ReadString();
                  /* convert passphrase from KeePass protected format to .NET
                   * protected format */
                  ssPassphrase = new SecureString();
                  for (int i = 0; i < passphrase.Length; i++) {
                    ssPassphrase.AppendChar(passphrase[i]);
                  }
                }

                string dbPath = database.IOConnectionInfo.Path;

                PpkFile.GetPassphraseCallback getPassphrase = delegate()
                {
                  return ssPassphrase;
                };

                PpkFile.WarnOldFileFormatCallback warnUser = delegate()
                {
                  // we will warn user a different way... won't we???
                };

                PpkKey ppkKey = PpkFile.ParseData(bin.Value.ReadData(),
                  getPassphrase, warnUser);
                KeeAgentKey key = new KeeAgentKey(ppkKey, dbPath, entry.Uuid,
                  bin.Key);
                keyList.Add(key);
                if (mDebug) Log("Found " + PSUtil.ToHex(OpenSsh.GetFingerprint(key.CipherKeyPair)));
              } catch (Exception ex) {
                if (!aSuppressErrorMessage || mDebug) {
                  string errorMessage = string.Format(
                    Translatable.ErrParsingKey,
                      entry.Strings.Get(PwDefs.TitleField).ReadString(),
                      entry.ParentGroup.GetFullPath(
                      Path.DirectorySeparatorChar.ToString(), false),
                      database.IOConnectionInfo.GetDisplayName());
                  string details = Translatable.ErrUnknown;
                  if (ex is PpkFileException) {
                    PpkFileException ppkFileEx = (PpkFileException)ex;
                    details = string.Format(Translatable.ErrPpkFileException,
                        ppkFileEx.Error.ToString(),
                        bin.Key);
                  }
                  string debugInfo = null;
                  if (mDebug) {
                    debugInfo = ex.ToString();
                  }
                  MessageService.ShowWarning(errorMessage, details, debugInfo);
                  if (mDebug) Log(errorMessage);
                  if (mDebug) Log(details);
                  if (debugInfo != null) Log(debugInfo);
                }
              }
            } // end .ppk file
          }
        }
      }
      return keyList;
    }

    internal PpkKey GetSSH2Key(byte[] aFingerprint)
    {
      if (mDebug) Log("External program requested key " +
        PSUtil.ToHex(aFingerprint));
      mPluginHost.MainWindow.NotifyUserActivity();

      /* TODO it would probably be better if we cached the fingerprints and
       * mapped them to the database path and the PwEntry Uuid rather than
       * regenerating the full list to get a single key as we are doing here.
       * 
       * Also, there is the problem of duplicate fingerprints. We are currently
       * just selecting the first match.
       */

      IEnumerable<KeeAgentKey> ppkKeyList = GetKeeAgentKeyList();
      KeeAgentKey result = null;
      foreach (KeeAgentKey ppkKey in ppkKeyList) {
        if (result == null) {
          try {
            byte[] testFingerprint = OpenSsh.GetFingerprint(ppkKey.CipherKeyPair);
            if (testFingerprint.Length == aFingerprint.Length) {
              bool match = true;
              for (int i = 0; i < testFingerprint.Length; i++) {
                if (testFingerprint[i] != aFingerprint[i]) {
                  match = false;
                  break;
                }
              }
              if (match) {
                result = ppkKey;
              }
            }
          } catch (Exception ex) {
            Debug.Fail(ex.ToString());
          }
        }
        // dispose all keys except for the one match
        if (ppkKey != result) {
          ppkKey.Dispose();
        }
      }
      if (mDebug && result != null) Log("Match found");
      if (mDebug && result == null) Log("Match not found");
      if (result != null && confirmKeyRequest(result)) {
        return result;
      } else {
        result.Dispose();
        return null;
      }
    }

    internal bool AddKey(PpkKey aKey)
    {
      mInMemoryKeys.Add(aKey);
      return true;
    }

    internal bool RemoveKey(byte[] aFingerprint)
    {
      PpkKey removeKey = null;
      foreach(PpkKey key in mInMemoryKeys) {
        byte[] itemFingerprint = OpenSsh.GetFingerprint(key.CipherKeyPair);
        if (itemFingerprint == aFingerprint) {
          removeKey = key;
        }
      }
      if (removeKey != null) {
        mInMemoryKeys.Remove(removeKey);
      }
      return true;
    }

    internal bool RemoveAllKeys()
    {
      mInMemoryKeys.Clear();
      return true;
    }

    /// <summary>
    /// Asks for confirmation or notifies user of key request 
    /// depending on option selected
    /// </summary>
    /// <param name="aKey">The key being requested</param>
    /// <returns>true if the request was allowed by the user</returns>
    public bool confirmKeyRequest(KeeAgentKey aKey)
    {
      switch (mOptions.Notification) {
        case NotificationOptions.AlwaysAsk:
        case NotificationOptions.AskOnce:
          if (mOptions.Notification == NotificationOptions.AskOnce &&
            mApprovedKeys.Contains(aKey.Uuid)) {
            return true;
          }
          mPluginHost.MainWindow.Invoke((MethodInvoker)delegate()
          {
            // trick to make sure dialog shows in front of other applications
            // TODO this could be done better. Right now KeePass stays on top
            // we need to put it back where it was or find a way to only make
            // the dialog come to the top
            mPluginHost.MainWindow.TopMost = true;
            mPluginHost.MainWindow.TopMost = false;
          });
          DialogResult result = MessageBox.Show(
              string.Format(Translatable.ConfirmKeyFetch, aKey.Comment),
              string.Empty,
              MessageBoxButtons.YesNo, MessageBoxIcon.Question);

          if (mOptions.Notification == NotificationOptions.AskOnce &&
            result == DialogResult.Yes) {
            mApprovedKeys.Add(aKey.Uuid);
          }
          return result == DialogResult.Yes;
        case NotificationOptions.Balloon:
          string notifyText = string.Format(
              Translatable.NotifyKeyFetched,
              aKey.Comment);
          MUIHelper.ShowBalloonNotification(notifyText);
          return true;
        case NotificationOptions.Never:
          return true;
        default:
          Debug.Fail("Unsupported option");
          return false;
      }
    }

    internal void saveOptions()
    {
      mPluginHost.CustomConfig.SetString(
          KeeAgentExt.cNotificationOptionName,
          mOptions.Notification.ToString());
      mPluginHost.CustomConfig.SetBool(
          KeeAgentExt.cLogginEnabledOptionName,
          mOptions.LoggingEnabled);
      mPluginHost.CustomConfig.SetString(
          KeeAgentExt.cLogFileNameOptionName,
          mOptions.LogFileName);
    }

    private void loadOptions()
    {
      mOptions = new Options();

      /* Notification Option */

      NotificationOptions defaultNotificationValue =
          NotificationOptions.Balloon;
      NotificationOptions configFileNotificationValue;
      if (Enum.TryParse<NotificationOptions>(
          mPluginHost.CustomConfig.GetString(
          KeeAgentExt.cNotificationOptionName,
          defaultNotificationValue.ToString()),
          out configFileNotificationValue)) {
        mOptions.Notification = configFileNotificationValue;
      } else {
        mOptions.Notification = defaultNotificationValue;
      }

      /* Log File Options */

      bool defaultLoggingEnabledValue = false;
      bool configFileLoggingEnabledValue =
          mPluginHost.CustomConfig.GetBool(
          KeeAgentExt.cLogginEnabledOptionName,
          defaultLoggingEnabledValue);
      mOptions.LoggingEnabled = configFileLoggingEnabledValue;

      string defaultLogFileNameValue = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
          "KeeAgent.log");
      string configFileLogFileNameValue =
          mPluginHost.CustomConfig.GetString(
          KeeAgentExt.cLogFileNameOptionName);
      if (string.IsNullOrEmpty(configFileLogFileNameValue)) {
        mOptions.LogFileName = defaultLogFileNameValue;
      } else {
        mOptions.LogFileName = configFileLogFileNameValue;
      }
    }

    internal void Log(string aMessage)
    {
      if (mOptions.LoggingEnabled) {
        try {
          File.AppendAllText(mOptions.LogFileName,
              DateTime.Now + ": " + aMessage + "\r\n");
        } catch { }
      }
    }

  } // class
} // namespace
