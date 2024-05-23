using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mirador
{
    public class RawInput
    {
        public event EventHandler<RawMouseEventArgs> MouseMoved;
        public event EventHandler<RawMouseEventArgs> LeftButtonDown;
        public event EventHandler<RawMouseEventArgs> LeftButtonUp;

        private const int WM_INPUT = 0x00FF;
        private const uint RIDEV_INPUTSINK = 0x00000100;
        private const int RID_INPUT = 0x10000003;
        private const int RIM_TYPEMOUSE = 0;
        private const int RI_MOUSE_LEFT_BUTTON_DOWN = 0x0001;
        private const int RI_MOUSE_LEFT_BUTTON_UP = 0x0002;

        private bool isLeftButtonDown = false;

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTHEADER
        {
            public uint dwType;
            public uint dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWMOUSE
        {
            public ushort usFlags;
            public uint ulButtons;
            public ushort usButtonFlags;
            public ushort usButtonData;
            public uint ulRawButtons;
            public int lLastX;
            public int lLastY;
            public uint ulExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUT
        {
            public RAWINPUTHEADER header;
            public RAWMOUSE mouse;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICE
        {
            public ushort usUsagePage;
            public ushort usUsage;
            public uint dwFlags;
            public IntPtr hwndTarget;
        }

        [DllImport("User32.dll")]
        public static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        [DllImport("User32.dll")]
        public static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

        public void RegisterRawMouseInput(IntPtr hwndTarget)
        {
            RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];

            rid[0].usUsagePage = 0x01;
            rid[0].usUsage = 0x02;
            rid[0].dwFlags = RIDEV_INPUTSINK;
            rid[0].hwndTarget = hwndTarget;

            if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE))))
            {
                throw new ApplicationException("Failed to register raw input device.");
            }
            else
            {
                Console.WriteLine("Raw input device registered successfully.");
            }
        }

        public void ProcessInputMessage(ref Message m)
        {
            if (m.Msg == WM_INPUT)
            {
                uint dwSize = 0;

                GetRawInputData(m.LParam, RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));
                if (dwSize == 0)
                {
                    Console.WriteLine("Failed to get raw input data size.");
                    return;
                }

                IntPtr rawInputBuffer = Marshal.AllocHGlobal((int)dwSize);

                if (GetRawInputData(m.LParam, RID_INPUT, rawInputBuffer, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) != dwSize)
                {
                    Console.WriteLine("Failed to get raw input data.");
                    return;
                }

                RAWINPUT rawInput = Marshal.PtrToStructure<RAWINPUT>(rawInputBuffer);

                if (rawInput.header.dwType == RIM_TYPEMOUSE)
                {
                    int x = rawInput.mouse.lLastX;
                    int y = rawInput.mouse.lLastY;
                    uint buttonFlags = rawInput.mouse.ulButtons;

                    bool downState = (rawInput.mouse.ulButtons & RI_MOUSE_LEFT_BUTTON_DOWN) > 0 || (rawInput.mouse.usButtonFlags & RI_MOUSE_LEFT_BUTTON_DOWN) > 0;
                    bool upState = (rawInput.mouse.ulButtons & RI_MOUSE_LEFT_BUTTON_UP) > 0 || (rawInput.mouse.usButtonFlags & RI_MOUSE_LEFT_BUTTON_UP) > 0;

                    if (downState && !upState)
                    {
                        Console.WriteLine("Left button down.");
                        LeftButtonDown?.Invoke(this, new RawMouseEventArgs(x, y));
                    }
                    if (!downState && upState)
                    {
                        Console.WriteLine("Left button up.");
                        LeftButtonUp?.Invoke(this, new RawMouseEventArgs(x, y));
                    }

                    MouseMoved?.Invoke(this, new RawMouseEventArgs(x, y));
                }
            }
        }
    }

    public class RawMouseEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }

        public RawMouseEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
