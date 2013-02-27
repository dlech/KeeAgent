using System;
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

      databaseSettingsBindingSource.DataSource = mDatabase.GetKeeAgentSettings();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (ParentForm != null) {
        ParentForm.FormClosing +=
          delegate(object aSender, FormClosingEventArgs aEventArgs)
          {
            if (ParentForm.DialogResult == DialogResult.OK) {
              var settings = databaseSettingsBindingSource.DataSource as DatabaseSettings;
              mDatabase.SetKeeAgentSettings(settings);
            }
          };
      }
    }


  }
}
