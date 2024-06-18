using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
        public SettingsForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            SetRoundedCorners(this, 20);
            this.Resize += SettingsForm_Resize;
            SetButtonRoundedCorners();
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

        private void btnTaskbar_Click(object sender, EventArgs e)
        {
            isTaskbarToggled = !isTaskbarToggled;
            panel1.Visible = isTaskbarToggled;
            btnTaskbar.BackColor = isTaskbarToggled ? Color.FromArgb(220, 175, 155) : Color.FromArgb(60, 60, 60);
        }

        private void btnDesktop_Click(object sender, EventArgs e)
        {
            // Handle btnDesktop click
        }

        private void lblTip_Click(object sender, EventArgs e)
        {
            // Handle lblTip click
        }

        private void volumeSlider_Scroll(object sender, EventArgs e)
        {
            // Handle volumeSlider scroll
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

                volumeSlider.Visible = true;
                lblVolume.Visible = true;
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

                volumeSlider.Visible = false;
                lblVolume.Visible = false;
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
                    MakePanelVisible(isCursorToggled, true);
                }
                else if (button == btnKey)
                {
                    isKeyToggled = !isKeyToggled;
                    MakePanelVisible(isKeyToggled, false);
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
                    Properties.Settings.Default.CursorUnhideRegion = 0;
                    Console.WriteLine("Right corner selected");
                    break;
                case "btnLeftCorner":
                    Properties.Settings.Default.CursorUnhideRegion = 1;
                    Console.WriteLine("Left corner selected");
                    break;
                case "btnBothCorners":
                    Properties.Settings.Default.CursorUnhideRegion = 2;
                    Console.WriteLine("Both corners selected");
                    break;
                case "btnEntireBar":
                    Properties.Settings.Default.CursorUnhideRegion = 3;
                    Console.WriteLine("Entire bar selected");
                    break;

            }
        }

        private void lblShortcut_Click(object sender, EventArgs e)
        {

        }

        private void buttonShortcut_Click(object sender, EventArgs e)
        {

        }

        private void btnAutoHide_Click(object sender, EventArgs e)
        {

        }
    }
}