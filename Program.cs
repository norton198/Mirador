using static NativeMethods;
using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;

namespace Mirador
{
    public class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public static RawInput rawInput { get; private set; }
        private NotifyIcon notifyIcon;

        private int _lastClickTime;
        private Point _lastClickPosition;
        private readonly int DoubleClickTime = GetDoubleClickTime();
        private readonly int DoubleClickDistance = GetSystemMetrics(SystemMetric.SM_CXDOUBLECLK);

        private const string UniqueIdentifier = "M1R4D0R-3RGO-3LFN-I99B-1NT1M3-1S0Z";

        private Timer _autoHideTaskbarTimer;
        private readonly int TimerInterval = 1000;
        public static TrayMenu trayMenu { get; private set; }

        [STAThread]
        public static void Main()
        {
            AllocConsole();
            bool createdNew;
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueIdentifier, out createdNew);

            if (!createdNew)
            {
                Console.WriteLine("Another instance is already running. Exiting new instance.");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var program = new Program();
            program.Initialize();
            Application.ApplicationExit += program.OnApplicationExit;
            Application.Run();
        }

        private void Initialize()
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

        private void OnApplicationExit(object sender, EventArgs e)
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

    // Used to handle raw input messages
    public class HiddenForm : Form
    {
        public HiddenForm()
        {
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Load += (s, e) => this.Hide();
        }

        protected override void WndProc(ref Message m)
        {
            Program.rawInput.ProcessInputMessage(ref m);
            base.WndProc(ref m);
        }
    }
}
