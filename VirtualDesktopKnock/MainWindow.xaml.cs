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
		private const double mouseInsideMargin = 25;
		private const double mouseOutsideMargin = 100;
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

			//this.Hide();
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
				this.vm.MousePosition = this.GetMousePosition();

				if (this.vm.MousePosition.X <= this.vm.ScreenBounds.X + mouseInsideMargin)
				{
					this.ksm.UpdateState(KnockStateMachine.MousePositions.Left);
				}
				else if (this.vm.MousePosition.X >= this.vm.ScreenBounds.X + this.vm.ScreenBounds.Width - mouseInsideMargin)
				{
					this.ksm.UpdateState(KnockStateMachine.MousePositions.Right);
				}
				else if (
					this.vm.MousePosition.X >= this.vm.ScreenBounds.X + (mouseOutsideMargin) &&
					this.vm.MousePosition.X <= this.vm.ScreenBounds.X + this.vm.ScreenBounds.Width - (mouseOutsideMargin))
				{

					this.ksm.UpdateState(KnockStateMachine.MousePositions.Outside);
				}

			}
			catch
			{
				Debug.WriteLine($"Error getting mouse position");
			}
		}

		private System.Windows.Point GetMousePosition()
		{
			return WinApi.GetCursorPosition();

			//// Gets a mouse point for the virtual device (e.g., doesn't map to real screen pixels but is modified based on the Display Settings Scaling (DPI))
			//var deviceIndependentPosition = System.Windows.Forms.Control.MousePosition;

			//// Use a transform to convert to virtual point to real screen coordinates (note, this is a global point across all monitors)
			////var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
			//var realScreenPosition = PointToScreen(new System.Windows.Point(deviceIndependentPosition.X, deviceIndependentPosition.Y));
			////var realScreenPosition = transform.Transform(new System.Windows.Point(deviceIndependentPosition.X, deviceIndependentPosition.Y));

			//return realScreenPosition;
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
	}
}
