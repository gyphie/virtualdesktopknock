using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VirtualDesktopKnock
{
	public static class WinApi
	{
		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();	// Note: Use of this is generally bad as it gets the Window of other applications (not your own) so you end up mucking with the behavior of other applications which users won't like
		[DllImport("User32.dll")]
		public static extern bool SetCursorPos(int X, int Y);
		[DllImport("User32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);


		const int WM_KILLFOCUS = 0x0008;

		public static void UnfocusForegroundWindow()
		{
			SendMessage(GetForegroundWindow(), WM_KILLFOCUS, IntPtr.Zero, IntPtr.Zero);
		}
	}
}
