using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using dlech.PageantSharp;

namespace dlech.KeeAgent
{
	class FakePutty
	{
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(String sClassName, String sAppName);

		public static void Main()
		{
			PageantWindow win = new PageantWindow();
			IntPtr hwnd = FindWindow("Pageant", "Pageant");
			if (hwnd == IntPtr.Zero) {
				MessageBox.Show("Pageant not running");
			} else {
				MessageBox.Show("Got handle: " + hwnd);
			}
			win.Dispose();
		}
	}
}
