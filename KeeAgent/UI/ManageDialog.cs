using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using dlech.SshAgentLib;
using KeePassLib;
using System.ComponentModel;
using System.Drawing;
using KeeAgent.Properties;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace KeeAgent.UI
{
  public partial class ManageDialog : Form
  {
    private IAgent mAgent;

    public ManageDialog(IAgent aAgent)
    {
      InitializeComponent();
      mAgent = aAgent;
      keyInfoView1.SetAgent(mAgent);
    }

    private void ManageDialog_Load(object sender, EventArgs e)
    {
      
    }



  }
}
