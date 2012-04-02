using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KeePassLib;

namespace KeeAgent.UI
{
	public partial class PuttyKeyListDialog : Form
	{
		public PuttyKeyListDialog(PwDatabase database)
		{
			InitializeComponent();
			// borrow icon from parent
			if (this.ParentForm != null) {
				this.Icon = this.ParentForm.Icon;
			}
		}
	}
}
