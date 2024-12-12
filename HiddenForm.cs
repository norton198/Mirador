using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirador
{
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
            Mirador.rawInput.ProcessInputMessage(ref m);
            base.WndProc(ref m);
        }
    }
}
