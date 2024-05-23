using System;
using System.Windows.Forms;

namespace Mirador
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {


            MessageBox.Show("Settings saved successfully", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
