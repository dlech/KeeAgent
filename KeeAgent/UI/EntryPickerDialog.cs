using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using dlech.SshAgentLib;
using KeePass.App;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;

namespace KeeAgent.UI
{
  public partial class EntryPickerDialog : Form
  {
    public IPluginHost mPluginHost;
    public DateTime mCachedNow;
    public PwDatabase mActiveDb;
    public Font mExpiredFont, mBoldFont, mItalicFont;

    public PwEntry SelectedEntry { get; private set; }

    public ICollection<Agent.KeyConstraint> Constraints
    {
      get
      {
        var constraints = new List<Agent.KeyConstraint>();
        if (mConfirmConstraintControl.Checked) {
          constraints.addConfirmConstraint();
        }
        if (mLifetimeConstraintControl.Checked) {
          constraints.addLifetimeConstraint(mLifetimeConstraintControl.Lifetime);
        }
        return constraints;
      }
    }

    public EntryPickerDialog(IPluginHost aPluginHost)
    {
      mPluginHost = aPluginHost;
      InitializeComponent();
#if !__MonoCS__
      // TODO figure out why this crashes mono
      Icon = Properties.Resources.KeeAgent_ico;
#endif

      mExpiredFont = FontUtil.CreateFont(mCustomTreeViewEx.Font, FontStyle.Strikeout);
      mBoldFont = FontUtil.CreateFont(mCustomTreeViewEx.Font, FontStyle.Bold);
      mItalicFont = FontUtil.CreateFont(mCustomTreeViewEx.Font, FontStyle.Italic);
      InitalizeList();
    }

    private void InitalizeList()
    {
      mCustomTreeViewEx.BeginUpdate();
      mCustomTreeViewEx.Nodes.Clear();
      mCachedNow = DateTime.Now;

      foreach (var db in mPluginHost.MainWindow.DocumentManager.GetOpenDatabases()) {
        mActiveDb = db;
        UpdateImageLists();

        TreeNode rootNode = null;
        var rootGroup = mActiveDb.RootGroup;
        if (rootGroup != null) {
          int nIconID = ((!rootGroup.CustomIconUuid.EqualsValue(PwUuid.Zero)) ?
            ((int)PwIcon.Count + mActiveDb.GetCustomIconIndex(
            rootGroup.CustomIconUuid)) : (int)rootGroup.IconId);
          if (rootGroup.Expires && (rootGroup.ExpiryTime <= mCachedNow)) {
            nIconID = (int)PwIcon.Expired;
          }

          rootNode = new TreeNode(rootGroup.Name, nIconID, nIconID);
          rootNode.Tag = rootGroup;
          rootNode.ForeColor = SystemColors.GrayText;

          if (mBoldFont != null) {
            rootNode.NodeFont = mBoldFont;
          }
          rootNode.ToolTipText = db.IOConnectionInfo.GetDisplayName();

          mCustomTreeViewEx.Nodes.Add(rootNode);
        }

        RecursiveAddGroup(rootNode, rootGroup);

        if (rootNode != null) {
          rootNode.Expand();
        }
      }
      mCustomTreeViewEx.EndUpdate();
    }

    private void RecursiveAddGroup(TreeNode aParentNode, PwGroup aParentGroup)
    {
      if (aParentGroup == null) {
        return;
      }

      TreeNodeCollection treeNodes;
      if (aParentNode == null) treeNodes = mCustomTreeViewEx.Nodes;
      else treeNodes = aParentNode.Nodes;

      foreach (PwGroup childGroup in aParentGroup.Groups) {
        bool bExpired = (childGroup.Expires && (childGroup.ExpiryTime <= mCachedNow));
        string strName = childGroup.Name;

        int iconID = ((!childGroup.CustomIconUuid.EqualsValue(PwUuid.Zero)) ?
          ((int)PwIcon.Count + mActiveDb.GetCustomIconIndex(childGroup.CustomIconUuid)) :
          (int)childGroup.IconId);
        if (bExpired) {
          iconID = (int)PwIcon.Expired;
        }

        var newNode = new TreeNode(strName, iconID, iconID);
        newNode.Tag = childGroup;
        newNode.ForeColor = SystemColors.GrayText;
        UIUtil.SetGroupNodeToolTip(newNode, childGroup);

        if (mActiveDb.RecycleBinEnabled &&
            childGroup.Uuid.EqualsValue(mActiveDb.RecycleBinUuid) &&
            (mItalicFont != null)) {

          newNode.NodeFont = mItalicFont;
        } else if (bExpired && (mExpiredFont != null)) {
          newNode.NodeFont = mExpiredFont;
        }

        treeNodes.Add(newNode);

        RecursiveAddGroup(newNode, childGroup);

        foreach (var entry in childGroup.Entries) {
          var settings = entry.GetKeeAgentSettings();
          if (settings.AllowUseOfSshKey) {
            var entryNode = new TreeNode(entry.Strings.Get(PwDefs.TitleField).ReadString(),
              (int)entry.IconId, (int)entry.IconId);
            entryNode.Tag = entry;

            if (entry.Expires && (entry.ExpiryTime <= mCachedNow)) {
              entryNode.ImageIndex = (int)PwIcon.Expired;
              if (mExpiredFont != null) entryNode.NodeFont = mExpiredFont;
            } else { // Not expired			
              if (entry.CustomIconUuid.EqualsValue(PwUuid.Zero))
                entryNode.ImageIndex = (int)entry.IconId;
              else
                entryNode.ImageIndex = (int)PwIcon.Count +
                  mActiveDb.GetCustomIconIndex(entry.CustomIconUuid);
            }
            entryNode.ForeColor = entry.ForegroundColor;
            entryNode.BackColor = entryNode.BackColor;
            newNode.Nodes.Add(entryNode);
          }
        }


        if (newNode.Nodes.Count > 0) {
          if ((newNode.IsExpanded) && (!childGroup.IsExpanded)) {
            newNode.Collapse();
          } else if ((!newNode.IsExpanded) && (childGroup.IsExpanded)) {
            newNode.Expand();
          }
        }

      }
    }

    private void UpdateImageLists()
    {
      ImageList imgList = new ImageList();
      imgList.ImageSize = new Size(16, 16);
      imgList.ColorDepth = ColorDepth.Depth32Bit;

      List<Image> lStdImages = new List<Image>();
      foreach (Image imgStd in mPluginHost.MainWindow.ClientIcons.Images) {
        lStdImages.Add(imgStd);
      }
      imgList.Images.AddRange(lStdImages.ToArray());

      Debug.Assert(imgList.Images.Count == (int)PwIcon.Count);

      List<Image> lCustom = UIUtil.BuildImageListEx(
       mActiveDb.CustomIcons, 16, 16);
      if ((lCustom != null) && (lCustom.Count > 0))
        imgList.Images.AddRange(lCustom.ToArray());

      if (UIUtil.VistaStyleListsSupported) {
        mCustomTreeViewEx.ImageList = imgList;
      } else {
        List<Image> vAllImages = new List<Image>();
        foreach (Image imgClient in imgList.Images)
          vAllImages.Add(imgClient);
        vAllImages.AddRange(lCustom);
        Debug.Assert(imgList.Images.Count == vAllImages.Count);

        ImageList imgSafe = UIUtil.ConvertImageList24(vAllImages, 16, 16,
          AppDefs.ColorControlNormal);
        mCustomTreeViewEx.ImageList = imgSafe;
      }
    }

    private void customTreeViewEx_NodeMouseDoubleClick(object sender,
      TreeNodeMouseClickEventArgs e)
    {
      var entry = e.Node.Tag as PwEntry;
      if (entry != null)      {
        AcceptButton.PerformClick();
      }
    }

    private void customTreeViewEx_AfterSelect(object sender, TreeViewEventArgs e)
    {
      SelectedEntry = e.Node.Tag as PwEntry;
    }

    private void EntryPickerDialog_FormClosing(object sender,
      FormClosingEventArgs e)
    {
      if (DialogResult == DialogResult.OK) {
        if (SelectedEntry == null) {
          MessageBox.Show("Must select an entry", Util.AssemblyTitle,
             MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          e.Cancel = true;
        }
        if (mLifetimeConstraintControl.Checked &&
          mLifetimeConstraintControl.Lifetime == 0)
        {
          MessageBox.Show("Invalid lifetime", Util.AssemblyTitle,
             MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          e.Cancel = true;
        }
      }
    }

  }
}
