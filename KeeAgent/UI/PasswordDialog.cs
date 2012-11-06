using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KeePass.UI;
using dlech.PageantSharp;

namespace KeeAgent.UI
{
  public partial class PasswordDialog : Form
  {
    SecureEdit mSecureEdit;

    public PasswordDialog()
    {
      InitializeComponent();

      mSecureEdit = new SecureEdit();
      mSecureEdit.Attach(passwordTextBox, PasswordTextChanged, true);

    }

    public PasswordDialog(string aMessage) : this()
    {
      SetMessage(aMessage);
    }

    public void SetMessage(string aMessage)
    {
      messageLabel.Text = aMessage;
    }

    public PinnedByteArray GetPassword()
    {
      return new PinnedByteArray(mSecureEdit.ToUtf8());
    }

    private void PasswordTextChanged(object aSender, EventArgs aEventArgs)
    {

    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      mSecureEdit.Detach();
      base.Dispose(disposing);
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }   

  }
}
