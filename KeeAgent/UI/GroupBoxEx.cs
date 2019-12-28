//
//  GroupBoxEx.cs
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
using System.ComponentModel;
using System.Windows.Forms;

namespace KeeAgent.UI
{
  public class GroupBoxEx : GroupBox
  {
    private string pSelectedRadioButton = string.Empty;

    public event EventHandler SelectedRadioButtonChanged;

    /// <summary>
    /// 
    /// </summary>
    [Category("Appearance")]
    public string SelectedRadioButton
    {
      get
      {
        return pSelectedRadioButton;
      }
      set
      {
        if (value == null) {
          value = string.Empty;
        }
        if (value == pSelectedRadioButton) {
          return;
        }
        if (value == string.Empty) {
          foreach (var control in Controls) {
            var radioButton = Controls[value] as RadioButton;
            if (radioButton != null) {
              radioButton.Checked = false;
            }
          }
        } else {
          var selectedRadioButton = Controls[value] as RadioButton;
          if (selectedRadioButton == null) {
            throw new ArgumentException("unknown radio button");
          }
          selectedRadioButton.Checked = true;
          pSelectedRadioButton = value;
          OnSelectedRadioButtonChanged();
        }
      }
    }

    private void OnSelectedRadioButtonChanged()
    {
      if (SelectedRadioButtonChanged != null) {
        SelectedRadioButtonChanged(this, new EventArgs());
      }
      // Workaround for strange behavior of data binding:
      // If we change the SelecteRadioButton property and then change another 
      // data bound property in this group box, the SelectedRadioButton changes
      // will be reverted by the data binding code. To get around this, after we
      // have notified the the listeners, we select a control outside of the
      // group box and then select the usual next control.
      Parent.SelectNextControl(this, true, false, false, true);
      SelectNextControl(this, true, true, true, true);
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
      base.OnControlAdded(e);
      var radioButton = e.Control as RadioButton;
      if (radioButton != null) {
        radioButton.CheckedChanged += (s, e2) =>
          {
            if (radioButton.Checked) {
              SelectedRadioButton = radioButton.Name;
            }
          };
      }
    }
  }
}
