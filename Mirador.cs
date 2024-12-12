using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mirador.RawInput;
using static NativeMethods;
using Timer = System.Windows.Forms.Timer;

namespace Mirador
{
    public class Mirador
    {

        private NotifyIcon notifyIcon;

        private int _lastClickTime;
        private Point _lastClickPosition;
        private readonly int DoubleClickTime = GetDoubleClickTime();
        private readonly int DoubleClickDistance = GetSystemMetrics(SystemMetric.SM_CXDOUBLECLK);

        public static RawInput rawInput;
        public static TrayMenu trayMenu;

        private Timer _autoHideTaskbarTimer;
        private readonly int TimerInterval = 1000;

        public void Initialize()
        {
            // Initialize Tray Icon
            trayMenu = new TrayMenu();
            trayMenu.InitializeTrayIcon();

            // Create a hidden form to handle raw input
            var form = new HiddenForm();
            rawInput = new RawInput();
            rawInput.RegisterRawInput(form.Handle);
            rawInput.MouseMoved += RawInput_MouseMoved;
            rawInput.LeftButtonUp += RawInput_LeftButtonUp;

            // Load settings from file or create default settings if file doesn't exist
            Settings.Load();

            // Initialize and start the auto-hide taskbar timer
            _autoHideTaskbarTimer = new Timer();
            _autoHideTaskbarTimer.Interval = TimerInterval;
            _autoHideTaskbarTimer.Tick += (sender, args) =>
            {
                if (Taskbar.IsTaskbarVisible() && Settings.Current.AutoHide)
                {
                    NativeMethods.GetCursorPos(out Point point);
                    Point currentPos = new Point(point.X, point.Y);
                    Taskbar.AutoHideTaskbar(currentPos);
                }
            };
            _autoHideTaskbarTimer.Start();
            Console.WriteLine("Auto-hide taskbar timer started.");
        }

        private void RawInput_MouseMoved(object sender, RawMouseEventArgs e)
        {
            OnMouseMove();
        }

        private void RawInput_LeftButtonUp(object sender, RawMouseEventArgs e)
        {
            OnMouseButtonUp(e);
        }

        private void OnMouseButtonUp(RawMouseEventArgs e)
        {
            Console.WriteLine("Left button up.");
            int currentTime = Environment.TickCount;

            NativeMethods.GetCursorPos(out Point point);

            Point currentPos = new Point(point.X, point.Y);

            Console.WriteLine("Current position: " + currentPos.X + ", " + currentPos.Y);
            if (currentTime - _lastClickTime <= DoubleClickTime &&
                Math.Abs(currentPos.X - _lastClickPosition.X) <= DoubleClickDistance &&
                Math.Abs(currentPos.Y - _lastClickPosition.Y) <= DoubleClickDistance)
            {
                if (DesktopUtilities.IsDesktopInFocus())
                {
                    Console.WriteLine("Double click detected on desktop.");

                    if (DesktopUtilities.IsAnyDesktopIconSelected())
                    {
                        Console.WriteLine("Double click on an icon.");
                    }
                    else
                    {
                        Console.WriteLine("Double click on desktop but not on an icon.");
                        ShowFlashOverlay(EffectArea.Desktop);
                        DesktopUtilities.ToggleIcons();
                    }
                }
                else if (Taskbar.IsTaskbarInFocus())
                {

                    bool isClickInTray = Taskbar.IsClickInTaskbarTrayArea(currentPos.X, currentPos.Y);

                    Console.WriteLine("Click in tray area: " + isClickInTray);

                    if (Taskbar.IsTaskbarVisible() && !Taskbar.IsClickInTaskbarTrayArea(currentPos.X, currentPos.Y))
                    {
                        ShowFlashOverlay(EffectArea.Taskbar, 25);
                        Taskbar.HideShowTaskbar(true);
                    }
                }
                else
                {
                    Console.WriteLine("Double click detected outside.");
                }
            }

            // Check if the click was outside the settings form
            // Close the settings form if it's open
            trayMenu.CheckClickOutsideForm(currentPos);

            _lastClickTime = currentTime;
            _lastClickPosition = currentPos;
        }

        private void OnMouseMove()
        {
            if (NativeMethods.GetCursorPos(out Point point))
            {
                Point currentPos = new Point(point.X, point.Y);
                //Console.WriteLine("Mouse moved to: " + currentPos.X + ", " + currentPos.Y);
                if (!Taskbar.IsTaskbarVisible())
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        Taskbar.TriggerUnhideRelativeToMousePosition(currentPos);
                    });
                }
            }
        }

        public void OnApplicationExit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;

            // Save settings to file before exiting the application.
            Settings.Current.Save();
        }

        // Desktop & taskbar flash effect areas
        public enum EffectArea
        {
            Desktop,
            Taskbar
        }

        // Show a flash overlay effect on the desktop or taskbar
        private void ShowFlashOverlay(EffectArea mode, int interval = 50, float opacity = 0.25f)
        {
            OverlayForm overlay = new OverlayForm
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                TopMost = true,
                BackColor = Color.White,
                Opacity = opacity,
                ShowInTaskbar = false
            };

            if (mode == EffectArea.Desktop)
            {
                overlay.Bounds = Screen.PrimaryScreen.WorkingArea;
            }
            else if (mode == EffectArea.Taskbar)
            {
                var taskbarRect = Taskbar.GetTaskbarPositionAndSize();
                if (taskbarRect.HasValue)
                {
                    overlay.Bounds = taskbarRect.Value;
                }
                else
                {
                    overlay.Bounds = Screen.PrimaryScreen.WorkingArea;
                }
            }

            overlay.Show();

            Timer flashTimer = new Timer();
            flashTimer.Interval = interval;
            flashTimer.Tick += (s, e) =>
            {
                flashTimer.Stop();
                overlay.Close();
            };
            flashTimer.Start();
        }

        private void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}