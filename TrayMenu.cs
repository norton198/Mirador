using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Mirador
{
    internal class TrayMenu
    {
        public static NotifyIcon _notifyIcon;
        private static ContextMenuStrip _contextMenu;
        private static Image _titleBackgroundImageLight;
        private static Image _titleBackgroundImageDark;
        private static ToolStripMenuItem _titleMenuItem;

        // Initialize system tray icon and context menu
        public static void InitializeTrayIcon()
        {
            _notifyIcon = new NotifyIcon();
            _contextMenu = new ContextMenuStrip();

            // Load the background images for the title
            _titleBackgroundImageLight = Properties.Resources.Tray_Menu_Bar_Light;
            _titleBackgroundImageDark = Properties.Resources.Tray_Menu_Bar_Dark;

            // Title
            _titleMenuItem = new ToolStripMenuItem("Mirador");
            _titleMenuItem.Enabled = false;
            _titleMenuItem.Paint += TitleMenuItem_Paint;
            _contextMenu.Items.Add(_titleMenuItem);

            var settingsMenuItem = new ToolStripMenuItem("Settings", null, OnSettingsMenuItemClick);
            _contextMenu.Items.Add(settingsMenuItem);

            var donateMenuItem = new ToolStripMenuItem("Donate", null, OnDonateMenuItemClick);
            _contextMenu.Items.Add(donateMenuItem);

            var aboutMenuItem = new ToolStripMenuItem("About", null, OnAboutMenuItemClick);
            _contextMenu.Items.Add(aboutMenuItem);

            var exitMenuItem = new ToolStripMenuItem("Exit", null, OnExitMenuItemClick);
            _contextMenu.Items.Add(exitMenuItem);

            _notifyIcon.Text = "Mirador";
            _notifyIcon.Icon = Properties.Resources.Tray_Icon;
            _notifyIcon.ContextMenuStrip = _contextMenu;
            _notifyIcon.Visible = true;

            ApplyTheme();

            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        }

        private static void TitleMenuItem_Paint(object sender, PaintEventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                bool isDarkMode = IsDarkModeEnabled();
                Image backgroundImage = isDarkMode ? _titleBackgroundImageDark : _titleBackgroundImageLight;
                Color textColor = isDarkMode ? Color.White : Color.Black;

                if (backgroundImage != null)
                {
                    // Draw the background image
                    e.Graphics.DrawImage(backgroundImage, e.ClipRectangle);
                }
                else
                {
                    Console.WriteLine("Background image is null. Drawing background color instead.");
                    e.Graphics.FillRectangle(new SolidBrush(isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White), e.ClipRectangle);

                    // Draw the text
                    TextRenderer.DrawText(e.Graphics, menuItem.Text, menuItem.Font, e.ClipRectangle, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
        }

        private static void OnSettingsMenuItemClick(object sender, EventArgs e)
        {
            using (SettingsForm settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog();
            }
        }

        private static void OnDonateMenuItemClick(object sender, EventArgs e)
        {
            string donateUrl = "todo";
            Process.Start(new ProcessStartInfo
            {
                FileName = donateUrl,
                UseShellExecute = true
            });
        }

        private static void OnAboutMenuItemClick(object sender, EventArgs e)
        {
            string githubUrl = "https://github.com/norton198/Mirador";
            Process.Start(new ProcessStartInfo
            {
                FileName = githubUrl,
                UseShellExecute = true
            });
        }

        private static void OnExitMenuItemClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private static void ApplyTheme()
        {
            bool isDarkMode = IsDarkModeEnabled();
            Color backColor = isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White;
            Color foreColor = isDarkMode ? Color.White : Color.Black;

            _contextMenu.BackColor = backColor;
            _contextMenu.ForeColor = foreColor;
            _contextMenu.Renderer = new CustomToolStripRenderer(isDarkMode);

            // Update title menu item to force a repaint with the new theme
            _titleMenuItem.Invalidate();
        }

        private static bool IsDarkModeEnabled()
        {
            int lightTheme = 0;
            if (Environment.OSVersion.Version.Build >= 17763)
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        var registryValueObject = key.GetValue("AppsUseLightTheme");
                        if (registryValueObject != null)
                        {
                            lightTheme = (int)registryValueObject;
                        }
                    }
                }
            }
            return lightTheme == 0;
        }

        private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                ApplyTheme();
            }
        }
    }

    internal class CustomToolStripRenderer : ToolStripProfessionalRenderer
    {
        private bool isDarkMode;

        public CustomToolStripRenderer(bool isDarkMode) : base(new CustomColorTable(isDarkMode))
        {
            this.isDarkMode = isDarkMode;
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderMenuItemBackground(e);
            ToolStripMenuItem menuItem = e.Item as ToolStripMenuItem;
            if (menuItem != null)
            {
                Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
                Color backColor = e.Item.Selected ? (isDarkMode ? Color.FromArgb(62, 62, 64) : Color.FromArgb(210, 210, 210)) : (isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White);
                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBackground(e);
            Color backColor = isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White;
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {

        }
    }

    internal class CustomColorTable : ProfessionalColorTable
    {
        private bool isDarkMode;

        public CustomColorTable(bool isDarkMode)
        {
            this.isDarkMode = isDarkMode;
        }

        public override Color ToolStripDropDownBackground => isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White;
        public override Color MenuBorder => isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White;
        public override Color MenuItemBorder => isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White;
        public override Color MenuItemSelected => isDarkMode ? Color.FromArgb(62, 62, 64) : Color.FromArgb(210, 210, 210);
        public override Color MenuStripGradientBegin => isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White;
        public override Color MenuStripGradientEnd => isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White;
        public override Color MenuItemSelectedGradientBegin => isDarkMode ? Color.FromArgb(62, 62, 64) : Color.FromArgb(210, 210, 210);
        public override Color MenuItemSelectedGradientEnd => isDarkMode ? Color.FromArgb(62, 62, 64) : Color.FromArgb(210, 210, 210);
        public override Color MenuItemPressedGradientBegin => isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White;
        public override Color MenuItemPressedGradientEnd => isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White;
    }
}
