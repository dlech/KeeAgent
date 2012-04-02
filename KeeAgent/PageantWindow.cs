using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace KeeAgent
{
	/// <summary>
	/// Creates a window using Windows API calls so that we can register the class of the window
	/// This is how putty "talks" to pageant.
	/// This window will not actually be shown, just used to receive messages from clients.
	/// 
	/// Code based on http://stackoverflow.com/questions/128561/registering-a-custom-win32-window-class-from-c-sharp
	/// and Putty source code http://www.chiark.greenend.org.uk/~sgtatham/putty/download.html
	/// </summary>
	public class PageantWindow : IDisposable
	{
		#region /* constants */

		private const int ERROR_CLASS_ALREADY_EXISTS = 1410;
		private const string className = "Pageant";

		#endregion


		#region /* global variables */

		private bool disposed;
		private IntPtr hwnd;

		#endregion


		#region /* delegates */

		private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		#endregion


		#region /* structs */

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct WNDCLASS
		{
			public uint style;
			public WndProc lpfnWndProc;
			public int cbClsExtra;
			public int cbWndExtra;
			public IntPtr hInstance;
			public IntPtr hIcon;
			public IntPtr hCursor;
			public IntPtr hbrBackground;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpszMenuName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpszClassName;
		}

		#endregion


		#region /* externs */

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(String sClassName, String sAppName);

		/// See http://msdn.microsoft.com/en-us/library/windows/desktop/ms633586%28v=vs.85%29.aspx
		[DllImport("user32.dll", SetLastError = true)]
		private static extern System.UInt16 RegisterClassW([In] ref WNDCLASS lpWndClass);

		/// See http://msdn.microsoft.com/en-us/library/windows/desktop/ms632680%28v=vs.85%29.aspx
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr CreateWindowExW(
			 UInt32 dwExStyle,
			 [MarshalAs(UnmanagedType.LPWStr)]
       string lpClassName,
			 [MarshalAs(UnmanagedType.LPWStr)]
       string lpWindowName,
			 UInt32 dwStyle,
			 Int32 x,
			 Int32 y,
			 Int32 nWidth,
			 Int32 nHeight,
			 IntPtr hWndParent,
			 IntPtr hMenu,
			 IntPtr hInstance,
			 IntPtr lpParam
		);

		[DllImport("user32.dll", SetLastError = true)]
		static extern System.IntPtr DefWindowProcW(
				IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam
		);

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool DestroyWindow(IntPtr hWnd);

		#endregion


		#region /* constructors */

		/// <summary>
		/// Creates a new instance of PageantWindow. This window is not meant to be used for UI.
		/// 
		/// </summary>
		/// <exception cref="PageantException">Thrown when another instance of Pageant is running.</exception>
		public PageantWindow()
		{
			if (CheckAlreadyRunning()) {
				throw new PageantException();
			}

			// Create WNDCLASS
			WNDCLASS wind_class = new WNDCLASS();
			wind_class.lpszClassName = PageantWindow.className;
			wind_class.lpfnWndProc = CustomWndProc;

			UInt16 class_atom = RegisterClassW(ref wind_class);

			int last_error = Marshal.GetLastWin32Error();

			// TODO do we really need to worry about an error when regisering class?
			if (class_atom == 0 && last_error != PageantWindow.ERROR_CLASS_ALREADY_EXISTS) {
				throw new System.Exception("Could not register window class");
			}

			// Create window
			this.hwnd = CreateWindowExW(
					0, // dwExStyle
					PageantWindow.className, // lpClassName
					PageantWindow.className, // lpWindowName
					0, // dwStyle
					0, // x
					0, // y
					0, // nWidth
					0, // nHeight
					IntPtr.Zero, // hWndParent
					IntPtr.Zero, // hMenu
					IntPtr.Zero, // hInstance
					IntPtr.Zero // lpParam
			);
		}

		#endregion


		#region /* public methods */

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion


		#region /* private methods */

		private void Dispose(bool disposing)
		{
			if (!this.disposed) {
				if (disposing) {
					// Dispose managed resources
				}

				// Dispose unmanaged resources
				if (this.hwnd != IntPtr.Zero) {
					if (DestroyWindow(this.hwnd)) {
						this.hwnd = IntPtr.Zero;
						this.disposed = true;
					}
				}

			}
		}

		/// <summary>
		/// Checks to see if Pageant is already running
		/// </summary>
		/// <returns>true if Pageant is running</returns>
		private bool CheckAlreadyRunning()
		{
			IntPtr hwnd = FindWindow(PageantWindow.className, PageantWindow.className);
			return (hwnd != IntPtr.Zero);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="msg"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			if (msg == 0x004a) {
				MessageBox.Show("Copy Data Msg: " + lParam);
			}
			// TODO finish implement window messaging
			return DefWindowProcW(hWnd, msg, wParam, lParam);
		}

		#endregion
	}

}
