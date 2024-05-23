using System.Runtime.InteropServices;
using static NativeMethods;
using Point = System.Drawing.Point;
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
        private System.Timers.Timer _timer;
        private Point _lastMousePosition;
        private DateTime _lastMouseMoveTime;

        public enum Corner
        {
            RightBottom,
            LeftBottom,
            BothBottom,
            EntireBottom
        }

        public static void TriggerUnhideRelativeToMousePosition(Point mousePosition, Corner detectionMode = Corner.EntireBottom)
        {
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;

            bool isBottom = mousePosition.X >= screenHeight - EDGE_THRESHOLD;
            bool isLeftCorner = mousePosition.Y <= EDGE_THRESHOLD;
            bool isRightCorner = mousePosition.X >= screenWidth - EDGE_THRESHOLD;

            Console.WriteLine($"Mouse Position: {mousePosition.X}, {mousePosition.Y}");
            Console.WriteLine($"Screen Width: {screenWidth}, Screen Height: {screenHeight}");
            Console.WriteLine($"Is Bottom: {isBottom}, Is Left Corner: {isLeftCorner}, Is Right Corner: {isRightCorner}");

            switch (detectionMode)
            {
                case Corner.RightBottom:
                    if (isBottom && isRightCorner)
                    {
                        Console.WriteLine("Triggering unhide for RightBottom corner.");
                        HideShowTaskbar(false);
                    }
                    break;
                case Corner.LeftBottom:
                    if (isBottom && isLeftCorner)
                    {
                        Console.WriteLine("Triggering unhide for LeftBottom corner.");
                        HideShowTaskbar(false);
                    }
                    break;
                case Corner.BothBottom:
                    if (isBottom && (isLeftCorner || isRightCorner))
                    {
                        Console.WriteLine("Triggering unhide for BothBottom corner.");
                        HideShowTaskbar(false);
                    }
                    break;
                case Corner.EntireBottom:
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
