namespace Mirador
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnDesktop;
        private System.Windows.Forms.Button btnTaskbar;
        private System.Windows.Forms.TrackBar delaySlider;
        private System.Windows.Forms.Label lblDelay;
        private System.Windows.Forms.Panel bottomBar;
        private System.Windows.Forms.Label lblDesktop;
        private System.Windows.Forms.Label lblTaskbar;
        private System.Windows.Forms.Panel topRowPanel;
        private System.Windows.Forms.ToolTip toolTip;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            btnDesktop = new Button();
            btnTaskbar = new Button();
            delaySlider = new TrackBar();
            lblDelay = new Label();
            bottomBar = new Panel();
            lblDesktop = new Label();
            lblTaskbar = new Label();
            topRowPanel = new Panel();
            btnAutoHide = new Button();
            lblTip = new Label();
            panel1 = new Panel();
            btnDoubleClick = new Button();
            label1 = new Label();
            btnCursor = new Button();
            label2 = new Label();
            btnKey = new Button();
            label3 = new Label();
            panel2 = new Panel();
            lblShortcut = new Label();
            btnShortcut = new Button();
            btnEntireBar = new Button();
            lblEntireBar = new Label();
            btnRightCorner = new Button();
            lblRightCorner = new Label();
            btnLeftCorner = new Button();
            btnBothCorners = new Button();
            lblLeftCorner = new Label();
            lblAnyCorner = new Label();
            toolTip = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)delaySlider).BeginInit();
            topRowPanel.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // btnDesktop
            // 
            btnDesktop.BackColor = Color.FromArgb(60, 60, 60);
            btnDesktop.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
            btnDesktop.FlatAppearance.BorderSize = 0;
            btnDesktop.FlatStyle = FlatStyle.Flat;
            btnDesktop.ForeColor = Color.White;
            btnDesktop.Image = Properties.Resources.DesktopIcon;
            btnDesktop.Location = new Point(0, 0);
            btnDesktop.Name = "btnDesktop";
            btnDesktop.Size = new Size(80, 45);
            btnDesktop.TabIndex = 1;
            toolTip.SetToolTip(btnDesktop, "Toggle Desktop Icons On Double-Click");
            btnDesktop.UseVisualStyleBackColor = false;
            btnDesktop.Click += toggleButton_Click;
            // 
            // btnTaskbar
            // 
            btnTaskbar.BackColor = Color.FromArgb(60, 60, 60);
            btnTaskbar.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
            btnTaskbar.FlatAppearance.BorderSize = 0;
            btnTaskbar.FlatStyle = FlatStyle.Flat;
            btnTaskbar.ForeColor = Color.White;
            btnTaskbar.Image = Properties.Resources.TaskbarIcon;
            btnTaskbar.Location = new Point(90, 0);
            btnTaskbar.Name = "btnTaskbar";
            btnTaskbar.Size = new Size(80, 45);
            btnTaskbar.TabIndex = 3;
            toolTip.SetToolTip(btnTaskbar, "Toggle Taskbar Visibility Settings");
            btnTaskbar.UseVisualStyleBackColor = false;
            btnTaskbar.Click += btnTaskbar_Click;
            // 
            // delaySlider
            // 
            delaySlider.BackColor = Color.FromArgb(44, 44, 44);
            delaySlider.LargeChange = 100;
            delaySlider.Location = new Point(20, 338);
            delaySlider.Maximum = 1000;
            delaySlider.Name = "delaySlider";
            delaySlider.Size = new Size(260, 45);
            delaySlider.TabIndex = 13;
            delaySlider.TickStyle = TickStyle.None;
            toolTip.SetToolTip(delaySlider, "Adjust Auto-Hide Delay");
            delaySlider.Scroll += delaySlider_Scroll;
            // 
            // lblDelay
            // 
            lblDelay.ForeColor = Color.White;
            lblDelay.Location = new Point(20, 360);
            lblDelay.Name = "lblDelay";
            lblDelay.Size = new Size(260, 23);
            lblDelay.TabIndex = 14;
            lblDelay.Text = "Delay";
            lblDelay.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // bottomBar
            // 
            bottomBar.BackColor = Color.FromArgb(36, 36, 36);
            bottomBar.Dock = DockStyle.Bottom;
            bottomBar.Location = new Point(0, 403);
            bottomBar.Name = "bottomBar";
            bottomBar.Size = new Size(300, 40);
            bottomBar.TabIndex = 0;
            // 
            // lblDesktop
            // 
            lblDesktop.ForeColor = Color.White;
            lblDesktop.Location = new Point(0, 48);
            lblDesktop.Name = "lblDesktop";
            lblDesktop.Size = new Size(80, 20);
            lblDesktop.TabIndex = 2;
            lblDesktop.Text = "Icons Toggler";
            lblDesktop.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTaskbar
            // 
            lblTaskbar.ForeColor = Color.White;
            lblTaskbar.Location = new Point(90, 48);
            lblTaskbar.Name = "lblTaskbar";
            lblTaskbar.Size = new Size(80, 20);
            lblTaskbar.TabIndex = 4;
            lblTaskbar.Text = "Taskbar";
            lblTaskbar.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // topRowPanel
            // 
            topRowPanel.Controls.Add(btnDesktop);
            topRowPanel.Controls.Add(lblDesktop);
            topRowPanel.Controls.Add(btnTaskbar);
            topRowPanel.Controls.Add(lblTaskbar);
            topRowPanel.Controls.Add(btnAutoHide);
            topRowPanel.Controls.Add(lblTip);
            topRowPanel.Location = new Point(20, 20);
            topRowPanel.Name = "topRowPanel";
            topRowPanel.Size = new Size(260, 75);
            topRowPanel.TabIndex = 15;
            // 
            // btnAutoHide
            // 
            btnAutoHide.BackColor = Color.FromArgb(60, 60, 60);
            btnAutoHide.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
            btnAutoHide.FlatAppearance.BorderSize = 0;
            btnAutoHide.FlatStyle = FlatStyle.Flat;
            btnAutoHide.ForeColor = Color.White;
            btnAutoHide.Image = Properties.Resources.AutoHideIcon;
            btnAutoHide.Location = new Point(180, 0);
            btnAutoHide.Name = "btnAutoHide";
            btnAutoHide.Size = new Size(80, 45);
            btnAutoHide.TabIndex = 5;
            toolTip.SetToolTip(btnAutoHide, "Toggle Customizable Auto-Hide Feature");
            btnAutoHide.UseVisualStyleBackColor = false;
            btnAutoHide.Click += toggleButton_Click;
            // 
            // lblTip
            // 
            lblTip.ForeColor = Color.White;
            lblTip.Location = new Point(180, 48);
            lblTip.Name = "lblTip";
            lblTip.Size = new Size(80, 20);
            lblTip.TabIndex = 6;
            lblTip.Text = "Auto-Hide";
            lblTip.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnDoubleClick);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnCursor);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(btnKey);
            panel1.Controls.Add(label3);
            panel1.Location = new Point(20, 101);
            panel1.Name = "panel1";
            panel1.Size = new Size(260, 75);
            panel1.TabIndex = 16;
            panel1.Visible = false;
            // 
            // btnDoubleClick
            // 
            btnDoubleClick.BackColor = Color.FromArgb(60, 60, 60);
            btnDoubleClick.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
            btnDoubleClick.FlatAppearance.BorderSize = 0;
            btnDoubleClick.FlatStyle = FlatStyle.Flat;
            btnDoubleClick.ForeColor = Color.White;
            btnDoubleClick.Image = Properties.Resources.DoubleClickIcon;
            btnDoubleClick.Location = new Point(0, 0);
            btnDoubleClick.Name = "btnDoubleClick";
            btnDoubleClick.Size = new Size(80, 45);
            btnDoubleClick.TabIndex = 1;
            toolTip.SetToolTip(btnDoubleClick, "Toggle Hide Taskbar On Double-Click");
            btnDoubleClick.UseVisualStyleBackColor = false;
            btnDoubleClick.Click += toggleButton_Click;
            // 
            // label1
            // 
            label1.ForeColor = Color.White;
            label1.Location = new Point(0, 48);
            label1.Name = "label1";
            label1.Size = new Size(80, 20);
            label1.TabIndex = 2;
            label1.Text = "Double-Click";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnCursor
            // 
            btnCursor.BackColor = Color.FromArgb(60, 60, 60);
            btnCursor.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
            btnCursor.FlatAppearance.BorderSize = 0;
            btnCursor.FlatStyle = FlatStyle.Flat;
            btnCursor.ForeColor = Color.White;
            btnCursor.Image = Properties.Resources.CursorIcon;
            btnCursor.Location = new Point(90, 0);
            btnCursor.Name = "btnCursor";
            btnCursor.Size = new Size(80, 45);
            btnCursor.TabIndex = 3;
            toolTip.SetToolTip(btnCursor, "Unhide Taskbar On Cursor Settings");
            btnCursor.UseVisualStyleBackColor = false;
            btnCursor.Click += toggleButton_Click;
            // 
            // label2
            // 
            label2.ForeColor = Color.White;
            label2.Location = new Point(90, 48);
            label2.Name = "label2";
            label2.Size = new Size(80, 20);
            label2.TabIndex = 4;
            label2.Text = "Cursor";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnKey
            // 
            btnKey.BackColor = Color.FromArgb(60, 60, 60);
            btnKey.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
            btnKey.FlatAppearance.BorderSize = 0;
            btnKey.FlatStyle = FlatStyle.Flat;
            btnKey.ForeColor = Color.White;
            btnKey.Image = Properties.Resources.KeyIcon;
            btnKey.Location = new Point(180, 0);
            btnKey.Name = "btnKey";
            btnKey.Size = new Size(80, 45);
            btnKey.TabIndex = 5;
            toolTip.SetToolTip(btnKey, "Toggle Taskbar Visiblity Control On Shortcut");
            btnKey.UseVisualStyleBackColor = false;
            btnKey.Click += toggleButton_Click;
            // 
            // label3
            // 
            label3.ForeColor = Color.White;
            label3.Location = new Point(180, 48);
            label3.Name = "label3";
            label3.Size = new Size(80, 20);
            label3.TabIndex = 6;
            label3.Text = "Key";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            panel2.Controls.Add(lblShortcut);
            panel2.Controls.Add(btnShortcut);
            panel2.Controls.Add(btnEntireBar);
            panel2.Controls.Add(lblEntireBar);
            panel2.Controls.Add(btnRightCorner);
            panel2.Controls.Add(lblRightCorner);
            panel2.Controls.Add(btnLeftCorner);
            panel2.Controls.Add(btnBothCorners);
            panel2.Controls.Add(lblLeftCorner);
            panel2.Controls.Add(lblAnyCorner);
            panel2.Location = new Point(20, 182);
            panel2.Name = "panel2";
            panel2.Size = new Size(260, 150);
            panel2.TabIndex = 16;
            panel2.Visible = false;
            // 
            // lblShortcut
            // 
            lblShortcut.ForeColor = Color.White;
            lblShortcut.Location = new Point(134, 121);
            lblShortcut.Name = "lblShortcut";
            lblShortcut.Size = new Size(80, 20);
            lblShortcut.TabIndex = 10;
            lblShortcut.Text = "Shortcut";
            lblShortcut.TextAlign = ContentAlignment.MiddleCenter;
            lblShortcut.Visible = false;
            // 
            // btnShortcut
            // 
            btnShortcut.BackColor = Color.FromArgb(60, 60, 60);
            btnShortcut.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
            btnShortcut.FlatAppearance.BorderSize = 0;
            btnShortcut.FlatStyle = FlatStyle.Flat;
            btnShortcut.Font = new Font("Segoe UI", 12F);
            btnShortcut.ForeColor = Color.White;
            btnShortcut.Location = new Point(90, 73);
            btnShortcut.Name = "btnShortcut";
            btnShortcut.Size = new Size(167, 45);
            btnShortcut.TabIndex = 9;
            btnShortcut.Text = "Shift+Tab";
            toolTip.SetToolTip(btnShortcut, "Set Shortcut");
            btnShortcut.UseVisualStyleBackColor = false;
            btnShortcut.Visible = false;
            btnShortcut.Click += toggleButton_Click;
            // 
            // btnEntireBar
            // 
            btnEntireBar.BackColor = Color.FromArgb(60, 60, 60);
            btnEntireBar.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
            btnEntireBar.FlatAppearance.BorderSize = 0;
            btnEntireBar.FlatStyle = FlatStyle.Flat;
            btnEntireBar.ForeColor = Color.White;
            btnEntireBar.Image = Properties.Resources.EntireBarIcon;
            btnEntireBar.Location = new Point(0, 73);
            btnEntireBar.Name = "btnEntireBar";
            btnEntireBar.Size = new Size(80, 45);
            btnEntireBar.TabIndex = 7;
            toolTip.SetToolTip(btnEntireBar, "Entire Bar Setting");
            btnEntireBar.UseVisualStyleBackColor = false;
            btnEntireBar.Visible = false;
            btnEntireBar.Click += exclusiveButton_Click;
            // 
            // lblEntireBar
            // 
            lblEntireBar.ForeColor = Color.White;
            lblEntireBar.Location = new Point(0, 121);
            lblEntireBar.Name = "lblEntireBar";
            lblEntireBar.Size = new Size(80, 20);
            lblEntireBar.TabIndex = 8;
            lblEntireBar.Text = "Bottom";
            lblEntireBar.TextAlign = ContentAlignment.MiddleCenter;
            lblEntireBar.Visible = false;
            // 
            // btnRightCorner
            // 
            btnRightCorner.BackColor = Color.FromArgb(60, 60, 60);
            btnRightCorner.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
            btnRightCorner.FlatAppearance.BorderSize = 0;
            btnRightCorner.FlatStyle = FlatStyle.Flat;
            btnRightCorner.ForeColor = Color.White;
            btnRightCorner.Image = Properties.Resources.RightCornerIcon;
            btnRightCorner.Location = new Point(90, 0);
            btnRightCorner.Name = "btnRightCorner";
            btnRightCorner.Size = new Size(80, 45);
            btnRightCorner.TabIndex = 1;
            toolTip.SetToolTip(btnRightCorner, "Right Corner Setting");
            btnRightCorner.UseVisualStyleBackColor = false;
            btnRightCorner.Visible = false;
            btnRightCorner.Click += exclusiveButton_Click;
            // 
            // lblRightCorner
            // 
            lblRightCorner.ForeColor = Color.White;
            lblRightCorner.Location = new Point(90, 48);
            lblRightCorner.Name = "lblRightCorner";
            lblRightCorner.Size = new Size(80, 20);
            lblRightCorner.TabIndex = 2;
            lblRightCorner.Text = "Right-Corner";
            lblRightCorner.TextAlign = ContentAlignment.MiddleCenter;
            lblRightCorner.Visible = false;
            // 
            // btnLeftCorner
            // 
            btnLeftCorner.BackColor = Color.FromArgb(60, 60, 60);
            btnLeftCorner.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
            btnLeftCorner.FlatAppearance.BorderSize = 0;
            btnLeftCorner.FlatStyle = FlatStyle.Flat;
            btnLeftCorner.ForeColor = Color.White;
            btnLeftCorner.Image = Properties.Resources.LeftCornerIcon;
            btnLeftCorner.Location = new Point(0, 0);
            btnLeftCorner.Name = "btnLeftCorner";
            btnLeftCorner.Size = new Size(80, 45);
            btnLeftCorner.TabIndex = 3;
            toolTip.SetToolTip(btnLeftCorner, "Left Corner Setting");
            btnLeftCorner.UseVisualStyleBackColor = false;
            btnLeftCorner.Visible = false;
            btnLeftCorner.Click += exclusiveButton_Click;
            // 
            // btnBothCorners
            // 
            btnBothCorners.BackColor = Color.FromArgb(60, 60, 60);
            btnBothCorners.FlatAppearance.BorderColor = SystemColors.ButtonShadow;
            btnBothCorners.FlatAppearance.BorderSize = 0;
            btnBothCorners.FlatStyle = FlatStyle.Flat;
            btnBothCorners.ForeColor = Color.White;
            btnBothCorners.Image = Properties.Resources.BothCornersIcon;
            btnBothCorners.Location = new Point(180, 0);
            btnBothCorners.Name = "btnBothCorners";
            btnBothCorners.Size = new Size(80, 45);
            btnBothCorners.TabIndex = 5;
            toolTip.SetToolTip(btnBothCorners, "Any Corner Setting");
            btnBothCorners.UseVisualStyleBackColor = false;
            btnBothCorners.Visible = false;
            btnBothCorners.Click += exclusiveButton_Click;
            // 
            // lblLeftCorner
            // 
            lblLeftCorner.ForeColor = Color.White;
            lblLeftCorner.Location = new Point(0, 48);
            lblLeftCorner.Name = "lblLeftCorner";
            lblLeftCorner.Size = new Size(80, 20);
            lblLeftCorner.TabIndex = 4;
            lblLeftCorner.Text = "Left-Corner";
            lblLeftCorner.TextAlign = ContentAlignment.MiddleCenter;
            lblLeftCorner.Visible = false;
            // 
            // lblAnyCorner
            // 
            lblAnyCorner.ForeColor = Color.White;
            lblAnyCorner.Location = new Point(180, 48);
            lblAnyCorner.Name = "lblAnyCorner";
            lblAnyCorner.Size = new Size(80, 20);
            lblAnyCorner.TabIndex = 6;
            lblAnyCorner.Text = "Any-Corner";
            lblAnyCorner.TextAlign = ContentAlignment.MiddleCenter;
            lblAnyCorner.Visible = false;
            // 
            // SettingsForm
            // 
            BackColor = Color.FromArgb(44, 44, 44);
            ClientSize = new Size(300, 443);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(topRowPanel);
            Controls.Add(lblDelay);
            Controls.Add(bottomBar);
            Controls.Add(delaySlider);
            ForeColor = Color.FromArgb(30, 30, 30);
            FormBorderStyle = FormBorderStyle.None;
            Name = "SettingsForm";
            TransparencyKey = Color.Magenta;
            ((System.ComponentModel.ISupportInitialize)delaySlider).EndInit();
            topRowPanel.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel panel1;
        private Button btnDoubleClick;
        private Label label1;
        private Button btnCursor;
        private Label label2;
        private Button btnKey;
        private Label label3;
        private Panel panel2;
        private Button btnEntireBar;
        private Label lblEntireBar;
        private Button btnRightCorner;
        private Label lblRightCorner;
        private Button btnLeftCorner;
        private Label lblLeftCorner;
        private Button btnBothCorners;
        private Label lblAnyCorner;
        private Label lblShortcut;
        private Button btnShortcut;
        private Button btnAutoHide;
        private Label lblTip;
    }
}
