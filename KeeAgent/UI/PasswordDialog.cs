//
//  PasswordDialog.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2012-2013  David Lechner
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

      if (Type.GetType("Mono.Runtime") == null)
        Icon = Properties.Resources.KeeAgent_icon;
      else
        Icon = Properties.Resources.KeeAgent_icon_mono;

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
