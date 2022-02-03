// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2022 David Lechner <david@lechnology.com>

using System.ComponentModel;
using System.Windows.Forms;

namespace KeeAgent.UI
{
  /// <summary>
  /// Label-like control for displaying an in-place message with icon.
  /// </summary>
  public partial class InPlaceMessage : UserControl
  {
    /// <summary>
    /// Creates a new in-place message control.
    /// </summary>
    public InPlaceMessage()
    {
      InitializeComponent();
      label1.TextChanged += (s, e) => OnTextChanged(e);
    }

    /// <summary>
    /// Gets and sets the icon.
    /// </summary>
    [Category("Appearance"), DefaultValue(SystemIcon.StockIconId.Info)]
    public SystemIcon.StockIconId StockIcon {
      get { return systemIcon1.StockIcon; }
      set { systemIcon1.StockIcon = value; }
    }

    /// <summary>
    /// Gets and sets the message text.
    /// </summary>
    [Bindable(true)]
    [Browsable(true)]
    [DefaultValue("<message>")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override string Text {
      get { return label1.Text; }
      set { label1.Text = value; }
    }

    public override void ResetText()
    {
      Text = "<message>";
    }
  }
}
