using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using KeePass.UI;

namespace KeeAgent.UI
{
  public partial class OptionsPanel : UserControl
  {
    private KeeAgentExt mExt;
    private CheckedLVItemDXList mOptionsList;

    public OptionsPanel(KeeAgentExt aExt)
    {
      mExt = aExt;

      InitializeComponent();

      // make transparent so tab styling shows
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;
            
      // additional configuration of list view
      customListViewEx1.UseCompatibleStateImageBehavior = false;
      UIUtil.SetExplorerTheme(customListViewEx1, false);
      mOptionsList = new CheckedLVItemDXList(customListViewEx1, true);
      var optionsGroup = customListViewEx1.Groups["options"];
      mOptionsList.CreateItem(aExt.Options, "AlwasyConfirm", optionsGroup,
        Translatable.OptionAlwaysConfirm);
      mOptionsList.CreateItem(aExt.Options, "ShowBalloon", optionsGroup,
        Translatable.OptionShowBalloon);
      //mOptionsList.CreateItem(aExt.Options, "LoggingEnabled", optionsGroup,
      //  Translatable.optionLoggingEnabled);
      columnHeader1.Width = customListViewEx1.ClientRectangle.Width -
        UIUtil.GetVScrollBarWidth() - 1;
      mOptionsList.UpdateData(false);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (ParentForm != null) {
        ParentForm.FormClosing +=
          delegate(object aSender, FormClosingEventArgs aEventArgs)
          {            
            if (ParentForm.DialogResult == DialogResult.OK) {
              mOptionsList.UpdateData(true);
            }
            mOptionsList.Release();
          };
      }
    }   
  }
}
