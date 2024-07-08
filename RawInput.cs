using System.Runtime.InteropServices;
using System.Text;

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

        // Raw Input Mouse
        private const int RIM_TYPEMOUSE = 0;
        private const int RI_MOUSE_LEFT_BUTTON_DOWN = 0x0001;
        private const int RI_MOUSE_LEFT_BUTTON_UP = 0x0002;

        // Raw Input Keyboard
        private const int RIM_TYPEKEYBOARD = 1;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const uint WM_SYSKEYDOWN = 0x0104;
        public const uint WM_SYSKEYUP = 0x0105;
        public const uint MAPVK_VSC_TO_VK_EX = 0x03;
        public const uint MAPVK_VK_TO_CHAR = 0x02;


        public const ushort RI_KEY_BREAK = 0x01;
        public const ushort RI_KEY_E0 = 0x02;
        public const ushort RI_KEY_E1 = 0x04;

        private bool isLeftButtonDown = false;

        public SettingsForm settingsForm;

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
            public DUMMYUNIONNAME Union;
            public uint ulRawButtons;
            public int lLastX;
            public int lLastY;
            public uint ulExtraInformation;

            [StructLayout(LayoutKind.Explicit)]
            public struct DUMMYUNIONNAME
            {
                [FieldOffset(0)]
                public uint ulButtons;

                [FieldOffset(0)]
                public DUMMYSTRUCTNAME Struct;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct DUMMYSTRUCTNAME
            {
                public ushort usButtonFlags;
                public ushort usButtonData;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public ushort Flags;
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct RAWHID
        {
            [FieldOffset(0)]
            public uint dwSizeHid;
            [FieldOffset(4)]
            public uint dwCount;
            [FieldOffset(8)]
            public byte bRawData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUT
        {
            public RAWINPUTHEADER header;
            public RAWINPUTUNION data;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct RAWINPUTUNION
        {
            [FieldOffset(0)]
            public RAWMOUSE mouse;
            [FieldOffset(0)]
            public RAWKEYBOARD keyboard;
            [FieldOffset(0)]
            public RAWHID hid;
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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetKeyNameTextW(uint lParam, StringBuilder lpString, int cchSize);


        // Keyboard keys codes for reference and testing.
        public enum Keys : uint
        {
            // Modifier keys
            LeftCtrl = 0x1D,
            LeftShift = 0x2A,
            LeftAlt = 0x38,
            RightCtrl = 0xE01D,
            RightShift = 0x36,
            RightAlt = 0xE038,

            // Letter keys
            A = 0x1E,
            B = 0x30,
            C = 0x2E,
            D = 0x20,
            E = 0x12,
            F = 0x21,
            G = 0x22,
            H = 0x23,
            I = 0x17,
            J = 0x24,
            K = 0x25,
            L = 0x26,
            M = 0x32,
            N = 0x31,
            O = 0x18,
            P = 0x19,
            Q = 0x10,
            R = 0x13,
            S = 0x1F,
            T = 0x14,
            U = 0x16,
            V = 0x2F,
            W = 0x11,
            X = 0x2D,
            Y = 0x15,
            Z = 0x2C,

            // Number keys (top row)
            D0 = 0x0B,
            D1 = 0x02,
            D2 = 0x03,
            D3 = 0x04,
            D4 = 0x05,
            D5 = 0x06,
            D6 = 0x07,
            D7 = 0x08,
            D8 = 0x09,
            D9 = 0x0A,

            // Function keys
            F1 = 0x3B,
            F2 = 0x3C,
            F3 = 0x3D,
            F4 = 0x3E,
            F5 = 0x3F,
            F6 = 0x40,
            F7 = 0x41,
            F8 = 0x42,
            F9 = 0x43,
            F10 = 0x44,
            F11 = 0x57,
            F12 = 0x58,
            F13 = 0x64,
            F14 = 0x65,
            F15 = 0x66,
            F16 = 0x67,
            F17 = 0x68,
            F18 = 0x69,
            F19 = 0x6A,
            F20 = 0x6B,
            F21 = 0x6C,
            F22 = 0x6D,
            F23 = 0x6E,
            F24 = 0x76,

            // Special keys
            Enter = 0x1C,
            Space = 0x39,
            Backspace = 0x0E,
            Tab = 0x0F,
            Esc = 0x01,
            Insert = 0x52,
            Delete = 0x53,
            Home = 0x47,
            End = 0x4F,
            PageUp = 0x49,
            PageDown = 0x51,

            // Arrow keys
            UpArrow = 0x48,
            DownArrow = 0x50,
            LeftArrow = 0x4B,
            RightArrow = 0x4D,

            // Numpad keys
            NumLock = 0x45,
            Numpad0 = 0x52,
            Numpad1 = 0x4F,
            Numpad2 = 0x50,
            Numpad3 = 0x51,
            Numpad4 = 0x4B,
            Numpad5 = 0x4C,
            Numpad6 = 0x4D,
            Numpad7 = 0x47,
            Numpad8 = 0x48,
            Numpad9 = 0x49,
            NumpadAdd = 0x4E,
            NumpadSubtract = 0x4A,
            NumpadMultiply = 0x37,
            NumpadDivide = 0x35,
            NumpadEnter = 0xE01C,
            NumpadPeriod = 0x53,

            // Punctuation keys
            Semicolon = 0x27,
            Equals = 0x0D,
            Comma = 0x33,
            Minus = 0x0C,
            Period = 0x34,
            Slash = 0x35,
            GraveAccent = 0x29,
            LeftBracket = 0x1A,
            Backslash = 0x2B,
            RightBracket = 0x1B,
            Apostrophe = 0x28,

            // Other keys
            CapsLock = 0x3A,
            ScrollLock = 0x46,
            PrintScreen = 0xE037,
            Pause = 0xE11D45,

            // Media keys
            MediaPrevious = 0xE010,
            MediaNext = 0xE019,
            MediaPlay = 0xE022,
            MediaStop = 0xE024,
            VolumeMute = 0xE020,
            VolumeDown = 0xE02E,
            VolumeUp = 0xE030,

            // Browser keys
            BrowserBack = 0xE06A,
            BrowserForward = 0xE069,
            BrowserRefresh = 0xE067,
            BrowserStop = 0xE068,
            BrowserSearch = 0xE065,
            BrowserFavorites = 0xE066,
            BrowserHome = 0xE032,

            // Application launcher keys
            LaunchMail = 0xE06C,
            LaunchMedia = 0xE06D,
            LaunchApp1 = 0xE06B,
            LaunchApp2 = 0xE021,

            // OEM keys
            Oem1 = 0x5A,
            Oem2 = 0x5B,
            Oem3 = 0x5C,
            Oem4 = 0x5E,
            Oem5 = 0x5F,
            Oem6 = 0x6F,
            Oem7 = 0x71,
            Oem8 = 0xE07E,
            OemReset = 0x71,
            OemJump = 0x5C,
            OemPa1 = 0x7B,
            OemPa2 = 0xE05B,
            OemPa3 = 0x6F,
            OemWsCtrl = 0x5A,
            OemCusel = 0x5E,
            OemAttn = 0x7B,
            OemFinish = 0x5B,
            OemCopy = 0xE02A,
            OemAuto = 0x5F,
            OemEnlw = 0xE05E,
            OemBacktab = 0x5E,

            // Japanese keys
            Katakana = 0x70,
            Convert = 0x79,
            NoConvert = 0x7B,

            // Other extended keys
            MetaLeft = 0xE05B,
            MetaRight = 0xE05C,
            Application = 0xE05D,
            Power = 0xE05E,
            Sleep = 0xE05F,
            Wake = 0xE063,
        }

        public void RegisterRawInput(IntPtr hwndTarget)
        {
            RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[2];

            // Register for mouse input
            rid[0].usUsagePage = 0x01; // HID_USAGE_PAGE_GENERIC
            rid[0].usUsage = 0x02; // HID_USAGE_GENERIC_MOUSE
            rid[0].dwFlags = RIDEV_INPUTSINK; // Enables the caller to receive the input even when the caller is not in the foreground.
            rid[0].hwndTarget = hwndTarget; // Handle to the window that will receive raw input

            // Register for keyboard input
            rid[1].usUsagePage = 0x01; // HID_USAGE_PAGE_GENERIC
            rid[1].usUsage = 0x06; // HID_USAGE_GENERIC_KEYBOARD
            rid[1].dwFlags = RIDEV_INPUTSINK; // Enables the caller to receive the input even when the caller is not in the foreground.
            rid[1].hwndTarget = hwndTarget; // Handle to the window that will receive raw input

            if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE))))
            {
                throw new ApplicationException("Failed to register raw input device.");
            }
            else
            {
                Console.WriteLine("Raw input device registered successfully.");
            }
        }

        public static string GetKeyName(uint scanCode, uint flags)
        {
            uint lParam = (scanCode & 0xff) << 16;
            if ((flags & RI_KEY_E0) != 0)
                lParam |= 1 << 24;
            StringBuilder sb = new StringBuilder(256);
            int result = GetKeyNameTextW(lParam, sb, sb.Capacity);
            return result > 0 ? sb.ToString() : "Unknown";
        }

        private HashSet<uint> pressedKeys = new HashSet<uint>();
        private List<uint> shortcut = new List<uint>() { (uint)Keys.LeftCtrl, (uint)Keys.LeftShift, (uint)Keys.A };
        private bool isListeningForShortcut = false;
        private List<uint> currentCombination = new List<uint>();

        public void ListenForShortcut(bool isListening)
        {
            isListeningForShortcut = isListening;
            currentCombination.Clear();
        }

        public void SetShortcut(params uint[] keys)
        {
            shortcut = new List<uint>(keys);
            StoreShortcut(shortcut);
        }

        public void StoreShortcut(List<uint> keys)
        {
            Settings.Current.ShortcutKeys = keys;
            Settings.Current.Save();

            if (keys.Count > 0)
            {
                Console.WriteLine("Combined Shortcut: " + string.Join(" + ", keys.Select(k => RawInput.GetKeyName(k, 0))));
                Console.WriteLine("Shortcut stored successfully!");
            }
            else
            {
                Console.WriteLine("Failed to store shortcut.");
            }
        }

        private bool IsShortcutPressed()
        {
            var shortcutKeys = Settings.Current.ShortcutKeys;
            return shortcutKeys != null && shortcutKeys.All(key => pressedKeys.Contains(key));
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
                    Marshal.FreeHGlobal(rawInputBuffer);
                    return;
                }

                RAWINPUT rawInput = Marshal.PtrToStructure<RAWINPUT>(rawInputBuffer);
                Marshal.FreeHGlobal(rawInputBuffer);

                if (rawInput.header.dwType == RIM_TYPEMOUSE)
                {
                    //Console.WriteLine("Mouse input received.");

                    int x = rawInput.data.mouse.lLastY;
                    int y = rawInput.data.mouse.lLastY;
                    ushort buttonFlags = rawInput.data.mouse.Union.Struct.usButtonFlags;
                    uint ulButtons = rawInput.data.mouse.Union.ulButtons;

                    //Console.WriteLine($"Buttons: {ulButtons}, usButtonFlags: {buttonFlags}");
                    //Console.WriteLine($"usButtonFlags (binary): {Convert.ToString(buttonFlags, 2).PadLeft(16, '0')}");

                    bool leftButtonDown = (buttonFlags & RI_MOUSE_LEFT_BUTTON_DOWN) != 0;
                    bool leftButtonUp = (buttonFlags & RI_MOUSE_LEFT_BUTTON_UP) != 0;

                    //Console.WriteLine($"leftButtonDown : {leftButtonDown}, leftButtonUp : {leftButtonUp}");

                    if (leftButtonDown)
                    {
                        Console.WriteLine("Left button down.");
                        LeftButtonDown?.Invoke(this, new RawMouseEventArgs(x, y));
                    }

                    if (leftButtonUp)
                    {
                        Console.WriteLine("Left button up.");
                        LeftButtonUp?.Invoke(this, new RawMouseEventArgs(x, y));
                    }

                    MouseMoved?.Invoke(this, new RawMouseEventArgs(x, y));
                }
                else if (rawInput.header.dwType == RIM_TYPEKEYBOARD)
                {
                    ushort makeCode = rawInput.data.keyboard.MakeCode;
                    ushort flags = rawInput.data.keyboard.Flags;
                    bool isE0 = (flags & RI_KEY_E0) != 0;
                    bool isBreak = (flags & RI_KEY_BREAK) != 0;

                    if (isE0)
                    {
                        makeCode |= 0xE000;
                    }
                    else if ((flags & RI_KEY_E1) != 0)
                    {
                        makeCode |= 0xE100;
                    }

                    string keyName = GetKeyName((uint)makeCode, (uint)flags);
                    string state = isBreak ? "Up" : "Down";

                    if (!isBreak)
                    {
                        pressedKeys.Add(makeCode);
                        if (isListeningForShortcut)
                        {
                            if (!currentCombination.Contains(makeCode))
                            {
                                if (currentCombination.Count == 0)
                                {
                                    settingsForm.StopListeningAnimation();
                                    settingsForm.ClearShortcutBtnText();
                                }

                                currentCombination.Add(makeCode);
                                settingsForm.UpdateShortcutBtnText(keyName, false);
                            }
                        }
                    }
                    else
                    {
                        pressedKeys.Remove(makeCode);
                    }

                    Console.WriteLine($"Scancode: {makeCode}, Key: {keyName}, State: {state}");

                    if (isListeningForShortcut)
                    {
                        if (currentCombination.Count > 0 && isBreak)
                        {
                            SetShortcut(currentCombination.ToArray());
                            Console.WriteLine("New shortcut set!");
                            isListeningForShortcut = false;
                            currentCombination.Clear();
                            settingsForm.UpdateShortcutBtnText("", true);
                        }
                    }
                    else if (IsShortcutPressed())
                    {
                        Console.WriteLine("Shortcut triggered!");
                        Taskbar.HideShowTaskbar(false);
                    }
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
