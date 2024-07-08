using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using Timer = System.Windows.Forms.Timer;

namespace Mirador
{
    public partial class SettingsForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        private bool isDesktopToggled = false;
        private bool isTaskbarToggled = false;
        private bool isDoubleClickToggled = false;
        private bool isCursorToggled = false;
        private bool isKeyToggled = false;
        private bool isAutoHideToggled = false;
        private bool isShortcutListening = false;

        private Timer listeningTimer;
        private int dotCount = 0;
        public SettingsForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            SetRoundedCorners(this, 20);
            this.Resize += SettingsForm_Resize;
            SetButtonRoundedCorners();
            InitializeButtonStates();
        }

        private void SettingsForm_Resize(object sender, EventArgs e)
        {
            SetRoundedCorners(this, 20);
        }

        private void SetRoundedCorners(Control control, int radius)
        {
            control.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius));
        }

        private void SetButtonRoundedCorners()
        {
            SetRoundedCorners(btnDesktop, 5);
            SetRoundedCorners(btnTaskbar, 5);
            SetRoundedCorners(btnAutoHide, 5);
            SetRoundedCorners(btnDoubleClick, 5);
            SetRoundedCorners(btnCursor, 5);
            SetRoundedCorners(btnKey, 5);
            SetRoundedCorners(btnRightCorner, 5);
            SetRoundedCorners(btnLeftCorner, 5);
            SetRoundedCorners(btnBothCorners, 5);
            SetRoundedCorners(btnEntireBar, 5);
            SetRoundedCorners(btnShortcut, 5);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }

        public void ClearShortcutBtnText()
        {
            btnShortcut.Text = "";
        }

        public void SetLastKnownShortcut()
        {
            var storedKeys = Settings.Current.ShortcutKeys;

            if (storedKeys == null || storedKeys.Count == 0)
            {
                btnShortcut.Text = "Shortcut not set";
            }
            else
            {
                List<string> keys = new List<string>();

                foreach (var key in storedKeys)
                {
                    var keyName = RawInput.GetKeyName(key, 0);
                    keys.Add(keyName);
                    Console.WriteLine($"Key: {key} ({keyName})");
                }

                btnShortcut.Text = string.Join(" + ", keys);
            }
        }

        public void StopListeningAnimation()
        {
            listeningTimer.Stop();
            listeningTimer.Dispose();
        }

        public void StartListeningAnimation()
        {
            dotCount = 0;
            listeningTimer = new Timer();
            listeningTimer.Interval = 250;
            listeningTimer.Tick += ListeningTimer_Tick;
            listeningTimer.Start();
        }
        private void ListeningTimer_Tick(object sender, EventArgs e)
        {
            dotCount = (dotCount + 1) % 4;
            string dots = new string('.', dotCount);
            btnShortcut.Text = $"Listening{dots} 👂 ";
        }

        public void UpdateShortcutBtnText(string key, bool ShortcutSet)
        {
            if (ShortcutSet)
            {
                btnShortcut.BackColor = Color.FromArgb(60, 60, 60);
                isShortcutListening = false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(btnShortcut.Text))
                {
                    btnShortcut.Text = key;
                }
                else
                {
                    btnShortcut.Text += $" + {key}";
                }
            }
        }

        private void btnTaskbar_Click(object sender, EventArgs e)
        {
            isTaskbarToggled = !isTaskbarToggled;
            Settings.Current.IsTaskbarToggled = isTaskbarToggled;
            panel1.Visible = isTaskbarToggled;


            if ((isCursorToggled || isKeyToggled) && isTaskbarToggled) panel2.Visible = true;
            else if (!isTaskbarToggled) panel2.Visible = false;

            btnTaskbar.BackColor = isTaskbarToggled ? Color.FromArgb(220, 175, 155) : Color.FromArgb(60, 60, 60);

            if (isAutoHideToggled) isAutoHideToggled = false;
            Settings.Current.AutoHide = isAutoHideToggled;

            btnAutoHide.BackColor = isAutoHideToggled ? Color.FromArgb(220, 175, 155) : Color.FromArgb(60, 60, 60);
        }

        private void delaySlider_Scroll(object sender, EventArgs e)
        {
            Settings.Current.HideDelay = delaySlider.Value;
            lblDelay.Text = $"Delay: {delaySlider.Value}ms";
        }

        int panelCallers = 0;

        private void MakePanelVisible(bool isVisible, bool isCursorOrKey)
        {
            if (isVisible)
            {
                panelCallers++;
            }
            else
            {
                panelCallers--;
            }

            if (panelCallers > 0)
            {
                panel2.Visible = true;
            }
            else
            {
                panel2.Visible = false;
            }

            if (isCursorOrKey && isVisible)
            {
                btnBothCorners.Visible = true;
                btnLeftCorner.Visible = true;
                btnRightCorner.Visible = true;
                btnEntireBar.Visible = true;

                lblAnyCorner.Visible = true;
                lblLeftCorner.Visible = true;
                lblRightCorner.Visible = true;
                lblEntireBar.Visible = true;

                delaySlider.Visible = true;
                lblDelay.Visible = true;
            }
            else if (isCursorOrKey && !isVisible)
            {
                btnBothCorners.Visible = false;
                btnLeftCorner.Visible = false;
                btnRightCorner.Visible = false;
                btnEntireBar.Visible = false;

                lblAnyCorner.Visible = false;
                lblLeftCorner.Visible = false;
                lblRightCorner.Visible = false;
                lblEntireBar.Visible = false;

                delaySlider.Visible = false;
                lblDelay.Visible = false;
            }

            if (!isCursorOrKey && isVisible)
            {
                btnShortcut.Visible = true;
                lblShortcut.Visible = true;
            }
            else if (!isCursorOrKey && !isVisible)
            {
                btnShortcut.Visible = false;
                lblShortcut.Visible = false;
            }
        }

        private void toggleButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                bool isToggled = button.BackColor == Color.FromArgb(220, 175, 155);
                button.BackColor = isToggled ? Color.FromArgb(60, 60, 60) : Color.FromArgb(220, 175, 155);
                if (button == btnCursor)
                {
                    isCursorToggled = !isCursorToggled;
                    Settings.Current.IsCursorToggled = isCursorToggled;
                    MakePanelVisible(isCursorToggled, true);

                    //TO DO:
                    if (isAutoHideToggled && !isCursorToggled && !isKeyToggled)
                    {
                        isAutoHideToggled = false;
                        Settings.Current.AutoHide = isAutoHideToggled;
                        btnAutoHide.BackColor = Color.FromArgb(60, 60, 60);
                    }
                }
                else if (button == btnDoubleClick)
                {
                    // Handle double click button click
                    isDoubleClickToggled = !isDoubleClickToggled;
                    Settings.Current.DoubleClickToHide = isDoubleClickToggled;
                }
                else if (button == btnDesktop)
                {
                    // Handle desktop button click
                    isDesktopToggled = !isDesktopToggled;
                    Settings.Current.IsDesktopSHIconsToggled = isDesktopToggled;
                }
                else if (button == btnKey)
                {
                    isKeyToggled = !isKeyToggled;
                    Settings.Current.IsShortcutToggled = isKeyToggled;
                    MakePanelVisible(isKeyToggled, false);
                    if(isAutoHideToggled && !isCursorToggled && !isKeyToggled)
                    {
                        isAutoHideToggled = false;
                        Settings.Current.AutoHide = isAutoHideToggled;
                        btnAutoHide.BackColor = Color.FromArgb(60, 60, 60);
                    }
                }
                else if (button == btnAutoHide)
                {
                    isAutoHideToggled = !isAutoHideToggled;
                    Settings.Current.AutoHide = isAutoHideToggled;
                    // We do this to ensure that there is a way to unhide the taskbar if the autohide is enabled and all the other options are displayed
                    if (!isTaskbarToggled) btnTaskbar_Click(sender, e);
                    if (!isKeyToggled && !isCursorToggled)
                    {
                        isCursorToggled = !isCursorToggled;
                        Settings.Current.IsCursorToggled = isCursorToggled;
                        MakePanelVisible(isCursorToggled, true);
                        btnCursor.BackColor = isCursorToggled ? Color.FromArgb(220, 175, 155) : Color.FromArgb(60, 60, 60);
                    }
                }
                else if (button == btnShortcut)
                {
                    isShortcutListening = !isShortcutListening;

                    if (isShortcutListening)
                    {
                        StartListeningAnimation();
                    }
                    else
                    {
                        StopListeningAnimation();
                        SetLastKnownShortcut();
                    }
                    Program.rawInput.ListenForShortcut(isShortcutListening);
                }
            }
        }

        private void exclusiveButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            // Reset all buttons to default color
            btnRightCorner.BackColor = Color.FromArgb(60, 60, 60);
            btnLeftCorner.BackColor = Color.FromArgb(60, 60, 60);
            btnBothCorners.BackColor = Color.FromArgb(60, 60, 60);
            btnEntireBar.BackColor = Color.FromArgb(60, 60, 60);

            // Set clicked button to toggled color
            if (clickedButton != null)
            {
                clickedButton.BackColor = Color.FromArgb(220, 175, 155);
            }

            switch (clickedButton.Name)
            {
                case "btnRightCorner":
                    Settings.Current.CursorUnhideRegion = 0;
                    Console.WriteLine("Right corner selected");
                    break;
                case "btnLeftCorner":
                    Settings.Current.CursorUnhideRegion = 1;
                    Console.WriteLine("Left corner selected");
                    break;
                case "btnBothCorners":
                    Settings.Current.CursorUnhideRegion = 2;
                    Console.WriteLine("Both corners selected");
                    break;
                case "btnEntireBar":
                    Settings.Current.CursorUnhideRegion = 3;
                    Console.WriteLine("Entire bar selected");
                    break;
            }
        }

        private void InitializeButtonStates()
        {
            Program.rawInput.settingsForm = this;

            // Initialize the buttons' states based on the current settings
            isDesktopToggled = Settings.Current.IsDesktopSHIconsToggled;
            btnDesktop.BackColor = isDesktopToggled ? Color.FromArgb(220, 175, 155) : Color.FromArgb(60, 60, 60);

            isTaskbarToggled = Settings.Current.IsTaskbarToggled;
            btnTaskbar.BackColor = isTaskbarToggled ? Color.FromArgb(220, 175, 155) : Color.FromArgb(60, 60, 60);
            panel1.Visible = isTaskbarToggled;

            isDoubleClickToggled = Settings.Current.DoubleClickToHide;
            btnDoubleClick.BackColor = isDoubleClickToggled ? Color.FromArgb(220, 175, 155) : Color.FromArgb(60, 60, 60);

            isAutoHideToggled = Settings.Current.AutoHide;
            btnAutoHide.BackColor = isAutoHideToggled ? Color.FromArgb(220, 175, 155) : Color.FromArgb(60, 60, 60);

            isCursorToggled = Settings.Current.IsCursorToggled;
            btnCursor.BackColor = isCursorToggled ? Color.FromArgb(220, 175, 155) : Color.FromArgb(60, 60, 60);
            if (isCursorToggled && isTaskbarToggled) MakePanelVisible(isCursorToggled, true);

            isKeyToggled = Settings.Current.IsShortcutToggled;
            btnKey.BackColor = isKeyToggled ? Color.FromArgb(220, 175, 155) : Color.FromArgb(60, 60, 60);
            if (isKeyToggled && isTaskbarToggled) MakePanelVisible(isKeyToggled, false);

            isAutoHideToggled = Settings.Current.AutoHide;
            btnAutoHide.BackColor = isAutoHideToggled ? Color.FromArgb(220, 175, 155) : Color.FromArgb(60, 60, 60);

            lblDelay.Text = $"Delay: {Settings.Current.HideDelay}ms";
            delaySlider.Value = Settings.Current.HideDelay;

            if (!isTaskbarToggled || !isCursorToggled) { 
                lblDelay.Visible = false;
                delaySlider.Visible = false;
            }

            SetLastKnownShortcut();

            // Initialize exclusive buttons for CursorUnhideRegion
            switch (Settings.Current.CursorUnhideRegion)
            {
                case 0:
                    btnRightCorner.BackColor = Color.FromArgb(220, 175, 155);
                    break;
                case 1:
                    btnLeftCorner.BackColor = Color.FromArgb(220, 175, 155);
                    break;
                case 2:
                    btnBothCorners.BackColor = Color.FromArgb(220, 175, 155);
                    break;
                case 3:
                    btnEntireBar.BackColor = Color.FromArgb(220, 175, 155);
                    break;
            }
        }
    }
}
