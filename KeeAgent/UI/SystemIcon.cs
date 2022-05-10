// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2022 David Lechner <david@lechnology.com>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeeAgent.UI
{
  /// <summary>
  /// A control that gets a properly scaled system icon.
  /// </summary>
  public class SystemIcon : PictureBox
  {
    [DllImport("Comctl32.dll")]
    private static extern int LoadIconWithScaleDown(
      IntPtr hinstance,
      IntPtr pszName,
      int cx,
      int cy,
      out IntPtr phico);

    [DllImport("User32.dll")]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public enum StockIconId : uint
    {
      Info = 32516, // IDI_INFORMATION
      Warning = 32515, // IDI_WARNING
      Error = 32513, // IDI_ERROR,
      Help = 32514, // IDI_QUESTION
    }

    /// <summary>
    /// Creates a new system icon.
    /// </summary>
    public SystemIcon()
    {
      BackColor = Color.Transparent;
      SizeMode = PictureBoxSizeMode.CenterImage;

      stockIconId = StockIconId.Info;
      SizeChanged += (s, e) => UpdateImage();
      UpdateImage();
    }

    private StockIconId stockIconId;

    /// <summary>
    /// Gets and sets the stock icon type.
    /// </summary>
    [Category("Appearance"), DefaultValue(StockIconId.Info)]
    public StockIconId StockIcon {
      get {
        return stockIconId;
      }
      set {
        stockIconId = value;
        UpdateImage();
      }
    }

    private void UpdateImage()
    {
      // Use ugly icons on mono - it's the best we can do
      if (Type.GetType("Mono.Runtime") != null) {
        Icon icon;

        switch (stockIconId)  {
          case StockIconId.Info:
            icon = SystemIcons.Information;
            break;
          case StockIconId.Warning:
            icon = SystemIcons.Warning;
            break;
          case StockIconId.Error:
            icon = SystemIcons.Error;
            break;
          case StockIconId.Help:
            icon = SystemIcons.Question;
            break;
          default:
            throw new NotSupportedException("unsupported icon id");
        }

        Image = new Icon(icon, Size.Width, Size.Height).ToBitmap();

        return;
      }

      IntPtr handle;

      Marshal.ThrowExceptionForHR(
        LoadIconWithScaleDown(IntPtr.Zero, (IntPtr)stockIconId, Size.Width, Size.Height, out handle));

      try {
        Image = Icon.FromHandle(handle).ToBitmap();
      }
      finally {
        DestroyIcon(handle);
      }
    }
  }
}
