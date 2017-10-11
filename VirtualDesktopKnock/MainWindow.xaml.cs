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
			this.DataContext = this.vm;

			this.mouseTimer = new DispatcherTimer();
			this.mouseTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
			this.mouseTimer.Tick += MouseTimer_Tick;
			this.mouseTimer.Start();
		}

		private void MouseTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				this.vm.MousePosition = this.GetMousePosition();
				//Debug.WriteLine($"{mp.X}, {mp.Y}");
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
		
	}
}
