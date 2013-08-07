//
//  TreeViewEx.cs
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

using System.Drawing;
using System.Windows.Forms;

namespace KeeAgent.UI
{
  /// <summary>
  /// nodes can be disabled by setting ForeColor to SystemColors.GrayText
  /// </summary>
  public class TreeViewEx : TreeView
  {
    // used to prevent clicking on disabled nodes
    protected override void WndProc(ref Message m)
    {
      const int WM_LBUTTONDOWN = 0x0201;
      const int WM_LBUTTONUP = 0x0202; 
      const int WM_LBUTTONDBLCLK = 0x0203;
      const int WM_RBUTTONDOWN = 0x0204;
      const int WM_RBUTTONUP = 0x0205; 
      const int WM_RBUTTONDBLCLK = 0x0206;
           

      if (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_LBUTTONUP ||
          m.Msg == WM_LBUTTONDBLCLK ||  m.Msg == WM_RBUTTONDOWN ||
          m.Msg == WM_RBUTTONUP ||  m.Msg == WM_RBUTTONDBLCLK) {

        var posData = m.LParam.ToInt32();
        var xPos = posData & 0xFF;
        var yPos = posData >> 16;

        var pos = new Point(xPos, yPos);
        var node = GetNodeAt(pos);
        if (node != null) {
          var bounds = node.Bounds;
          bounds.X -= 19;
          bounds.Width += 20;
          if (bounds.Contains(pos) && node.ForeColor == SystemColors.GrayText) {
            return;
          }
        }
      }
      base.WndProc(ref m);
    }

    // cancel selection just in case we get selected somehow
    protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
    {
      base.OnBeforeSelect(e);
      if (e.Node.ForeColor == SystemColors.GrayText) {
        e.Cancel = true;
      }
    }
  }
}
