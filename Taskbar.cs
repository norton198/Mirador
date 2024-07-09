using System.Runtime.InteropServices;
using static NativeMethods;
using Point = System.Drawing.Point;
using System.Windows.Automation;

namespace Mirador
{
    public class Taskbar
    {
        const int ABM_SETSTATE = 0xA;
        const int ABS_AUTOHIDE = 0x1;
        const int ABS_ALWAYSONTOP = 0x2;
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // DllImports should be moved to NativeMethods.cs for better organization.

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        private static bool allowStartMenuAccessWhenHidden;

        private const int EDGE_THRESHOLD = 10;
        private const int AUTOHIDE_CHECK_INTERVAL = 100;
        private static DateTime _lastMouseMoveTime;
        private static System.Timers.Timer _autoHideTimer;

        public enum Corner
        {
            RightBottom,
            LeftBottom,
            BothBottom,
            EntireBottom
        }

        // This method checks if the mouse click happened in the taskbar tray area.
        public static bool IsClickInTaskbarTrayArea(int x, int y)
        {
            try
            {
                Rectangle? trayArea = GetTrayArea();
                if (trayArea.HasValue)
                {
                    // Check if the click is within the tray area
                    Rectangle rect = trayArea.Value;
                    Console.WriteLine($"Tray Area: Left={rect.Left}, Top={rect.Top}, Right={rect.Right}, Bottom={rect.Bottom}");
                    Console.WriteLine($"Click Coordinates: X={x}, Y={y}");

                    bool isInTrayArea = x >= rect.Left && x <= rect.Right && y >= rect.Top && y <= rect.Bottom;
                    Console.WriteLine($"Is In Tray Area: {isInTrayArea}");
                    return isInTrayArea;
                }
                else
                {
                    throw new InvalidOperationException("Could not determine tray area.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        // This works with the new taskbar in Windows 11
        // it finds the tray area and returns the rectangle so we can check if the mouse is in that region
        public static Rectangle? GetTrayArea()
        {
            // Find the desktop element
            AutomationElement desktop = AutomationElement.RootElement;

            // Locate the taskbar
            AutomationElement taskbar = FindElementByClassName(desktop, "Shell_TrayWnd");
            if (taskbar == null)
            {
                throw new InvalidOperationException("Could not find taskbar window.");
            }

            // Locate the system tray frame within the taskbar
            AutomationElement systemTrayFrame = FindElementByClassName(taskbar, "TrayNotifyWnd");
            if (systemTrayFrame == null)
            {
                throw new InvalidOperationException("Could not find TrayNotifyWnd area.");
            }

            // Get the bounding rectangle of the TrayNotifyWnd
            System.Windows.Rect rect = systemTrayFrame.Current.BoundingRectangle;
            return new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);
        }

        private static AutomationElement FindElementByClassName(AutomationElement parent, string className)
        {
            Condition condition = new PropertyCondition(AutomationElement.ClassNameProperty, className);
            return parent.FindFirst(TreeScope.Descendants, condition);
        }

        // Testing purposes only, might delete later
        // This method highlights the tray area with a red overlay, used for testing purposes to see if the tray area is detected correctly and the rectangle returned is correct
        public static void HighlightTrayArea(Rectangle trayArea)
        {
            Form highlightForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.Red,
                Opacity = 0.5,
                ShowInTaskbar = false,
                TopMost = true,
                StartPosition = FormStartPosition.Manual,
                Location = trayArea.Location,
                Size = trayArea.Size
            };

            // Make the form click-through
            int initialStyle = GetWindowLong(highlightForm.Handle, -20);
            SetWindowLong(highlightForm.Handle, -20, initialStyle | 0x80000 | 0x20);

            Application.Run(highlightForm);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private static bool _isAutoHideTaskbarRunning = false;
        private static readonly object _lockObject = new object();

        // This method is used to auto-hide the taskbar when the mouse is not near the taskbar area or
        // any taskbar window is in focus like the start menu, search, notification center, tray, etc.
        // Its intended to provide a better user experience than the default auto-hide feature in Windows while providing some customization.
        public static void AutoHideTaskbar(Point mousePosition)
        {
            lock (_lockObject)
            {
                if (_isAutoHideTaskbarRunning)
                {
                    Console.WriteLine("AutoHideTaskbar is already running. Exiting to prevent multiple calls.");
                    return;
                }
                _isAutoHideTaskbarRunning = true;
            }

            Console.WriteLine($"AutoHideTaskbar called with mousePosition: {mousePosition}");

            try
            {
                Rectangle? taskbarRect = GetTaskbarPositionAndSize();
                Console.WriteLine($"Taskbar position and size: {taskbarRect}");

                Console.WriteLine($"Should hide taskbar: {CanHideTaskbar(mousePosition, taskbarRect)}");

                if (CanHideTaskbar(mousePosition, taskbarRect))
                {
                    if (_autoHideTimer == null)
                    {
                        _autoHideTimer = new System.Timers.Timer(AUTOHIDE_CHECK_INTERVAL);
                        Console.WriteLine($"Auto-hide timer created with interval: {AUTOHIDE_CHECK_INTERVAL}");

                        _autoHideTimer.Elapsed += (sender, args) =>
                        {
                            lock (_lockObject)
                            {
                                double elapsedMilliseconds = (DateTime.Now - _lastMouseMoveTime).TotalMilliseconds;
                                Console.WriteLine($"Timer elapsed, elapsed milliseconds since last mouse move: {elapsedMilliseconds}");

                                if (!CanHideTaskbar(mousePosition, taskbarRect)) return;

                                if (elapsedMilliseconds >= Settings.Current.HideDelay)
                                {
                                    if (!CanHideTaskbar(mousePosition, taskbarRect)) return;
                                    Console.WriteLine("Elapsed time meets hide delay, hiding taskbar.");
                                    _autoHideTimer.Stop();
                                    ThreadPool.QueueUserWorkItem(state =>
                                    {
                                        HideShowTaskbar(true);
                                    });
                                }
                            }
                        };
                    }

                    _lastMouseMoveTime = DateTime.Now;
                    _autoHideTimer.Start();
                    Console.WriteLine("Auto-hide timer started.");
                }
                else
                {
                    _autoHideTimer?.Stop();
                    Console.WriteLine("Auto-hide timer stopped.");
                }
            }
            finally
            {
                lock (_lockObject)
                {
                    _isAutoHideTaskbarRunning = false;
                }
            }
        }

        private static bool IsForegroundWindowInFullscreen()
        {
            IntPtr hWnd = GetForegroundWindow();
            GetWindowRect(hWnd, out RECT rect);
            Rectangle screenRect = Screen.FromHandle(hWnd).Bounds;

            Console.WriteLine($"Foreground window rect: {rect}");
            Console.WriteLine($"Screen rect: {screenRect}");

            return rect.Left == screenRect.Left && rect.Top == screenRect.Top && rect.Right == screenRect.Right && rect.Bottom == screenRect.Bottom;
        }

        // This method checks if the taskbar should be hidden based on the mouse position and if any taskbar window is in focus
        private static bool CanHideTaskbar(Point mousePosition, Rectangle? taskbarRect)
        {

            return taskbarRect.HasValue &&
                                     !taskbarRect.Value.Contains(mousePosition) &&
                                     !IsAnyTaskbarWindowInFocus();
        }

        // This method works with the new taskbar in Windows 11.
        // This method checks if any taskbar window is in focus like the start menu, search, notification center, tray, etc.
        public static bool IsAnyTaskbarWindowInFocus()
        {
            IntPtr focusedWnd = GetForegroundWindow();

            IntPtr startMenuWnd = FindWindow("Windows.UI.Core.CoreWindow", "Start");
            IntPtr searchWnd = FindWindow("Windows.UI.Core.CoreWindow", "Search");
            IntPtr taskViewWnd = FindWindow("Windows.UI.Core.CoreWindow", "Task View");
            IntPtr notificationCenterWnd = FindWindow("Windows.UI.Core.CoreWindow", "Notification Center");
            IntPtr trayWnd = FindWindow("TopLevelWindowForOverflowXamlIsland", "System tray overflow window.");


            Console.WriteLine($"Focused window handle: {focusedWnd}");
            Console.WriteLine($"Start menu window handle: {startMenuWnd}");
            Console.WriteLine($"Search window handle: {searchWnd}");
            Console.WriteLine($"Task View window handle: {taskViewWnd}");
            Console.WriteLine($"Notification Center window handle: {notificationCenterWnd}");
            Console.WriteLine($"Tray window handle: {trayWnd}");

            bool isInFocus = focusedWnd == startMenuWnd ||
                             focusedWnd == searchWnd ||
                             focusedWnd == trayWnd ||
                             focusedWnd == notificationCenterWnd ||
                             Program.trayMenu._settingsForm != null;

            Console.WriteLine($"Is any taskbar window in focus: {isInFocus}");

            return isInFocus;
        }

        // This method triggers the taskbar to unhide based on the mouse position
        // It checks if the mouse is near the bottom edge, bottom-left corner, bottom-right corner or bottom-any corner
        public static void TriggerUnhideRelativeToMousePosition(Point mousePosition)
        {
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;

            bool isBottom = mousePosition.Y >= screenHeight - EDGE_THRESHOLD; // Checks if the mouse is near the bottom edge
            bool isLeftCorner = mousePosition.X <= EDGE_THRESHOLD && mousePosition.Y >= screenHeight - EDGE_THRESHOLD; // Checks if the mouse is near the bottom-left corner
            bool isRightCorner = mousePosition.X >= screenWidth - EDGE_THRESHOLD && mousePosition.Y >= screenHeight - EDGE_THRESHOLD; // Checks if the mouse is near the bottom-right corner

            Console.WriteLine($"Mouse Position: {mousePosition.X}, {mousePosition.Y}");
            Console.WriteLine($"Screen Width: {screenWidth}, Screen Height: {screenHeight}");
            Console.WriteLine($"Is Bottom: {isBottom}, Is Left Corner: {isLeftCorner}, Is Right Corner: {isRightCorner}");

            var detectionRegion = Settings.Current.CursorUnhideRegion;

            switch (detectionRegion)
            {
                case 0: // Corner.RightBottom
                    if (isBottom && isRightCorner)
                    {
                        Console.WriteLine("Triggering unhide for RightBottom corner.");
                        HideShowTaskbar(false);
                    }
                    break;
                case 1: // Corner.LeftBottom
                    if (isBottom && isLeftCorner)
                    {
                        Console.WriteLine("Triggering unhide for LeftBottom corner.");
                        HideShowTaskbar(false);
                    }
                    break;
                case 2: // Corner.AnyCorner
                    if (isBottom && (isLeftCorner || isRightCorner))
                    {
                        Console.WriteLine("Triggering unhide for any bottom corner.");
                        HideShowTaskbar(false);
                    }
                    break;
                case 3: // Corner.EntireBottom
                    if (isBottom)
                    {
                        Console.WriteLine("Triggering unhide for EntireBottom.");
                        HideShowTaskbar(false);
                    }
                    break;
            }
        }

        static void SetTaskbarState(int state)
        {
            APPBARDATA appBarData = new APPBARDATA();
            appBarData.cbSize = Marshal.SizeOf(appBarData);
            appBarData.hWnd = FindWindow("Shell_TrayWnd", null);
            appBarData.lParam = new IntPtr(state);

            SHAppBarMessage(ABM_SETSTATE, ref appBarData);
        }

        public static void HideShowTaskbar(bool hide)
        {
            if (IsForegroundWindowInFullscreen()) return;

            IntPtr taskbarWnd = FindWindow("Shell_TrayWnd", null);
            IntPtr startButtonWnd = FindWindow("Button", "Start");

            const int maxRetries = 5; // Maximum number of retries
            int retryCount = 0;

            if (hide)
            {
                SetTaskbarState(ABS_AUTOHIDE);
                bool result = false;

                // Ensure taskbar is hidden and ShowWindow returns true
                // If it doesn't return true, keep trying until it does or until the retry limit is reached
                while (retryCount < maxRetries)
                {
                    SpinWait.SpinUntil(() =>
                    {
                        result = ShowWindow(taskbarWnd, SW_HIDE);
                        Console.WriteLine($"ShowWindow result: {result}");
                        return result;
                    }, TimeSpan.FromSeconds(1));

                    if (result)
                    {
                        break;
                    }

                    retryCount++;
                    Console.WriteLine($"Retry attempt: {retryCount}");
                }

                if (!result)
                {
                    Console.WriteLine("Failed to hide taskbar within the retry limit.");
                    return;
                }

                ShowWindow(taskbarWnd, SW_HIDE);

                if (!allowStartMenuAccessWhenHidden)
                {
                    ShowWindow(startButtonWnd, SW_HIDE);
                }
            }
            else
            {
                ShowWindow(taskbarWnd, SW_SHOW);
                ShowWindow(startButtonWnd, SW_SHOW);
                SetTaskbarState(ABS_ALWAYSONTOP);
            }
        }

        internal static bool IsTaskbarInFocus()
        {
            IntPtr taskbarWnd = FindWindow("Shell_TrayWnd", null!);
            IntPtr focusedWnd = GetForegroundWindow();
            if (taskbarWnd == focusedWnd)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsTaskbarVisible()
        {
            IntPtr taskbarWnd = FindWindow("Shell_TrayWnd", null!);
            if(IsWindowVisible(taskbarWnd))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SystemParametersInfo(int uAction, int uParam, out RECT lpvParam, int fuWinIni);

        private const int SPI_GETWORKAREA = 0x0030;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public static Rectangle? GetTaskbarPositionAndSize()
        {
            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
            if (taskbarHandle == IntPtr.Zero)
            {
                return null;
            }

            if (GetWindowRect(taskbarHandle, out RECT rect))
            {
                return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }
            return null;
        }
    }
}
