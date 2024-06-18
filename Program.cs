using Microsoft.VisualBasic.Devices;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NativeMethods;
using Timer = System.Windows.Forms.Timer;

namespace Mirador
{
    internal class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public static RawInput rawInput;
        private static NotifyIcon notifyIcon;

        private static int _lastClickTime;
        private static Point _lastClickPosition;
        private static readonly int DoubleClickTime = GetDoubleClickTime(); // System double-click time
        private static readonly int DoubleClickDistance = GetSystemMetrics(SystemMetric.SM_CXDOUBLECLK); // System double-click distance

        private const string UniqueIdentifier = "M1R4D0R-3RGO-3LFN-I99B-1NT1M3-1S0Z";

        private static Timer _autoHideTaskbarTimer;
        private static readonly int TimerInterval = 1000; // Interval in milliseconds
        private bool autohideTaskbarEnabled = Properties.Settings.Default.AutoHide;

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

            // Initialize Tray Icon
            TrayMenu.InitializeTrayIcon();

            // Create a hidden form to handle raw input
            var form = new HiddenForm();
            rawInput = new RawInput();
            rawInput.RegisterRawInput(form.Handle);
            rawInput.MouseMoved += RawInput_MouseMoved;
            rawInput.LeftButtonUp += RawInput_LeftButtonUp;

            /*
             * 
            Feeling like testing tray area detection today, might delete later

            Rectangle? trayArea = Taskbar.GetTrayArea();
            if (trayArea.HasValue)
            {
                Taskbar.HighlightTrayArea(trayArea.Value);
            }
            else
            {
                Console.WriteLine("Could not determine tray area.");
            }
            */

            // Not sure how i feel about this yet
            // Initialize and start the auto-hide taskbar timer
            _autoHideTaskbarTimer = new Timer();
            _autoHideTaskbarTimer.Interval = TimerInterval;
            _autoHideTaskbarTimer.Tick += (sender, args) =>
            {
                if (Taskbar.IsTaskbarVisible())
                {
                    NativeMethods.GetCursorPos(out Point point);
                    Point currentPos = new Point(point.X, point.Y);
                    Taskbar.AutoHideTaskbar(currentPos);
                }
            };
            _autoHideTaskbarTimer.Start();
            Console.WriteLine("Auto-hide taskbar timer started.");

            Application.ApplicationExit += OnApplicationExit;
            Application.Run();
        }

        private static void RawInput_MouseMoved(object sender, RawMouseEventArgs e)
        {
            OnMouseMove();
        }

        private static void RawInput_LeftButtonUp(object sender, RawMouseEventArgs e)
        {
            OnMouseButtonUp(e);
        }

        private static void OnMouseButtonUp(RawMouseEventArgs e)
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
            TrayMenu.CheckClickOutsideForm(currentPos);

            _lastClickTime = currentTime;
            _lastClickPosition = currentPos;
        }

        private static void OnMouseMove()
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

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
        }

        // Desktop & taskbar flash effect areas
        public enum EffectArea
        {
            Desktop,
            Taskbar
        }

        // Show a flash overlay effect on the desktop or taskbar
        private static void ShowFlashOverlay(EffectArea mode, int interval = 50, float opacity = 0.25f)
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

        private static void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    // Used to handle raw input messages
    // Might not be necessary
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