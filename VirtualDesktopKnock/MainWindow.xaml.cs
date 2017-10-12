using System;
using System.Diagnostics;
using System.Windows;
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
				this.CheckLeftKnock();
				this.CheckRightKnock();
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

		private void CheckLeftKnock()
		{
			var margin = 25;
			var inTimeout = TimeSpan.TicksPerSecond / 2;
			var outTimeout = TimeSpan.TicksPerSecond;

			var knock = this.vm.KnockLeftHistory.PeekOrDefault();
			var tickTime = DateTime.Now.Ticks;

			// Expire any old knocks
			if (knock != null && (tickTime - knock.InTime > inTimeout || (knock.OutTime != 0 && tickTime - knock.OutTime > outTimeout)))
			{
				this.vm.KnockLeftHistory.PopOrDefault();
				this.vm.OnPropertyChanged("KnockHistoryLabel");
				knock = null;
			}

			if (this.vm.MousePosition.X <= this.vm.ScreenBounds.X + margin)
			{
				if (knock == null || knock.OutTime != 0)
				{
					knock = new Knock(tickTime, 0);
					this.vm.KnockLeftHistory.Push(knock);
					this.vm.OnPropertyChanged("KnockHistoryLabel");
				}

			}
			else
			{
				if (knock != null && knock.OutTime == 0)
				{
					knock.OutTime = tickTime;
					this.vm.OnPropertyChanged("KnockHistoryLabel");
				}
			}

			// Check for knock-knock
			if (this.vm.KnockLeftHistory.Count >= 2)
			{
				this.vm.KnockLeftHistory.Clear();
				this.vm.OnPropertyChanged("KnockHistoryLabel");

				var currentVD = VirtualDesktop.Current;
				var leftDesktop = currentVD.GetLeft();
				if (leftDesktop != null)
				{
					WinApi.UnfocusForegroundWindow();
					leftDesktop.Switch();
					//WinApi.SetCursorPos((int)this.vm.ScreenBounds.X + (int)this.vm.ScreenBounds.Width - margin - 1, (int)this.vm.MousePosition.Y);

					this.vm.VirtualDesktopNumber = VirtualDesktop.Current.Id;
				}
			}
			
		}

		private void CheckRightKnock()
		{
			var margin = 25;
			var inTimeout = TimeSpan.TicksPerSecond / 2;
			var outTimeout = TimeSpan.TicksPerSecond;

			var knock = this.vm.KnockRightHistory.PeekOrDefault();
			var tickTime = DateTime.Now.Ticks;

			// Expire any old knocks
			if (knock != null && (tickTime - knock.InTime > inTimeout || (knock.OutTime != 0 && tickTime - knock.OutTime > outTimeout)))
			{
				this.vm.KnockRightHistory.PopOrDefault();
				this.vm.OnPropertyChanged("KnockHistoryLabel");
				knock = null;
			}

			if (this.vm.MousePosition.X >= this.vm.ScreenBounds.X + this.vm.ScreenBounds.Width - margin)
			{
				if (knock == null || knock.OutTime != 0)
				{
					knock = new Knock(tickTime, 0);
					this.vm.KnockRightHistory.Push(knock);
					this.vm.OnPropertyChanged("KnockHistoryLabel");
				}

			}
			else
			{
				if (knock != null && knock.OutTime == 0)
				{
					knock.OutTime = tickTime;
					this.vm.OnPropertyChanged("KnockHistoryLabel");
				}
			}

			// Check for knock-knock
			if (this.vm.KnockRightHistory.Count >= 2)
			{
				this.vm.KnockRightHistory.Clear();
				this.vm.OnPropertyChanged("KnockHistoryLabel");

				var currentVD = VirtualDesktop.Current;
				var rightDesktop = currentVD.GetRight();
				if (rightDesktop != null)
				{
					WinApi.UnfocusForegroundWindow();
					rightDesktop.Switch();
					this.vm.VirtualDesktopNumber = VirtualDesktop.Current.Id;
					//WinApi.SetCursorPos((int)this.vm.ScreenBounds.X + (int)this.vm.ScreenBounds.Width - margin - 1, (int)this.vm.MousePosition.Y);
				}
			}

		}

	}
}
