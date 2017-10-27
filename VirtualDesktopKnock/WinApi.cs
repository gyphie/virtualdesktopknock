using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VirtualDesktopKnock
{
	public static class WinApi
	{
		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow(); // Note: Use of this is generally bad as it gets the Window of other applications (not your own) so you end up mucking with the behavior of other applications which users won't like
		[DllImport("user32.dll")]
		static extern bool GetCursorPos(out POINT lpPoint);
		[DllImport("User32.dll")]
		static extern bool SetCursorPos(int X, int Y);
		[DllImport("User32.dll")]
		static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);


		const int WM_KILLFOCUS = 0x0008;

		public static void UnfocusForegroundWindow()
		{
			SendMessage(GetForegroundWindow(), WM_KILLFOCUS, IntPtr.Zero, IntPtr.Zero);
		}

		/// <summary>
		/// Struct representing a point.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public static implicit operator Point(POINT point)
			{
				return new Point(point.X, point.Y);
			}
		}

		/// <summary>
		/// Retrieves the cursor's position, in screen coordinates.
		/// </summary>
		/// <see>See MSDN documentation for further information.</see>
		public static Point GetCursorPosition()
		{
			POINT lpPoint;
			WinApi.GetCursorPos(out lpPoint);

			return lpPoint;
		}
	}
}
