namespace Mirador
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.CheckBox checkBoxEnableEnhancedTaskbarAutohide;
        private System.Windows.Forms.CheckBox checkBoxEnableDesktopIconToggle;

        private System.Windows.Forms.GroupBox groupBoxTaskbarAutohideSettings;
        private System.Windows.Forms.CheckBox checkBoxDoubleClickToHideTaskbar;
        private System.Windows.Forms.NumericUpDown numericUpDownDoubleClickSensitivity;
        private System.Windows.Forms.CheckBox checkBoxUnhideByDraggingUpwards;
        private System.Windows.Forms.NumericUpDown numericUpDownDragThreshold;
        private System.Windows.Forms.CheckBox checkBoxUnhideByCursorInCorners;
        private System.Windows.Forms.ComboBox comboBoxCorners;
        private System.Windows.Forms.NumericUpDown numericUpDownCornerThreshold;
        private System.Windows.Forms.NumericUpDown numericUpDownCornerDelay;
        private System.Windows.Forms.CheckBox checkBoxTemporarilyShowForNotifications;
        private System.Windows.Forms.NumericUpDown numericUpDownNotificationDuration;
        private System.Windows.Forms.CheckBox checkBoxLockUnlockWithDoubleClick;
        private System.Windows.Forms.Label labelLockState;
        private System.Windows.Forms.CheckBox checkBoxHideUnhideWithShortcut;
        private System.Windows.Forms.TextBox textBoxShortcutKey;
        private System.Windows.Forms.Button buttonCustomizeShortcut;

        private System.Windows.Forms.Button buttonSave;

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
            checkBoxEnableEnhancedTaskbarAutohide = new CheckBox();
            checkBoxEnableDesktopIconToggle = new CheckBox();
            groupBoxTaskbarAutohideSettings = new GroupBox();
            checkBoxDoubleClickToHideTaskbar = new CheckBox();
            numericUpDownDoubleClickSensitivity = new NumericUpDown();
            checkBoxUnhideByDraggingUpwards = new CheckBox();
            numericUpDownDragThreshold = new NumericUpDown();
            checkBoxUnhideByCursorInCorners = new CheckBox();
            comboBoxCorners = new ComboBox();
            numericUpDownCornerThreshold = new NumericUpDown();
            numericUpDownCornerDelay = new NumericUpDown();
            checkBoxTemporarilyShowForNotifications = new CheckBox();
            numericUpDownNotificationDuration = new NumericUpDown();
            checkBoxLockUnlockWithDoubleClick = new CheckBox();
            labelLockState = new Label();
            checkBoxHideUnhideWithShortcut = new CheckBox();
            textBoxShortcutKey = new TextBox();
            buttonCustomizeShortcut = new Button();
            buttonSave = new Button();
            groupBoxTaskbarAutohideSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDoubleClickSensitivity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDragThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCornerThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCornerDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownNotificationDuration).BeginInit();
            SuspendLayout();
            // 
            // checkBoxEnableEnhancedTaskbarAutohide
            // 
            checkBoxEnableEnhancedTaskbarAutohide.Location = new Point(12, 12);
            checkBoxEnableEnhancedTaskbarAutohide.Name = "checkBoxEnableEnhancedTaskbarAutohide";
            checkBoxEnableEnhancedTaskbarAutohide.Size = new Size(260, 20);
            checkBoxEnableEnhancedTaskbarAutohide.TabIndex = 0;
            checkBoxEnableEnhancedTaskbarAutohide.Text = "Enable Enhanced Taskbar Autohide";
            checkBoxEnableEnhancedTaskbarAutohide.CheckedChanged += checkBoxEnableEnhancedTaskbarAutohide_CheckedChanged;
            // 
            // checkBoxEnableDesktopIconToggle
            // 
            checkBoxEnableDesktopIconToggle.Location = new Point(12, 38);
            checkBoxEnableDesktopIconToggle.Name = "checkBoxEnableDesktopIconToggle";
            checkBoxEnableDesktopIconToggle.Size = new Size(260, 20);
            checkBoxEnableDesktopIconToggle.TabIndex = 1;
            checkBoxEnableDesktopIconToggle.Text = "Enable Desktop Icon Toggle";
            // 
            // groupBoxTaskbarAutohideSettings
            // 
            groupBoxTaskbarAutohideSettings.Controls.Add(checkBoxDoubleClickToHideTaskbar);
            groupBoxTaskbarAutohideSettings.Controls.Add(numericUpDownDoubleClickSensitivity);
            groupBoxTaskbarAutohideSettings.Controls.Add(checkBoxUnhideByDraggingUpwards);
            groupBoxTaskbarAutohideSettings.Controls.Add(numericUpDownDragThreshold);
            groupBoxTaskbarAutohideSettings.Controls.Add(checkBoxUnhideByCursorInCorners);
            groupBoxTaskbarAutohideSettings.Controls.Add(comboBoxCorners);
            groupBoxTaskbarAutohideSettings.Controls.Add(numericUpDownCornerThreshold);
            groupBoxTaskbarAutohideSettings.Controls.Add(numericUpDownCornerDelay);
            groupBoxTaskbarAutohideSettings.Controls.Add(checkBoxTemporarilyShowForNotifications);
            groupBoxTaskbarAutohideSettings.Controls.Add(numericUpDownNotificationDuration);
            groupBoxTaskbarAutohideSettings.Controls.Add(checkBoxLockUnlockWithDoubleClick);
            groupBoxTaskbarAutohideSettings.Controls.Add(labelLockState);
            groupBoxTaskbarAutohideSettings.Controls.Add(checkBoxHideUnhideWithShortcut);
            groupBoxTaskbarAutohideSettings.Controls.Add(textBoxShortcutKey);
            groupBoxTaskbarAutohideSettings.Controls.Add(buttonCustomizeShortcut);
            groupBoxTaskbarAutohideSettings.Location = new Point(12, 64);
            groupBoxTaskbarAutohideSettings.Name = "groupBoxTaskbarAutohideSettings";
            groupBoxTaskbarAutohideSettings.Size = new Size(360, 400);
            groupBoxTaskbarAutohideSettings.TabIndex = 2;
            groupBoxTaskbarAutohideSettings.TabStop = false;
            groupBoxTaskbarAutohideSettings.Text = "Taskbar Autohide Settings";
            groupBoxTaskbarAutohideSettings.Visible = false;
            // 
            // checkBoxDoubleClickToHideTaskbar
            // 
            checkBoxDoubleClickToHideTaskbar.Location = new Point(6, 20);
            checkBoxDoubleClickToHideTaskbar.Name = "checkBoxDoubleClickToHideTaskbar";
            checkBoxDoubleClickToHideTaskbar.Size = new Size(200, 20);
            checkBoxDoubleClickToHideTaskbar.TabIndex = 0;
            checkBoxDoubleClickToHideTaskbar.Text = "Double-Click to Hide Taskbar";
            // 
            // numericUpDownDoubleClickSensitivity
            // 
            numericUpDownDoubleClickSensitivity.Location = new Point(220, 20);
            numericUpDownDoubleClickSensitivity.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownDoubleClickSensitivity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownDoubleClickSensitivity.Name = "numericUpDownDoubleClickSensitivity";
            numericUpDownDoubleClickSensitivity.Size = new Size(120, 23);
            numericUpDownDoubleClickSensitivity.TabIndex = 1;
            numericUpDownDoubleClickSensitivity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // checkBoxUnhideByDraggingUpwards
            // 
            checkBoxUnhideByDraggingUpwards.Location = new Point(6, 50);
            checkBoxUnhideByDraggingUpwards.Name = "checkBoxUnhideByDraggingUpwards";
            checkBoxUnhideByDraggingUpwards.Size = new Size(200, 20);
            checkBoxUnhideByDraggingUpwards.TabIndex = 2;
            checkBoxUnhideByDraggingUpwards.Text = "Unhide by Dragging Upwards";
            // 
            // numericUpDownDragThreshold
            // 
            numericUpDownDragThreshold.Location = new Point(220, 50);
            numericUpDownDragThreshold.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownDragThreshold.Name = "numericUpDownDragThreshold";
            numericUpDownDragThreshold.Size = new Size(120, 23);
            numericUpDownDragThreshold.TabIndex = 3;
            numericUpDownDragThreshold.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // checkBoxUnhideByCursorInCorners
            // 
            checkBoxUnhideByCursorInCorners.Location = new Point(6, 80);
            checkBoxUnhideByCursorInCorners.Name = "checkBoxUnhideByCursorInCorners";
            checkBoxUnhideByCursorInCorners.Size = new Size(200, 20);
            checkBoxUnhideByCursorInCorners.TabIndex = 4;
            checkBoxUnhideByCursorInCorners.Text = "Unhide by Cursor in Bottom Corners";
            // 
            // comboBoxCorners
            // 
            comboBoxCorners.Items.AddRange(new object[] { "RightBottom", "LeftBottom", "BothBottom", "EntireBottom" });
            comboBoxCorners.Location = new Point(220, 80);
            comboBoxCorners.Name = "comboBoxCorners";
            comboBoxCorners.Size = new Size(120, 23);
            comboBoxCorners.TabIndex = 5;
            // 
            // numericUpDownCornerThreshold
            // 
            numericUpDownCornerThreshold.Location = new Point(220, 110);
            numericUpDownCornerThreshold.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownCornerThreshold.Name = "numericUpDownCornerThreshold";
            numericUpDownCornerThreshold.Size = new Size(120, 23);
            numericUpDownCornerThreshold.TabIndex = 6;
            numericUpDownCornerThreshold.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numericUpDownCornerDelay
            // 
            numericUpDownCornerDelay.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDownCornerDelay.Location = new Point(220, 140);
            numericUpDownCornerDelay.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numericUpDownCornerDelay.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDownCornerDelay.Name = "numericUpDownCornerDelay";
            numericUpDownCornerDelay.Size = new Size(120, 23);
            numericUpDownCornerDelay.TabIndex = 7;
            numericUpDownCornerDelay.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // checkBoxTemporarilyShowForNotifications
            // 
            checkBoxTemporarilyShowForNotifications.Location = new Point(6, 170);
            checkBoxTemporarilyShowForNotifications.Name = "checkBoxTemporarilyShowForNotifications";
            checkBoxTemporarilyShowForNotifications.Size = new Size(200, 20);
            checkBoxTemporarilyShowForNotifications.TabIndex = 8;
            checkBoxTemporarilyShowForNotifications.Text = "Temporarily Show for Notifications";
            // 
            // numericUpDownNotificationDuration
            // 
            numericUpDownNotificationDuration.Location = new Point(220, 170);
            numericUpDownNotificationDuration.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            numericUpDownNotificationDuration.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownNotificationDuration.Name = "numericUpDownNotificationDuration";
            numericUpDownNotificationDuration.Size = new Size(120, 23);
            numericUpDownNotificationDuration.TabIndex = 9;
            numericUpDownNotificationDuration.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // checkBoxLockUnlockWithDoubleClick
            // 
            checkBoxLockUnlockWithDoubleClick.Location = new Point(6, 200);
            checkBoxLockUnlockWithDoubleClick.Name = "checkBoxLockUnlockWithDoubleClick";
            checkBoxLockUnlockWithDoubleClick.Size = new Size(200, 20);
            checkBoxLockUnlockWithDoubleClick.TabIndex = 10;
            checkBoxLockUnlockWithDoubleClick.Text = "Lock/Unlock with Double-Click";
            // 
            // labelLockState
            // 
            labelLockState.Location = new Point(220, 200);
            labelLockState.Name = "labelLockState";
            labelLockState.Size = new Size(120, 20);
            labelLockState.TabIndex = 11;
            labelLockState.Text = "State: Unlocked";
            // 
            // checkBoxHideUnhideWithShortcut
            // 
            checkBoxHideUnhideWithShortcut.Location = new Point(6, 230);
            checkBoxHideUnhideWithShortcut.Name = "checkBoxHideUnhideWithShortcut";
            checkBoxHideUnhideWithShortcut.Size = new Size(200, 20);
            checkBoxHideUnhideWithShortcut.TabIndex = 12;
            checkBoxHideUnhideWithShortcut.Text = "Hide/Unhide with Shortcut Key";
            // 
            // textBoxShortcutKey
            // 
            textBoxShortcutKey.Location = new Point(220, 230);
            textBoxShortcutKey.Name = "textBoxShortcutKey";
            textBoxShortcutKey.Size = new Size(80, 23);
            textBoxShortcutKey.TabIndex = 13;
            textBoxShortcutKey.Text = "Shift + Tab";
            // 
            // buttonCustomizeShortcut
            // 
            buttonCustomizeShortcut.Location = new Point(310, 230);
            buttonCustomizeShortcut.Name = "buttonCustomizeShortcut";
            buttonCustomizeShortcut.Size = new Size(30, 20);
            buttonCustomizeShortcut.TabIndex = 14;
            buttonCustomizeShortcut.Text = "...";
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(297, 527);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(75, 23);
            buttonSave.TabIndex = 3;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // SettingsForm
            // 
            ClientSize = new Size(383, 600);
            Controls.Add(checkBoxEnableEnhancedTaskbarAutohide);
            Controls.Add(checkBoxEnableDesktopIconToggle);
            Controls.Add(groupBoxTaskbarAutohideSettings);
            Controls.Add(buttonSave);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "SettingsForm";
            Text = "Settings";
            groupBoxTaskbarAutohideSettings.ResumeLayout(false);
            groupBoxTaskbarAutohideSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDoubleClickSensitivity).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDragThreshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCornerThreshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCornerDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownNotificationDuration).EndInit();
            ResumeLayout(false);
        }

        private void checkBoxEnableEnhancedTaskbarAutohide_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxTaskbarAutohideSettings.Visible = checkBoxEnableEnhancedTaskbarAutohide.Checked;
        }
    }
}
