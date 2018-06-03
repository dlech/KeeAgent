//
//  DatabaseSettingsPanel.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2013-2014  David Lechner
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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using KeePassLib;

namespace KeeAgent.UI
{
  public partial class DatabaseSettingsPanel : UserControl
  {

    private PwDatabase mDatabase;

    public DatabaseSettingsPanel(PwDatabase aDatabase)
    {
      mDatabase = aDatabase;

      InitializeComponent();

      // make transparent so tab styling shows
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;

      mDatabaseSettingsBindingSource.DataSource = mDatabase.GetKeeAgentSettings();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (ParentForm != null) {
        ParentForm.FormClosing += (s, e2) =>
          {
            if (ParentForm.DialogResult == DialogResult.OK) {
              var settings = mDatabaseSettingsBindingSource.DataSource as DatabaseSettings;
              mDatabase.SetKeeAgentSettings(settings);
            }
          };
      }
    }

    private void mHelpButton_Click(object sender, EventArgs e)
    {
      Process.Start(Properties.Resources.WebHelpDatabaseSettings);
    }
  }
}
