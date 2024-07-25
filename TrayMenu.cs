using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Mirador
{
    public class TrayMenu
    {
        public NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;
        private Image _titleBackgroundImageLight;
        private Image _titleBackgroundImageDark;
        private ToolStripMenuItem _titleMenuItem;
        public SettingsForm _settingsForm;

        // Initialize system tray icon and context menu
        public void InitializeTrayIcon()
        {
            try
            {
                _notifyIcon = new NotifyIcon();
                _contextMenu = new ContextMenuStrip();

                _notifyIcon.MouseDown += NotifyIcon_Click;

                // Load the background images for the title
                _titleBackgroundImageLight = Properties.Resources.Tray_Menu_Bar_Light;
                _titleBackgroundImageDark = Properties.Resources.Tray_Menu_Bar_Dark;

                // Title
                _titleMenuItem = new ToolStripMenuItem("Mirador");
                _titleMenuItem.Enabled = false;
                _titleMenuItem.Paint += TitleMenuItem_Paint;
                _contextMenu.Items.Add(_titleMenuItem);

                // Moved access to the settings form from the context menu to the tray icon left click
                //var settingsMenuItem = new ToolStripMenuItem("Settings", null, OnSettingsMenuItemClick);
                //_contextMenu.Items.Add(settingsMenuItem);

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
                Console.WriteLine("Tray icon initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing tray icon: {ex.Message}");
            }
        }

        private void TitleMenuItem_Paint(object sender, PaintEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TitleMenuItem_Paint: {ex.Message}");
            }
        }

        private bool _notifyIconClicked = false;
        private Point _lastMousePosition;
        private const int ClickTolerance = 20; // Defines a tolerance for mouse movement

        private void NotifyIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !_notifyIconClicked)
            {
                _notifyIconClicked = true;
                _lastMousePosition = Control.MousePosition;

                Point mousePosition = Control.MousePosition;
                Console.WriteLine($"Mouse position: {mousePosition}");

                if (_settingsForm == null || _settingsForm.IsDisposed)
                {
                    _settingsForm = new SettingsForm();
                    PositionFormAboveMouse(_settingsForm, mousePosition);
                    _settingsForm.Show();
                    _settingsForm.BringToFront();
                    _settingsForm.TopMost = true;
                }
                else
                {
                    _settingsForm.Close();
                    _settingsForm = null;
                }

                _notifyIconClicked = false;
            }
        }

        public void CheckClickOutsideForm(Point currentPos)
        {
            // Check if the mouse is still close to the position it was when the icon was clicked
            // This is to prevent the form from rapidly closing and opening when the user clicks the icon again
            // Hacky solution, but it works for now

            if (Math.Abs(currentPos.X - _lastMousePosition.X) < ClickTolerance &&
                Math.Abs(currentPos.Y - _lastMousePosition.Y) < ClickTolerance)
            {
                Console.WriteLine("Mouse is still close to the notify icon position, bypassing outside click check.");
                return;
            }

            // Check if the click was outside the settings form
            if (_settingsForm != null && _settingsForm.Visible)
            {
                Rectangle formRectangle = new Rectangle(_settingsForm.Location, _settingsForm.Size);
                if (!formRectangle.Contains(currentPos))
                {
                    Settings.Current.Save();
                    _settingsForm.Close();
                    _settingsForm = null;
                    Console.WriteLine("Closed settings form because click was outside.");
                }
            }
        }

        // Hacky solution to open the form at icon position, not precise
        public void PositionFormAboveMouse(Form form, Point position)
        {
            try
            {
                Rectangle? taskbarRect = Taskbar.GetTaskbarPositionAndSize();
                if (taskbarRect.HasValue)
                {
                    int taskbarTop = taskbarRect.Value.Top;

                    // Calculate the desired location for the form
                    int formX = position.X - form.Width / 2;
                    int formY = position.Y - form.Height;

                    // Ensure the form is above the taskbar
                    if (formY < taskbarTop)
                    {
                        formY = taskbarTop - form.Height;
                    }

                    form.StartPosition = FormStartPosition.Manual;
                    form.Location = new Point(formX, formY);
                    Console.WriteLine($"Form positioned at: {form.Location}");
                }
                else
                {
                    Console.WriteLine("Taskbar not found, positioning form above the mouse without taskbar adjustment.");
                    form.StartPosition = FormStartPosition.Manual;
                    form.Location = new Point(position.X - form.Width / 2, position.Y - form.Height);
                    Console.WriteLine($"Form positioned at: {form.Location}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PositionFormAboveMouse: {ex.Message}");
            }
        }

        private void OnSettingsMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                using (SettingsForm settingsForm = new SettingsForm())
                {
                    settingsForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening Settings form: {ex.Message}");
            }
        }

        private void OnDonateMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                string donateUrl = "todo";
                Process.Start(new ProcessStartInfo
                {
                    FileName = donateUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening Donate URL: {ex.Message}");
            }
        }

        private void OnAboutMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                string githubUrl = "https://github.com/norton198/Mirador";
                Process.Start(new ProcessStartInfo
                {
                    FileName = githubUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening About URL: {ex.Message}");
            }
        }

        private void OnExitMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exiting application: {ex.Message}");
            }
        }

        private void ApplyTheme()
        {
            try
            {
                bool isDarkMode = IsDarkModeEnabled();
                Color backColor = isDarkMode ? Color.FromArgb(44, 44, 44) : Color.White;
                Color foreColor = isDarkMode ? Color.White : Color.Black;

                _contextMenu.BackColor = backColor;
                _contextMenu.ForeColor = foreColor;
                _contextMenu.Renderer = new CustomToolStripRenderer(isDarkMode);

                // Update title menu item to force a repaint with the new theme
                _titleMenuItem.Invalidate();
                Console.WriteLine("Theme applied successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying theme: {ex.Message}");
            }
        }

        private bool IsDarkModeEnabled()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking dark mode status: {ex.Message}");
                return false; // Default to light mode in case of error
            }
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
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
