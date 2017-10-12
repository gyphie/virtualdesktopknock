using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VirtualDesktopKnock
{
	public class KnockStateMachine
	{
		private DispatcherTimer stateTimer;
		private const long InsideTimeout = TimeSpan.TicksPerSecond;
		private const long OutsideTimeout = TimeSpan.TicksPerSecond;

		private KnockStateMachine() {
			this.State = States.Unknown;
			this.Ticks = DateTime.Now.Ticks;

			this.stateTimer = new DispatcherTimer();
			this.stateTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
			this.stateTimer.Tick += StateTimer_Tick;
			this.stateTimer.Start();
		}

		private void StateTimer_Tick(object sender, EventArgs e)
		{
			var now = DateTime.Now.Ticks;

			switch (this.State)
			{
				case States.Unknown:
					return;	// No transitions
				case States.Outside1:
					this.Ticks = now;
					return;	// No transitions
				case States.InsideLeft1:
				case States.InsideRight1:
					if (now - this.Ticks > KnockStateMachine.InsideTimeout)
					{
						this.State = States.Unknown;
						this.Ticks = now;
					}
					return;
				case States.OutsideLeft2:
				case States.OutsideRight2:
					if (now - this.Ticks > KnockStateMachine.OutsideTimeout)
					{
						this.State = States.Unknown;
						this.Ticks = now;
					}
					return;
				case States.InsideLeft2:
					this.State = States.Unknown;
					this.Ticks = now;
					this.RaiseKnockEvent(new KnockEventArgs(KnockEventArgs.Sides.Left));
					break;
				case States.InsideRight2:
					this.State = States.Unknown;
					this.Ticks = now;
					this.RaiseKnockEvent(new KnockEventArgs(KnockEventArgs.Sides.Right));
					break;
				default:
					this.State = States.Unknown;
					this.Ticks = now;
					break;
			}
		}

		private static KnockStateMachine singleton;
		public static KnockStateMachine GetMachine()
		{
			if (KnockStateMachine.singleton == null)
			{
				KnockStateMachine.singleton = new KnockStateMachine();
			}

			return KnockStateMachine.singleton;
		}

		private States State { get; set; }
		private long Ticks { get; set; }

		public void UpdateState(MousePositions mousePosition)
		{
			var now = DateTime.Now.Ticks;
			switch (this.State)
			{
				case States.Unknown:
					if (mousePosition == MousePositions.Outside)
					{
						this.State = States.Outside1;
						this.Ticks = now;
					}
					break;
				case States.Outside1:
					if (mousePosition == MousePositions.Left)
					{
						this.State = States.InsideLeft1;
						this.Ticks = now;
					}
					else if (mousePosition == MousePositions.Right)
					{
						this.State = States.InsideRight1;
						this.Ticks = now;
					}
					break;
				case States.InsideLeft1:
					if (mousePosition == MousePositions.Outside)
					{
						this.State = States.OutsideLeft2;
						this.Ticks = now;
					}
					else if (mousePosition == MousePositions.Right)
					{
						this.State = States.Unknown;
						this.Ticks = now;
					}
					break;
				case States.InsideRight1:
					if (mousePosition == MousePositions.Outside)
					{
						this.State = States.OutsideRight2;
						this.Ticks = now;
					}
					else if (mousePosition == MousePositions.Left)
					{
						this.State = States.Unknown;
						this.Ticks = now;
					}
					break;
				case States.OutsideLeft2:
					if (mousePosition == MousePositions.Left)
					{
						this.State = States.InsideLeft2;
						this.Ticks = now;
					}
					else if (mousePosition == MousePositions.Right)
					{
						this.State = States.InsideRight1;
						this.Ticks = now;
					}
					break;
				case States.OutsideRight2:
					if (mousePosition == MousePositions.Left)
					{
						this.State = States.InsideLeft1;
						this.Ticks = now;
					}
					else if (mousePosition == MousePositions.Right)
					{
						this.State = States.InsideRight2;
						this.Ticks = now;
					}
					break;
				case States.InsideLeft2:
					// Allow the timer to resolve the state (Knock Left)
					break;
				case States.InsideRight2:
					// Allow the timer to resolve the state (Knock Right)
					break;
				default:
					// Allow the timer to resolve the state (reset to Unknown)
					break;
			}
		}

		public event EventHandler<KnockEventArgs> OnKnock;

		protected virtual void RaiseKnockEvent(KnockEventArgs e)
		{
			this.OnKnock?.Invoke(this, e);
		}

		public enum States
		{
			Unknown,
			Outside1,
			InsideLeft1,
			InsideRight1,
			OutsideLeft2,
			OutsideRight2,
			InsideLeft2,
			InsideRight2
		}

		public enum MousePositions
		{
			Left,
			Outside,
			Right
		}

		public class KnockEventArgs : EventArgs
		{
			public KnockEventArgs(Sides side)
			{
				this.Side = side;
			}
			public Sides Side { get; private set; }

			public enum Sides
			{
				Left,
				Right
			}
		}
	}
}
