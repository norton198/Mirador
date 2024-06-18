using System.Runtime.InteropServices;
using static NativeMethods;
using Point = System.Drawing.Point;
using System.Windows.Automation;

namespace Mirador
{
    internal class Taskbar
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
        private const int TIME_DELAY = 500; 
        private static Point _lastMousePosition;
        private static DateTime _lastMouseMoveTime;
        private static System.Timers.Timer _autoHideTimer;
        private const int CHECK_INTERVAL = 100; // Check every 100 ms

        public enum Corner
        {
            RightBottom,
            LeftBottom,
            BothBottom,
            EntireBottom
        }

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
                        _autoHideTimer = new System.Timers.Timer(CHECK_INTERVAL);
                        Console.WriteLine($"Auto-hide timer created with interval: {CHECK_INTERVAL}");

                        _autoHideTimer.Elapsed += (sender, args) =>
                        {
                            lock (_lockObject)
                            {
                                double elapsedMilliseconds = (DateTime.Now - _lastMouseMoveTime).TotalMilliseconds;
                                Console.WriteLine($"Timer elapsed, elapsed milliseconds since last mouse move: {elapsedMilliseconds}");

                                if (!CanHideTaskbar(mousePosition, taskbarRect)) return;

                                if (elapsedMilliseconds >= Properties.Settings.Default.HideDelay)
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

        private static bool CanHideTaskbar(Point mousePosition, Rectangle? taskbarRect)
        {

            return taskbarRect.HasValue &&
                                     !taskbarRect.Value.Contains(mousePosition) &&
                                     !IsAnyTaskbarWindowInFocus();
        }

        public static bool IsAnyTaskbarWindowInFocus()
        {
            IntPtr focusedWnd = GetForegroundWindow();

            IntPtr startMenuWnd = FindWindow("Windows.UI.Core.CoreWindow", "Start");
            IntPtr searchWnd = FindWindow("Windows.UI.Core.CoreWindow", "Search");
            IntPtr taskViewWnd = FindWindow("Windows.UI.Core.CoreWindow", "Task View");
            IntPtr notificationCenterWnd = FindWindow("Windows.UI.Core.CoreWindow", "Notification Center");
            IntPtr pinnedAppsWnd = FindWindow("Shell_TrayWnd", null);

            Console.WriteLine($"Focused window handle: {focusedWnd}");
            Console.WriteLine($"Start menu window handle: {startMenuWnd}");
            Console.WriteLine($"Search window handle: {searchWnd}");
            Console.WriteLine($"Task View window handle: {taskViewWnd}");
            Console.WriteLine($"Notification Center window handle: {notificationCenterWnd}");
            Console.WriteLine($"Pinned apps window handle: {pinnedAppsWnd}");

            bool isInFocus = focusedWnd == startMenuWnd ||
                             focusedWnd == searchWnd ||
                             focusedWnd == taskViewWnd ||
                             focusedWnd == notificationCenterWnd ||
                             focusedWnd == pinnedAppsWnd ||
                             IsTrayInUse() || IsNotificationCenterInUse();

            Console.WriteLine($"Is any taskbar window in focus: {isInFocus}");

            return isInFocus;
        }

        // Method to check if the tray is in use on Windows 11
        private static bool IsTrayInUse()
        {
            try
            {
                // The handle of the element
                IntPtr handle = new IntPtr(0x0002033C);

                // Locate the element by its handle
                AutomationElement element = AutomationElement.FromHandle(handle);
                if (element == null)
                {
                    throw new InvalidOperationException("Could not find element with the specified handle.");
                }

                // Verify the class name to ensure it's the correct element
                if (element.Current.ClassName != "Windows.UI.Composition.DesktopWindowContentBridge")
                {
                    throw new InvalidOperationException("Found element does not match the expected class name.");
                }

                // Check if the element is in focus
                bool isFocused = element.Current.HasKeyboardFocus;

                Console.WriteLine($"Element focus status: {isFocused}");

                return isFocused;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking tray status: {ex.Message}");
                return false;
            }
        }

        // Is notification center in use ?
        private static bool IsNotificationCenterInUse()
        {
            try
            {
                // The handle of the element
                IntPtr handle = new IntPtr(0x00100488);

                // Locate the element by its handle
                AutomationElement element = AutomationElement.FromHandle(handle);
                if (element == null)
                {
                    throw new InvalidOperationException("Could not find element with the specified handle.");
                }

                // Verify the class name to ensure it's the correct element
                if (element.Current.ClassName != "Windows.UI.Core.CoreWindow")
                {
                    throw new InvalidOperationException("Found element does not match the expected class name.");
                }

                // Check if the element is in focus
                bool isFocused = element.Current.HasKeyboardFocus;

                Console.WriteLine($"Element focus status: {isFocused}");

                return isFocused;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking tray status: {ex.Message}");
                return false;
            }
        }

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

            var detectionRegion = Properties.Settings.Default.CursorUnhideRegion;

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
            IntPtr taskbarWnd = FindWindow("Shell_TrayWnd", null);
            IntPtr startButtonWnd = FindWindow("Button", "Start");

            if (hide)
            {
                SetTaskbarState(ABS_AUTOHIDE);
                bool result = false;

                // Ensure taskbar is hidden and ShowWindow returns true
                SpinWait.SpinUntil(() =>
                {
                    result = ShowWindow(taskbarWnd, SW_HIDE);
                    Console.WriteLine($"ShowWindow result: {result}");
                    return result;
                }, TimeSpan.FromSeconds(1));

                if (!result)
                {
                    Console.WriteLine("Failed to hide taskbar within the timeout period.");
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
