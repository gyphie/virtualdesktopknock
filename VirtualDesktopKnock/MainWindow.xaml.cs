using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WindowsDesktop;

namespace VirtualDesktopKnock
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private DispatcherTimer mouseTimer;
		private MainWindowViewModel vm;

		public MainWindow()
		{
			InitializeComponent();

			this.vm = new MainWindowViewModel();
			this.vm.ScreenBounds = this.GetScreenSize();
			this.vm.VirtualDesktopNumber = VirtualDesktop.Current.Id;
			this.DataContext = this.vm;

			this.mouseTimer = new DispatcherTimer();
			this.mouseTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
			this.mouseTimer.Tick += MouseTimer_Tick;
			this.mouseTimer.Start();
		}

		private void MouseTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				this.vm.MousePosition = this.GetMousePosition();
				this.vm.VirtualDesktopNumber = VirtualDesktop.Current.Id;
				this.Move();
			}
			catch
			{
				Debug.WriteLine($"Error getting mouse position");
			}
		}

		private System.Windows.Point GetMousePosition()
		{
			// Gets a mouse point for the virtual device (e.g., doesn't map to real screen pixels but is modified based on the Display Settings Scaling (DPI))
			var deviceIndependentPosition = System.Windows.Forms.Control.MousePosition;

			// Use a transform to convert to virtual point to real screen coordinates (note, this is a global point across all monitors)
			var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
			var realScreenPosition = transform.Transform(new System.Windows.Point(deviceIndependentPosition.X, deviceIndependentPosition.Y));

			return realScreenPosition;
		}

		private System.Windows.Rect GetScreenSize()
		{
			return new Rect(0, 0, SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
		}

		private void Move()
		{
			var currentVD = VirtualDesktop.Current;
			var margin = 5;

			if (this.vm.MousePosition.X <= this.vm.ScreenBounds.X + margin)
			{
				var leftDesktop = currentVD.GetLeft();
				if (leftDesktop != null)
				{
					WinApi.UnfocusForegroundWindow();
					leftDesktop.Switch();
					WinApi.SetCursorPos((int)this.vm.ScreenBounds.X + (int)this.vm.ScreenBounds.Width - margin - 1, (int)this.vm.MousePosition.Y);
				}
			}
			else if (this.vm.MousePosition.X >= this.vm.ScreenBounds.X + this.vm.ScreenBounds.Width - margin)
			{
				var rightDesktop = currentVD.GetRight();
				if (rightDesktop != null)
				{
					WinApi.UnfocusForegroundWindow();
					rightDesktop.Switch();
					WinApi.SetCursorPos((int)this.vm.ScreenBounds.X + margin - 1, (int)this.vm.MousePosition.Y);
				}
			}

			this.vm.VirtualDesktopNumber = VirtualDesktop.Current.Id;
		}
	}
}
