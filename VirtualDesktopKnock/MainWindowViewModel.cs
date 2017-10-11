using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VirtualDesktopKnock
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel()
		{
			this.MousePosition = new Point(0, 0);
			this.ScreenNumber = 0;
			this.VirtualDesktopNumber = 0;
			this.ScreenBounds = new Rect();
		}

		private Point mousePosition;
		public Point MousePosition
		{
			get
			{
				return this.mousePosition;
			}
			set
			{
				this.mousePosition = value;
				this.OnPropertyChanged("MousePositionLabel");
			}
		}

		private Rect screenBounds;
		public Rect ScreenBounds
		{
			get { }
			set { }
		}
		public string ScreenBoundsLabel { get { return $"{this.ScreenBounds.Width}, {this.ScreenBounds.Height}"; }}

		public int ScreenNumber { get; set; }
		public int VirtualDesktopNumber { get; set; }

		public string MousePositionLabel
		{
			get
			{
				return $"{this.MousePosition.X}, {this.MousePosition.Y}";
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
