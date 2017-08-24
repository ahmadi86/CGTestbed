namespace ExampleFilters
{
    partial class H264PropertiesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmboLevel = new System.Windows.Forms.ComboBox();
            this.cmboProfile = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbAutoBitrate = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbBitrate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmboRateControl = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbDeblocking = new System.Windows.Forms.CheckBox();
            this.cbGOP = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPPeriod = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbIDRPeriod = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmboMBEncoding = new System.Windows.Forms.ComboBox();
            this.timerCheck = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmboLevel);
            this.groupBox1.Controls.Add(this.cmboProfile);
            this.groupBox1.Location = new System.Drawing.Point(3, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Basic Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Level:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Profile:";
            // 
            // cmboLevel
            // 
            this.cmboLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLevel.FormattingEnabled = true;
            this.cmboLevel.Location = new System.Drawing.Point(73, 58);
            this.cmboLevel.Name = "cmboLevel";
            this.cmboLevel.Size = new System.Drawing.Size(121, 21);
            this.cmboLevel.TabIndex = 1;
            this.cmboLevel.SelectedIndexChanged += new System.EventHandler(this.HandleChanges);
            // 
            // cmboProfile
            // 
            this.cmboProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboProfile.FormattingEnabled = true;
            this.cmboProfile.Location = new System.Drawing.Point(74, 24);
            this.cmboProfile.Name = "cmboProfile";
            this.cmboProfile.Size = new System.Drawing.Size(121, 21);
            this.cmboProfile.TabIndex = 0;
            this.cmboProfile.SelectedIndexChanged += new System.EventHandler(this.HandleChanges);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbAutoBitrate);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbBitrate);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cmboRateControl);
            this.groupBox2.Location = new System.Drawing.Point(209, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(215, 100);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Bitrate";
            // 
            // cbAutoBitrate
            // 
            this.cbAutoBitrate.AutoSize = true;
            this.cbAutoBitrate.Location = new System.Drawing.Point(9, 74);
            this.cbAutoBitrate.Name = "cbAutoBitrate";
            this.cbAutoBitrate.Size = new System.Drawing.Size(127, 17);
            this.cbAutoBitrate.TabIndex = 7;
            this.cbAutoBitrate.Text = "Auto configure bitrate";
            this.cbAutoBitrate.UseVisualStyleBackColor = true;
            this.cbAutoBitrate.CheckedChanged += new System.EventHandler(this.HandleChanges);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Bitrate (kbps):";
            // 
            // tbBitrate
            // 
            this.tbBitrate.Location = new System.Drawing.Point(80, 46);
            this.tbBitrate.Name = "tbBitrate";
            this.tbBitrate.Size = new System.Drawing.Size(121, 20);
            this.tbBitrate.TabIndex = 5;
            this.tbBitrate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbBitrate.TextChanged += new System.EventHandler(this.HandleChanges);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Rate Control:";
            // 
            // cmboRateControl
            // 
            this.cmboRateControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboRateControl.FormattingEnabled = true;
            this.cmboRateControl.Location = new System.Drawing.Point(80, 17);
            this.cmboRateControl.Name = "cmboRateControl";
            this.cmboRateControl.Size = new System.Drawing.Size(121, 21);
            this.cmboRateControl.TabIndex = 3;
            this.cmboRateControl.SelectedIndexChanged += new System.EventHandler(this.HandleChanges);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbDeblocking);
            this.groupBox3.Controls.Add(this.cbGOP);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.tbPPeriod);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.tbIDRPeriod);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.cmboMBEncoding);
            this.groupBox3.Location = new System.Drawing.Point(3, 113);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(421, 72);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Advanced";
            // 
            // cbDeblocking
            // 
            this.cbDeblocking.AutoSize = true;
            this.cbDeblocking.Location = new System.Drawing.Point(74, 45);
            this.cbDeblocking.Name = "cbDeblocking";
            this.cbDeblocking.Size = new System.Drawing.Size(80, 17);
            this.cbDeblocking.TabIndex = 12;
            this.cbDeblocking.Text = "Deblocking";
            this.cbDeblocking.UseVisualStyleBackColor = true;
            this.cbDeblocking.CheckedChanged += new System.EventHandler(this.HandleChanges);
            // 
            // cbGOP
            // 
            this.cbGOP.AutoSize = true;
            this.cbGOP.Location = new System.Drawing.Point(12, 43);
            this.cbGOP.Name = "cbGOP";
            this.cbGOP.Size = new System.Drawing.Size(49, 17);
            this.cbGOP.TabIndex = 11;
            this.cbGOP.Text = "GOP";
            this.cbGOP.UseVisualStyleBackColor = true;
            this.cbGOP.CheckedChanged += new System.EventHandler(this.HandleChanges);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(268, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "P Period:";
            // 
            // tbPPeriod
            // 
            this.tbPPeriod.Location = new System.Drawing.Point(346, 41);
            this.tbPPeriod.Name = "tbPPeriod";
            this.tbPPeriod.Size = new System.Drawing.Size(61, 20);
            this.tbPPeriod.TabIndex = 9;
            this.tbPPeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbPPeriod.TextChanged += new System.EventHandler(this.HandleChanges);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(268, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "IDR Period:";
            // 
            // tbIDRPeriod
            // 
            this.tbIDRPeriod.Location = new System.Drawing.Point(346, 15);
            this.tbIDRPeriod.Name = "tbIDRPeriod";
            this.tbIDRPeriod.Size = new System.Drawing.Size(61, 20);
            this.tbIDRPeriod.TabIndex = 7;
            this.tbIDRPeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbIDRPeriod.TextChanged += new System.EventHandler(this.HandleChanges);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "MB Encoding:";
            // 
            // cmboMBEncoding
            // 
            this.cmboMBEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboMBEncoding.FormattingEnabled = true;
            this.cmboMBEncoding.Location = new System.Drawing.Point(84, 16);
            this.cmboMBEncoding.Name = "cmboMBEncoding";
            this.cmboMBEncoding.Size = new System.Drawing.Size(121, 21);
            this.cmboMBEncoding.TabIndex = 3;
            this.cmboMBEncoding.SelectedIndexChanged += new System.EventHandler(this.HandleChanges);
            // 
            // timerCheck
            // 
            this.timerCheck.Interval = 1000;
            this.timerCheck.Tick += new System.EventHandler(this.timerCheck_Tick);
            // 
            // H264PropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 192);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "H264PropertiesForm";
            this.Text = "Properties";
            this.Title = "Properties";
            this.Load += new System.EventHandler(this.H264PropertiesForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmboProfile;
        private System.Windows.Forms.ComboBox cmboLevel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmboRateControl;
        private System.Windows.Forms.TextBox tbBitrate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbAutoBitrate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmboMBEncoding;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbPPeriod;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbIDRPeriod;
        private System.Windows.Forms.CheckBox cbDeblocking;
        private System.Windows.Forms.CheckBox cbGOP;
        private System.Windows.Forms.Timer timerCheck;

    }
}