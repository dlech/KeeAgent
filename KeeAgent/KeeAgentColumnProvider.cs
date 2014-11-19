//
//  KeyStatusColumnProvider.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2012-2014  David Lechner
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dlech.SshAgentLib;
using KeePass.UI;
using KeePassLib;
using System.Windows.Forms;

namespace KeeAgent
{
  class KeeAgentColumnProvider : ColumnProvider, IDisposable
  {
    const string sshKeyStatusColumnName = "SSH Key Status";
    readonly string[] columnNames = {
        sshKeyStatusColumnName
      };

    KeeAgentExt ext;

    public override string[] ColumnNames
    {
      get { return columnNames.ToArray(); }
    }

    public KeeAgentColumnProvider(KeeAgentExt ext)
    {
      if (ext == null)
        throw new ArgumentNullException("ext");
      this.ext = ext;
      ext.agent.KeyAdded += Agent_KeyAddedOrRemoved;
      ext.agent.KeyRemoved += Agent_KeyAddedOrRemoved;
    }

    ~KeeAgentColumnProvider()
    {
      Dispose(false);
    }

    public override string GetCellData(string columnName, KeePassLib.PwEntry entry)
    {      
      switch (columnName) {
        case sshKeyStatusColumnName:
          try {
            var key = entry.GetSshKey();
            if (key == null)
              return "N/A";
            if (ext.agent.GetAllKeys().Get(key.Version, key.GetPublicKeyBlob()) != null)
              return "Loaded";
          } catch (PpkFormatterException) {
            return "Error";
          } catch (Exception ex) {
            Debug.Fail(ex.Message);
            return "*Error";
          }
          return "Not Loaded";
      }
      Debug.Fail(string.Format("Unknown column name: {0}", columnName));
      return string.Empty;
    }

    public override bool SupportsCellAction(string columnName)
    {
      switch (columnName) {
        case sshKeyStatusColumnName:
          return true;
        default:
          return false;
      }
    }

    public override void PerformCellAction(string columnName, KeePassLib.PwEntry entry)
    {
      switch (columnName) {
        case sshKeyStatusColumnName:
          try {
            var key = entry.GetSshKey();
            if (key == null)
              break;
            var agentKey = ext.agent.GetAllKeys().Get(key.Version, key.GetPublicKeyBlob());
            if (agentKey == null)
              ext.AddEntry(entry, null);
            else
              ext.agent.RemoveKey(agentKey);
          } catch (Exception ex) {
            Debug.Fail(ex.Message);
          }
          break;
        default:
          Debug.Fail(string.Format("Unsupported column: {0}", columnName));
          break;
      }
    }

    internal void Agent_KeyAddedOrRemoved(object sender, SshKeyEventArgs e)
    {
      // only update the entry list
      KeePass.Program.MainForm.UpdateUI(false, null, false, null, true, null, false);
    }

    private void Dispose(bool disposing)
    {
      ext.agent.KeyAdded -= Agent_KeyAddedOrRemoved;
      ext.agent.KeyRemoved -= Agent_KeyAddedOrRemoved;
    }

    public void Dispose()
    {
      GC.SuppressFinalize(this);
      Dispose(true);
    }
  }
}
