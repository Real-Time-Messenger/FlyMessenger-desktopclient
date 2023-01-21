using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Drawing.Color;
using FontStyle = System.Drawing.FontStyle;
using Forms = System.Windows.Forms;

namespace FlyMessenger.Core.Utils
{
    public class NotifyIconManager
    {
        /// <summary>
        /// Private slots for the notify icon and the context menu
        /// </summary>
        private readonly Forms.NotifyIcon _notifyIcon;

        private readonly Forms.ToolStripMenuItem _exit;
        private readonly Forms.ToolStripMenuItem _notifications;
        private readonly Forms.ToolStripMenuItem _showWindow;

        /// <summary>
        /// Constructor
        /// </summary>
        public NotifyIconManager()
        {
            _notifyIcon = new Forms.NotifyIcon();
            _exit = new Forms.ToolStripMenuItem();
            _notifications = new Forms.ToolStripMenuItem();
            _showWindow = new Forms.ToolStripMenuItem();
        }

        // Logic to NotifyIconManager
        public void InitializeNotifyIcon()
        {
            // Create the context menu
            var contextMenu = new Forms.ContextMenuStrip();

            // Initialize the context menu
            _notifyIcon.ContextMenuStrip = contextMenu;

            contextMenu.Items.Add(_showWindow);
            contextMenu.Items.Add(_notifications);
            contextMenu.Items.Add(_exit);

            _notifyIcon.Icon = new Icon("Public/Icons/Logo16.ico");
            _notifyIcon.Text = @"FlyMessenger";
            _notifyIcon.Visible = true;
            _notifyIcon.MouseUp += NotifyIcon_Click;

            // Set the menu items
            // Create manual translation
            _exit.Text = Resources.Languages.lang.exit;
            _exit.Click += Exit_Click;

            _notifications.Text = Resources.Languages.lang.notifications;
            _notifications.Click += Notifications_Click;

            _showWindow.Text = Resources.Languages.lang.showWindow;
            _showWindow.Click += ShowWindow_Click;

            // contextMenuStrip settings
            contextMenu.ShowImageMargin = false;
            contextMenu.ShowCheckMargin = false;
            contextMenu.ShowItemToolTips = false;
            contextMenu.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            foreach (Forms.ToolStripMenuItem item in contextMenu.Items)
            {
                item.AutoSize = false;
                item.Height = 32;
            }
            contextMenu.RenderMode = Forms.ToolStripRenderMode.Professional;
            contextMenu.Renderer = new CustomRenderer();
        }

        // Exit
        private void Exit_Click(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();

            // Dispose the notify icon
            _notifyIcon.Dispose();
        }

        // OFF/ON notifications
        private static void Notifications_Click(object? sender, EventArgs e)
        {
            // TODO: Create notification logic
        }

        // Open/close FlyMessenger
        private static void ShowWindow_Click(object? sender, EventArgs e)
        {
            Application.Current.MainWindow?.Show();
            Application.Current.MainWindow?.Activate();
        }

        // Logic for click to NotifyIcon
        private static void NotifyIcon_Click(object? sender, Forms.MouseEventArgs e)
        {
            if (e.Button != Forms.MouseButtons.Left) return;
            if (Application.Current.MainWindow?.Visibility == Visibility.Visible)
            {
                Application.Current.MainWindow?.Hide();
            }
            else
            {
                Application.Current.MainWindow?.Show();
                Application.Current.MainWindow?.Activate();
            }

            // WindowState
            if (Application.Current.MainWindow?.WindowState == WindowState.Minimized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
        }
    }

    public class CustomRenderer : Forms.ToolStripRenderer
    {
        // Change background color of items when mouse hover
        protected override void OnRenderMenuItemBackground(Forms.ToolStripItemRenderEventArgs e)
        {
            if (Application.Current.Resources.MergedDictionaries[0].Source.ToString().Contains("Light"))
            {
                e.Item.ForeColor = Color.FromArgb(16, 16, 16);
                e.Graphics.FillRectangle(
                    e.Item.Selected
                        ? new SolidBrush(Color.FromArgb(224, 227, 240))
                        : new SolidBrush(Color.FromArgb(234, 237, 250)),
                    e.Item.ContentRectangle
                );
            }
            else
            {
                e.Item.ForeColor = Color.FromArgb(184, 186, 242);
                e.Graphics.FillRectangle(
                    e.Item.Selected
                        ? new SolidBrush(Color.FromArgb(47, 56, 78))
                        : new SolidBrush(Color.FromArgb(16, 24, 43)),
                    e.Item.ContentRectangle
                );
            }
        }
        
        // Change background color of contextMenu
        protected override void OnRenderToolStripBackground(Forms.ToolStripRenderEventArgs e)
        {
            e.Graphics.FillRectangle(
                Application.Current.Resources.MergedDictionaries[0].Source.ToString().Contains("Light")
                    ? new SolidBrush(Color.FromArgb(234, 237, 250))
                    : new SolidBrush(Color.FromArgb(16, 24, 43)),
                e.AffectedBounds
            );
        }
    }
}
