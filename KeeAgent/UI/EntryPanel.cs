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

    public EntrySettings IntialSettings {
      get;
      private set;
    }

    public EntrySettings CurrentSettings {
      get;
      private set;
    }

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
        IntialSettings =
          mPwEntryForm.EntryRef.GetKeeAgentSettings();
        CurrentSettings = (EntrySettings)IntialSettings.Clone ();
        entrySettingsBindingSource.DataSource = CurrentSettings;
      } else {
        Debug.Fail("Don't have settings to bind to");
      }
      UpdateControlStates();
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

    private void helpButton_Click(object sender, EventArgs e)
    {
      Process.Start(Properties.Resources.WebHelpEntryOptions);
    }
  }
}
