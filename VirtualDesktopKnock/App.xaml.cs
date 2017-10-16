using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VirtualDesktopKnock
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private System.ComponentModel.Container components;
		private System.Windows.Forms.NotifyIcon notifyIcon;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			this.components = new System.ComponentModel.Container();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);

			var exitMenu = new System.Windows.Forms.MenuItem("Exit");
			exitMenu.Index = 0;
			exitMenu.Click += NotifyIconContextMenuExitItem_Click;

			var contextMenu = new System.Windows.Forms.ContextMenu();
			contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { exitMenu });
			this.notifyIcon.ContextMenu = contextMenu;

			this.notifyIcon.Icon = new System.Drawing.Icon(VirtualDesktopKnock.Properties.Resources.TrayIcon, System.Windows.Forms.SystemInformation.SmallIconSize);
			this.notifyIcon.Text = "Virtual Desktop Knock";
			this.notifyIcon.Visible = true;
		}

		private void NotifyIconContextMenuExitItem_Click(object sender, EventArgs e)
		{
			this.MainWindow.Close();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			this.components?.Dispose();
		}
	}
}
