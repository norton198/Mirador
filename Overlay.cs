using System;
using System.Drawing;
using System.Windows.Forms;
namespace Mirador
{
    public class OverlayForm : Form
    {
        public OverlayForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            Bounds = Screen.PrimaryScreen.WorkingArea;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            BackColor = Color.White;
            Opacity = 0.5;
            ShowInTaskbar = false;
        }
    }
}