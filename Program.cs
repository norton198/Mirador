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
            rawInput.RegisterRawMouseInput(form.Handle);
            rawInput.MouseMoved += RawInput_MouseMoved;
            rawInput.LeftButtonUp += RawInput_LeftButtonUp;

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
            Point currentPos = new Point(e.X, e.Y);

            if (currentTime - _lastClickTime <= DoubleClickTime &&
                Math.Abs(currentPos.X - _lastClickPosition.X) <= DoubleClickDistance &&
                Math.Abs(currentPos.Y - _lastClickPosition.Y) <= DoubleClickDistance)
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
                        ShowFlashOverlay(EffectArea.Desktop);
                        DesktopUtilities.ToggleIcons();
                    }
                }
                else if (Taskbar.IsTaskbarInFocus())
                {
                    if (Taskbar.IsTaskbarVisible())
                    {
                        ShowFlashOverlay(EffectArea.Taskbar, 25);
                        Taskbar.HideShowTaskbar(true);
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

        private static void OnMouseMove()
        {
            if (NativeMethods.GetCursorPos(out Point point))
            {
                Point currentPos = new Point(point.X, point.Y);
                Console.WriteLine("Mouse moved to: " + currentPos.X + ", " + currentPos.Y);
                if (!Taskbar.IsTaskbarVisible())
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        Taskbar.TriggerUnhideRelativeToMousePosition(currentPos, Taskbar.Corner.RightBottom);
                    });
                }
            }
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
        }

        // Desktop & taskbar flash effect
        public enum EffectArea
        {
            Desktop,
            Taskbar
        }

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