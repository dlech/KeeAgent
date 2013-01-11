using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

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
