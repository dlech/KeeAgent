using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeeAgent.UI
{
  public class GroupBoxEx : GroupBox
  {
    private string pSelectedRadioButton = string.Empty;

    /// <summary>
    /// 
    /// </summary>
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
        }
      }
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
      base.OnControlAdded(e);
      var radioButton = e.Control as RadioButton;
      if (radioButton != null) {
        radioButton.CheckedChanged +=
          delegate(object aSender, EventArgs aEventArgs)
          {
            if (radioButton.Checked) {
              SelectedRadioButton = radioButton.Name;
            }
          };
      }
    }
  }
}
