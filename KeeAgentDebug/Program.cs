using KeeAgent.UI;
using System;
using KeePassLib;
using KeePass.Plugins;
using KeePassPluginTestUtil;
using KeeAgent;
using System.Windows.Forms;
using KeeAgentTestProject.Properties;
using KeePassLib.Security;
using System.Security.Cryptography;
using System.Threading;

namespace KeeAgentDebug
{


  public static class Program
  {

    public static void Main()
    {
      using (KeePassAppDomain appDomain = new KeePassAppDomain()) {
        bool success = appDomain.StartKeePass(true, true, 2, true);
        if (!success) {
          MessageBox.Show("KeePass failed to start.");
          return;
        }

        appDomain.DoCallBack(delegate()
        {

          var keeAgent = new KeeAgentExt();
          var pluginHost = KeePass.Program.MainForm.PluginHost;

          var settings1 = new EntrySettings();
          settings1.HasSshKey = true;
          var settings2 = new EntrySettings();
          settings2.HasSshKey = true;
          settings2.LoadAtStartup = true;

          var withPassEntry = new PwEntry(true, true);
          withPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "with-passphrase"));
          withPassEntry.Binaries.Set("withPass.ppk", new ProtectedBinary(true, Resources.withPassphrase_ppk));
          withPassEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, "KeeAgent"));
          withPassEntry.SetKeeAgentEntrySettings(settings1);

          var withBadPassEntry = new PwEntry(true, true);
          withBadPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "with-bad-passphrase"));
          withBadPassEntry.Binaries.Set("withBadPass.ppk", new ProtectedBinary(true, Resources.withPassphrase_ppk));
          withBadPassEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, "BadPass"));
          withBadPassEntry.SetKeeAgentEntrySettings(settings1);

          var withoutPassEntry = new PwEntry(true, true);
          withoutPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "without-passphrase"));
          withoutPassEntry.Binaries.Set("withoutPass.ppk", new ProtectedBinary(true, Resources.withoutPassphrase_ppk));
          withoutPassEntry.SetKeeAgentEntrySettings(settings2);

          var dsaPassEntry = new PwEntry(true, true);
          dsaPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "dsa-with-passphrase"));
          dsaPassEntry.Binaries.Set("dsaWithPass.ppk", new ProtectedBinary(true, Resources.dsa_ppk));
          dsaPassEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, "KeeAgent"));
          dsaPassEntry.SetKeeAgentEntrySettings(settings1);

          var nonStandardLengthPassEntry = new PwEntry(true, true);
          nonStandardLengthPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "4095-bits"));
          nonStandardLengthPassEntry.Binaries.Set("4095-bits.ppk", new ProtectedBinary(true, Resources._4095_bits_ppk));
          nonStandardLengthPassEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, "KeeAgent"));
          nonStandardLengthPassEntry.SetKeeAgentEntrySettings(settings1);

          var nonAsciiPassEntry = new PwEntry(true, true);
          nonAsciiPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "non-ascii passphrase"));
          nonAsciiPassEntry.Binaries.Set("non-ascii-passphrase.ppk", new ProtectedBinary(true, Resources.non_ascii_passphrase_ppk));
          nonAsciiPassEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, "Ŧéşť"));
          nonAsciiPassEntry.SetKeeAgentEntrySettings(settings1);

          var rsaGroup = new PwGroup(true, true, "RSA", PwIcon.Key);
          rsaGroup.AddEntry(withPassEntry, true);
          //rsaGroup.AddEntry(withBadPassEntry, true);
          rsaGroup.AddEntry(withoutPassEntry, true);
          rsaGroup.AddEntry(nonAsciiPassEntry, true);

          var dsaGroup = new PwGroup(true, true, "DSA", PwIcon.Key);
          dsaGroup.AddEntry(dsaPassEntry, true);
          dsaGroup.AddEntry(nonStandardLengthPassEntry, true);

          var puttyGroup = new PwGroup(true, true, "Putty", PwIcon.Key);
          puttyGroup.AddGroup(rsaGroup, true);
          puttyGroup.AddGroup(dsaGroup, true);

          pluginHost.Database.RootGroup.AddGroup(puttyGroup, true);

          pluginHost.MainWindow.Invoke(new MethodInvoker(delegate()
          {
            pluginHost.MainWindow.UpdateUI(false, null, true, puttyGroup, true, puttyGroup, false);
            keeAgent.Initialize(pluginHost);
            pluginHost.MainWindow.FormClosing += delegate(Object source, FormClosingEventArgs args)
            {
              keeAgent.Terminate();
            };            
          }));
          while (KeePass.Program.MainForm != null && KeePass.Program.MainForm.Visible == true) {
            Thread.Sleep(500);
          }
        });
      }
      KeePassControl.ExitAll();
    }
  }
}
