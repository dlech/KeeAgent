using System;
using System.Windows.Forms;
using dlech.SshAgentLib;
using KeePass.UI;

namespace KeeAgent.UI
{
  public partial class PasswordDialog : Form
  {
    SecureEdit mSecureEdit;

    public PasswordDialog()
    {
      InitializeComponent();
#if __MonoCS__
      Icon = Properties.Resources.KeeAgent_icon_mono;
#else
      Icon = Properties.Resources.KeeAgent_ico;
#endif
      mSecureEdit = new SecureEdit();
      mSecureEdit.Attach(passwordTextBox, null, true);
    }

    public PasswordDialog(string aMessage) : this()
    {
      SetMessage(aMessage);
    }

    public void SetMessage(string aMessage)
    {
      messageLabel.Text = aMessage;
    }

    public PinnedArray<byte> GetPassword()
    {
      return new PinnedArray<byte>(mSecureEdit.ToUtf8());
    }
        
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">
    /// true if managed resources should be disposed; otherwise, false.
    /// </param>
    protected override void Dispose(bool disposing)
    {
      mSecureEdit.Detach();
      if (disposing && (components != null)) {
        components.Dispose();
      }      
      base.Dispose(disposing);
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }   

  }
}
