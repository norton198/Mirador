using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static NativeMethods;
using Point = NativeMethods.Point;

namespace Mirador
{
    internal static class Program
    {
        private static NotifyIcon _notifyIcon;
        private static ContextMenuStrip _contextMenu;

        private static IntPtr _hook = IntPtr.Zero;
        private static HookProc _hookProc;
        private static volatile bool _running = true;
        private static int _lastClickTime;
        private static Point _lastClickPosition;
        private static readonly int DoubleClickTime = GetDoubleClickTime(); // System double-click time
        private static readonly int DoubleClickDistance = GetSystemMetrics(SystemMetric.SM_CXDOUBLECLK);

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            InitializeTrayIcon();

            Thread hookThread = new Thread(HookThread);
            hookThread.Start();

            Application.ApplicationExit += OnApplicationExit;
            Application.Run();

            _running = false;
            hookThread.Join(); // Ensure the hook thread exits cleanly
            UnhookWindowsHookEx(_hook); // Unhook after the thread finishes
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            _running = false;
            UnhookWindowsHookEx(_hook);
            _notifyIcon.Visible = false;
        }

        private static void HookThread()
        {
            _hookProc = new HookProc(HookFunction);
            _hook = SetWindowsHookEx(HookType.WH_MOUSE_LL, _hookProc, IntPtr.Zero, 0);

            MSG msg;
            while (_running)
            {
                if (PeekMessage(out msg, IntPtr.Zero, 0, 0, 1))
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }
                else
                {
                    Thread.Sleep(10); // Sleep for 10 milliseconds if no message is found
                }
            }
            UnhookWindowsHookEx(_hook);
        }

        private static IntPtr HookFunction(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT mouseHookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                int currentTime = Environment.TickCount;
                Point currentPos = mouseHookStruct.pt;
                System.Drawing.Point currentPosAsRectangle = new System.Drawing.Point(mouseHookStruct.pt.x, mouseHookStruct.pt.y);

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
    }
}
