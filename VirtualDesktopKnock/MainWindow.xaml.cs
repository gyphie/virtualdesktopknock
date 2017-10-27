using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Win32;
using WindowsDesktop;

namespace VirtualDesktopKnock
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const double mouseInsideMargin = 25;
		private const double mouseOutsideMargin = 150;
		private const double mouseTopMargin = 50;
		private const double mouseBottomMargin = 50;
		private DispatcherTimer mouseTimer;
		private MainWindowViewModel vm;
		private KnockStateMachine ksm;

		public MainWindow()
		{
			InitializeComponent();

			this.vm = new MainWindowViewModel();
			this.vm.ScreenBounds = this.GetScreenSize();
			this.vm.VirtualDesktopNumber = VirtualDesktop.Current.Id;
			this.DataContext = this.vm;

			this.ksm = KnockStateMachine.GetMachine();
			this.ksm.OnKnock += Ksm_OnKnock;

			this.mouseTimer = new DispatcherTimer();
			this.mouseTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
			this.mouseTimer.Tick += MouseTimer_Tick;
			this.mouseTimer.Start();

			SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			this.ShowInTaskbar = false;
			this.Hide();
		}

		private void MouseTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				this.vm.MousePosition = WinApi.GetCursorPosition();

				if (this.vm.MousePosition.Y < mouseTopMargin || this.vm.MousePosition.Y > (this.vm.ScreenBounds.Height - mouseBottomMargin))
				{
					return; // Mouse was along the top or along the bottom and because there are many windows controls and the start bar in these regions we don't want to knock.
				}


				// Left
				if (this.vm.MousePosition.X <= this.vm.ScreenBounds.X + mouseInsideMargin)
				{
					this.ksm.UpdateState(KnockStateMachine.MousePositions.Left);
				}
				// Right
				else if (this.vm.MousePosition.X >= this.vm.ScreenBounds.X + this.vm.ScreenBounds.Width - mouseInsideMargin)
				{
					this.ksm.UpdateState(KnockStateMachine.MousePositions.Right);
				}
				// Middle
				else if (
					this.vm.MousePosition.X >= this.vm.ScreenBounds.X + mouseOutsideMargin &&
					this.vm.MousePosition.X <= this.vm.ScreenBounds.X + this.vm.ScreenBounds.Width - mouseOutsideMargin)
				{

					this.ksm.UpdateState(KnockStateMachine.MousePositions.Outside);
				}

			}
			catch
			{
				Debug.WriteLine($"Error getting mouse position");
			}
		}

		private System.Windows.Rect GetScreenSize()
		{
			return new Rect(0, 0, SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
		}

		private void Ksm_OnKnock(object sender, KnockStateMachine.KnockEventArgs e)
		{
			var currentVD = VirtualDesktop.Current;
			var nextDesktop = e.Side == KnockStateMachine.KnockEventArgs.Sides.Left ? currentVD.GetLeft() : currentVD.GetRight();
			if (nextDesktop != null)
			{
				WinApi.UnfocusForegroundWindow();
				nextDesktop.Switch();
				this.vm.VirtualDesktopNumber = VirtualDesktop.Current.Id;
			}
		}

		private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			this.vm.ScreenBounds = this.GetScreenSize();
		}


	}
}
