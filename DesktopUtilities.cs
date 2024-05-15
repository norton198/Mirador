using System;
using System.Runtime.InteropServices;
using System.Text;
using static NativeMethods;
public static class DesktopUtilities
{
    public static bool IsDesktopInFocus()
    {
        IntPtr hwnd = GetForegroundWindow();
        GetWindowThreadProcessId(hwnd, out int processId);

        StringBuilder className = new StringBuilder(100);
        int len = GetClassNameW(hwnd, className, className.Capacity);
        if (len > 0)
        {
            string classNameString = className.ToString();
            // Check if the foreground window is the desktop (Progman) or a shell window (WorkerW)
            return classNameString == "Progman" || classNameString == "WorkerW";
        }
        return false;
    }

    public static bool IsAnyDesktopIconSelected()
    {
        IntPtr desktopHandle = GetDesktopListViewHandle();
        if (desktopHandle == IntPtr.Zero)
        {
            Console.WriteLine("Unable to get desktop ListView handle.");
            return false;
        }

        int itemCount = GetDesktopListViewItemCount(desktopHandle);
        for (int i = 0; i < itemCount; i++)
        {
            if (IsListViewItemSelected(desktopHandle, i))
            {
                return true;
            }
        }
        return false;
    }

    public static IntPtr GetDesktopListViewHandle()
    {
        IntPtr progman = FindWindow("Progman", null);
        IntPtr defView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
        IntPtr sysListView32 = FindWindowEx(defView, IntPtr.Zero, "SysListView32", null);

        if (sysListView32 != IntPtr.Zero)
            return sysListView32;

        // Sometimes, especially on newer versions, the desktop might be under a different structure
        IntPtr workerW = IntPtr.Zero;
        IntPtr result = IntPtr.Zero;

        // Enumerate all Windows looking for the correct one
        EnumWindows((wnd, param) =>
        {
            IntPtr found = FindWindowEx(wnd, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (found != IntPtr.Zero)
            {
                if (FindWindowEx(found, IntPtr.Zero, "SysListView32", null) != IntPtr.Zero)
                {
                    result = found;
                    return false; // Found, stop enumeration
                }
            }
            return true;
        }, IntPtr.Zero);

        return FindWindowEx(result, IntPtr.Zero, "SysListView32", null);
    }

    private static int GetDesktopListViewItemCount(IntPtr listViewHandle)
    {
        return SendMessage(listViewHandle, (uint)LVM_FIRST + 4, IntPtr.Zero, IntPtr.Zero);
    }

    private static bool IsListViewItemSelected(IntPtr listViewHandle, int itemIndex)
    {
        int state = SendMessage(listViewHandle, LVM_GETITEMSTATE, new IntPtr(itemIndex), new IntPtr(LVIS_SELECTED));
        return (state & LVIS_SELECTED) == LVIS_SELECTED;
    }

    public static void ToggleIcons()
    {
        // Find Progman
        IntPtr progman = NativeMethods.FindWindow("Progman", null);
        if (progman != IntPtr.Zero)
        {
            Console.WriteLine(progman != IntPtr.Zero);
        }
        NativeMethods.EnumWindows(new EnumWindowsProc((tophandle, param) =>
        {
            IntPtr shellDllDefView = NativeMethods.FindWindowEx(tophandle, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (shellDllDefView != IntPtr.Zero)
            {
                var hwndSysListView32 = FindWindowEx(shellDllDefView, IntPtr.Zero, "SysListView32", "FolderView");

                if (hwndSysListView32 != IntPtr.Zero)
                {
                    bool visible = IsWindowVisible(hwndSysListView32);
                    Console.WriteLine("SysListView32 found.");
                    ShowWindow(hwndSysListView32, visible ? 0 : 1);
                }
                else
                {
                    Console.WriteLine("SysListView32 window not found.");
                }
            }

            return true;
        }), IntPtr.Zero);
    }
}
