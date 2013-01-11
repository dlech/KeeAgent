using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using KeePass.Forms;

namespace KeeAgent.UI
{
  public partial class EntryPanel : UserControl
  {

    private PwEntryForm mPwEntryForm;

    public EntryPanel()
    {
      InitializeComponent();

      // make transparent so tab styling shows
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      mPwEntryForm = ParentForm as PwEntryForm;
      if (mPwEntryForm != null) {
        entrySettingsBindingSource.DataSource =
          mPwEntryForm.EntryRef.GetKeeAgentSettings();
        entrySettingsBindingSource.BindingComplete +=
          entrySettingsBindingSource_BindingComplete;
      } else {
        Debug.Fail("Don't have settings to bind to");
      }
      UpdateControlStates();
    }

    private void entrySettingsBindingSource_BindingComplete(object aSender,
      BindingCompleteEventArgs aEventArgs)
    {
      if (aEventArgs.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate) {
        aEventArgs.Binding.BindingManagerBase.EndCurrentEdit();
        var settings = entrySettingsBindingSource.DataSource as EntrySettings;
        if (settings != null) {
          if (mPwEntryForm != null) {
            mPwEntryForm.EntryStrings.SetKeeAgentSettings(settings);
          }
        } else {
          Debug.Fail("Don't have settings");
        }
      }
    }

    private void UpdateControlStates()
    {
      addKeyAtOpenCheckBox.Enabled = hasSshKeyCheckBox.Checked;
      removeKeyAtCloseCheckBox.Enabled = hasSshKeyCheckBox.Checked;
      keyLocationPanel.Enabled = hasSshKeyCheckBox.Checked;
    }

    private void hasSshKeyCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      UpdateControlStates();
    }
  }
}
