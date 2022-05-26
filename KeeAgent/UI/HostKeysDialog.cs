// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2022 David Lechner <david@lechnology.com>

using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SshAgentLib.Keys;

namespace KeeAgent.UI
{
  public partial class HostKeysDialog : Form
  {
    public HostKeysDialog()
    {
      InitializeComponent();
    }

    public void AddKeySpec(EntrySettings.DestinationConstraint.KeySpec spec)
    {
      keySpecBindingSource.Add(spec.DeepCopy());
    }

    public EntrySettings.DestinationConstraint.KeySpec[] GetKeySpecs()
    {
      return keySpecBindingSource.Cast<EntrySettings.DestinationConstraint.KeySpec>()
        .Where(x => x.HostKey != null).Select(x => x.DeepCopy()).ToArray();
    }

    private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
    {
      var view = (DataGridView)sender;

      foreach (DataGridViewRow row in view.Rows) {

        if (row.IsNewRow) {
          continue;
        }

        try {
          var k = row.DataBoundItem as EntrySettings.DestinationConstraint.KeySpec;

          if (string.IsNullOrWhiteSpace(k.HostKey)) {
            row.ErrorText = "host key is required";
            continue;
          }

          SshPublicKey.Parse(k.HostKey);

          // no error, so hide error badge
          row.ErrorText = string.Empty;
        }
        catch (Exception ex) {
          row.ErrorText = ex.Message;
        }
      }
    }

    private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
    {
      var grid = sender as DataGridView;

      var hitTest = grid.HitTest(e.X, e.Y);

      if (hitTest.Type == DataGridViewHitTestType.RowHeader) {
        var clickedRow = grid.Rows[hitTest.RowIndex];

        grid.CurrentCell = clickedRow.Cells[0];

        if (grid.CurrentRow.IsNewRow) {
          // can't remove uncommited new row
          deleteToolStripMenuItem.Enabled = false;
        }
        else {
          deleteToolStripMenuItem.Enabled = true;
        }
      }
      else if (hitTest.Type == DataGridViewHitTestType.Cell) {
        var clickedCell = grid.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex];

        grid.CurrentCell = clickedCell;

        if (grid.CurrentRow.IsNewRow) {
          // can't remove uncommited new row
          deleteToolStripMenuItem.Enabled = false;
        }
        else {
          deleteToolStripMenuItem.Enabled = true;
        }
      }
      else {
        deleteToolStripMenuItem.Enabled = false;
      }
    }

    private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
    {
      try {
        dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
      }
      catch {
        // don't crash the program
      }
    }
  }
}
