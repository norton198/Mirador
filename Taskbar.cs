using System.Runtime.InteropServices;
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
            public int left;
            public int top;
            public int right;
            public int bottom;
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

        private static bool allowStartMenuAccessWhenHidden;

        static void SetTaskbarState(int state)
        {
            APPBARDATA appBarData = new APPBARDATA();
            appBarData.cbSize = Marshal.SizeOf(appBarData);
            appBarData.hWnd = FindWindow("Shell_TrayWnd", null);
            appBarData.lParam = new IntPtr(state);

            SHAppBarMessage(ABM_SETSTATE, ref appBarData);
        }

        static void HideShowTaskbar(bool hide)
        {
            IntPtr taskbarWnd = FindWindow("Shell_TrayWnd", null);
            IntPtr startButtonWnd = FindWindow("Button", "Start");

            if (hide)
            {
                SetTaskbarState(ABS_AUTOHIDE);
                bool result = false;

                // Ensure taskbar is hidden and ShowWindow returns true
                do
                {
                    result = ShowWindow(taskbarWnd, SW_HIDE);
                    Console.WriteLine($"ShowWindow result: {result}");
                    Thread.Sleep(50); // Small sleep interval to give system time to process
                }
                while (!result);

                Thread.Sleep(300); // In case the check put in place in not enaugh !!

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
    }
}
