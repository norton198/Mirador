using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static NativeMethods;
using Point = NativeMethods.Point;
using Timer = System.Windows.Forms.Timer;

namespace Mirador
{
    internal static class Program
    {
        private static NotifyIcon _notifyIcon;
        private static ContextMenuStrip _contextMenu;
        private static Timer _timer;

        private static IntPtr _hook = IntPtr.Zero;
        private static HookProc _hookProc;
        private static int _lastClickTime;
        private static Point _lastClickPosition;
        private static readonly int DoubleClickTime = GetDoubleClickTime(); // System double-click time
        private static readonly int DoubleClickDistance = GetSystemMetrics(SystemMetric.SM_CXDOUBLECLK); // System double-click distance

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            InitializeTrayIcon();
            InitializeHook();
            InitializeTimer();

            Application.ApplicationExit += OnApplicationExit;
            Application.Run();
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            UnhookWindowsHookEx(_hook); // Unhook when the application exits
            _notifyIcon.Visible = false; // Hide the tray icon
        }

        // Initialize the low-level mouse hook
        private static void InitializeHook()
        {
            _hookProc = new HookProc(HookFunction);
            _hook = SetWindowsHookEx(HookType.WH_MOUSE_LL, _hookProc, IntPtr.Zero, 0);
        }

        // Initialize a timer to periodically check for messages
        private static void InitializeTimer()
        {
            _timer = new Timer();
            _timer.Interval = 10;
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        // Timer tick event to process messages
        private static void OnTimerTick(object sender, EventArgs e)
        {
            MSG msg;
            while (PeekMessage(out msg, IntPtr.Zero, 0, 0, 1))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }

        // Hook function to process mouse events
        private static IntPtr HookFunction(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT mouseHookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                int currentTime = Environment.TickCount;
                Point currentPos = mouseHookStruct.pt;

                if (currentTime - _lastClickTime <= DoubleClickTime &&
                    Math.Abs(currentPos.x - _lastClickPosition.x) <= DoubleClickDistance &&
                    Math.Abs(currentPos.y - _lastClickPosition.y) <= DoubleClickDistance)
                {
                    if (DesktopUtilities.IsDesktopInFocus())
                    {
                        Console.WriteLine("Double click detected [ON DESKTOP].");

                        if (DesktopUtilities.IsAnyDesktopIconSelected())
                        {
                            Console.WriteLine("Double click on an icon.");
                        }
                        else
                        {
                            Console.WriteLine("Double click on desktop but not on an icon.");
                            // Show visual flash effect
                            ShowFlashOverlay();
                            // Toggle desktop icons
                            DesktopUtilities.ToggleIcons();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Double click detected [OUTSIDE].");
                    }
                }

                _lastClickTime = currentTime;
                _lastClickPosition = currentPos;
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        // Initialize system tray icon and context menu
        private static void InitializeTrayIcon()
        {
            _notifyIcon = new NotifyIcon();
            _contextMenu = new ContextMenuStrip();

            var aboutMenuItem = new ToolStripMenuItem("About", null, OnAboutMenuItemClick);
            _contextMenu.Items.Add(aboutMenuItem);

            var exitMenuItem = new ToolStripMenuItem("Exit", null, OnExitMenuItemClick);
            _contextMenu.Items.Add(exitMenuItem);

            _notifyIcon.Text = "Mirador";
            _notifyIcon.Icon = Properties.Resources.Tray_Icon;
            _notifyIcon.ContextMenuStrip = _contextMenu;
            _notifyIcon.Visible = true;
        }

        private static void OnExitMenuItemClick(object sender, EventArgs e)
        {
            Application.Exit();
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

        // Screen flash effect
        private static void ShowFlashOverlay()
        {
            OverlayForm overlay = new OverlayForm();
            overlay.Show();

            Timer flashTimer = new Timer();
            flashTimer.Interval = 100;
            flashTimer.Tick += (s, e) =>
            {
                flashTimer.Stop();
                overlay.Close();
            };
            flashTimer.Start();
        }

    }
}
