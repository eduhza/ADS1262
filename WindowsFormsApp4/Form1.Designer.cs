namespace WindowsFormsApp4
{
    partial class BioMensurae
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BioMensurae));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.OpenFile_Button = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.StatusText = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Plot_Button = new System.Windows.Forms.Button();
            this.Calib_Button = new System.Windows.Forms.Button();
            this.EventLogTextBox = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TipoBox = new System.Windows.Forms.ComboBox();
            this.SerialPortsBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.connect_label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Connect_Button = new System.Windows.Forms.Button();
            this.Stop_Button = new System.Windows.Forms.Button();
            this.Record_Button = new System.Windows.Forms.Button();
            this.clock_display = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip4 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip5 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip6 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.Ch3CheckBox = new System.Windows.Forms.CheckBox();
            this.Ch2CheckBox = new System.Windows.Forms.CheckBox();
            this.Ch1CheckBox = new System.Windows.Forms.CheckBox();
            this.formsPlot1 = new ScottPlot.FormsPlot();
            this.label6 = new System.Windows.Forms.Label();
            this.COPFormsPlot = new ScottPlot.FormsPlot();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.RecordTime_TextBox = new System.Windows.Forms.TextBox();
            this.Name_TextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.PesoText = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.NewtonText = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            // 
            // OpenFile_Button
            // 
            this.OpenFile_Button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("OpenFile_Button.BackgroundImage")));
            this.OpenFile_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.OpenFile_Button.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.OpenFile_Button.FlatAppearance.BorderSize = 0;
            this.OpenFile_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OpenFile_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenFile_Button.ForeColor = System.Drawing.Color.Black;
            this.OpenFile_Button.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.OpenFile_Button.Location = new System.Drawing.Point(73, 17);
            this.OpenFile_Button.Name = "OpenFile_Button";
            this.OpenFile_Button.Size = new System.Drawing.Size(43, 31);
            this.OpenFile_Button.TabIndex = 18;
            this.OpenFile_Button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.OpenFile_Button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolTip5.SetToolTip(this.OpenFile_Button, "Abrir coleta");
            this.OpenFile_Button.UseVisualStyleBackColor = true;
            this.OpenFile_Button.Click += new System.EventHandler(this.OpenFile_Button_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.StatusText);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.EventLogTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.TipoBox);
            this.groupBox1.Controls.Add(this.SerialPortsBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.connect_label);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.Connect_Button);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(7, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(213, 434);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configurações";
            // 
            // StatusText
            // 
            this.StatusText.AutoSize = true;
            this.StatusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.StatusText.ForeColor = System.Drawing.Color.Red;
            this.StatusText.Location = new System.Drawing.Point(171, 125);
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(30, 13);
            this.StatusText.TabIndex = 37;
            this.StatusText.Text = "OFF";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.OpenFile_Button);
            this.groupBox3.Controls.Add(this.Plot_Button);
            this.groupBox3.Controls.Add(this.Calib_Button);
            this.groupBox3.Location = new System.Drawing.Point(12, 135);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(187, 60);
            this.groupBox3.TabIndex = 46;
            this.groupBox3.TabStop = false;
            // 
            // Plot_Button
            // 
            this.Plot_Button.BackColor = System.Drawing.Color.Transparent;
            this.Plot_Button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Plot_Button.BackgroundImage")));
            this.Plot_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Plot_Button.Cursor = System.Windows.Forms.Cursors.Default;
            this.Plot_Button.Enabled = false;
            this.Plot_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Plot_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.Plot_Button.ForeColor = System.Drawing.Color.Transparent;
            this.Plot_Button.Location = new System.Drawing.Point(6, 17);
            this.Plot_Button.Name = "Plot_Button";
            this.Plot_Button.Size = new System.Drawing.Size(43, 31);
            this.Plot_Button.TabIndex = 34;
            this.toolTip2.SetToolTip(this.Plot_Button, "Plotar gráfico");
            this.Plot_Button.UseVisualStyleBackColor = false;
            this.Plot_Button.Click += new System.EventHandler(this.Plot_Button_Click);
            // 
            // Calib_Button
            // 
            this.Calib_Button.BackColor = System.Drawing.Color.Transparent;
            this.Calib_Button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Calib_Button.BackgroundImage")));
            this.Calib_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Calib_Button.FlatAppearance.BorderSize = 0;
            this.Calib_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Calib_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Calib_Button.Location = new System.Drawing.Point(138, 17);
            this.Calib_Button.Name = "Calib_Button";
            this.Calib_Button.Size = new System.Drawing.Size(43, 31);
            this.Calib_Button.TabIndex = 25;
            this.Calib_Button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Calib_Button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolTip6.SetToolTip(this.Calib_Button, "Calibrar Plataformas");
            this.Calib_Button.UseVisualStyleBackColor = true;
            this.Calib_Button.Click += new System.EventHandler(this.Calib_Button_Click);
            // 
            // EventLogTextBox
            // 
            this.EventLogTextBox.BackColor = System.Drawing.Color.White;
            this.EventLogTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EventLogTextBox.Location = new System.Drawing.Point(12, 211);
            this.EventLogTextBox.Name = "EventLogTextBox";
            this.EventLogTextBox.ReadOnly = true;
            this.EventLogTextBox.Size = new System.Drawing.Size(187, 217);
            this.EventLogTextBox.TabIndex = 45;
            this.EventLogTextBox.Text = "";
            this.EventLogTextBox.TextChanged += new System.EventHandler(this.EventLogTextBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(2, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 43;
            // 
            // TipoBox
            // 
            this.TipoBox.AllowDrop = true;
            this.TipoBox.DisplayMember = "0";
            this.TipoBox.DropDownHeight = 103;
            this.TipoBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TipoBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TipoBox.FormattingEnabled = true;
            this.TipoBox.IntegralHeight = false;
            this.TipoBox.Items.AddRange(new object[] {
            "Plataforma 1D",
            "Plataforma 3D",
            "Bilateral"});
            this.TipoBox.Location = new System.Drawing.Point(72, 68);
            this.TipoBox.Name = "TipoBox";
            this.TipoBox.Size = new System.Drawing.Size(132, 24);
            this.TipoBox.TabIndex = 35;
            this.TipoBox.ValueMember = "0";
            this.TipoBox.SelectedIndexChanged += new System.EventHandler(this.TipoBox_SelectedIndexChanged);
            // 
            // SerialPortsBox
            // 
            this.SerialPortsBox.AllowDrop = true;
            this.SerialPortsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SerialPortsBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SerialPortsBox.FormattingEnabled = true;
            this.SerialPortsBox.Location = new System.Drawing.Point(72, 32);
            this.SerialPortsBox.Name = "SerialPortsBox";
            this.SerialPortsBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SerialPortsBox.Size = new System.Drawing.Size(132, 24);
            this.SerialPortsBox.TabIndex = 36;
            this.SerialPortsBox.SelectedIndexChanged += new System.EventHandler(this.SerialPortsBox_SelectedIndexChanged);
            this.SerialPortsBox.Click += new System.EventHandler(this.SerialPortsBox_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 16);
            this.label1.TabIndex = 37;
            this.label1.Text = "Porta:";
            // 
            // connect_label
            // 
            this.connect_label.AutoSize = true;
            this.connect_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connect_label.Location = new System.Drawing.Point(9, 103);
            this.connect_label.Name = "connect_label";
            this.connect_label.Size = new System.Drawing.Size(65, 16);
            this.connect_label.TabIndex = 39;
            this.connect_label.Text = "Conectar:";
            this.connect_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 16);
            this.label2.TabIndex = 39;
            this.label2.Text = "Tipo:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Connect_Button
            // 
            this.Connect_Button.BackColor = System.Drawing.Color.ForestGreen;
            this.Connect_Button.Enabled = false;
            this.Connect_Button.FlatAppearance.BorderSize = 0;
            this.Connect_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Connect_Button.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Connect_Button.Location = new System.Drawing.Point(109, 102);
            this.Connect_Button.Name = "Connect_Button";
            this.Connect_Button.Size = new System.Drawing.Size(95, 20);
            this.Connect_Button.TabIndex = 41;
            this.toolTip1.SetToolTip(this.Connect_Button, "Conect");
            this.Connect_Button.UseVisualStyleBackColor = false;
            this.Connect_Button.Click += new System.EventHandler(this.Connect_Button_Click);
            // 
            // Stop_Button
            // 
            this.Stop_Button.BackColor = System.Drawing.Color.Transparent;
            this.Stop_Button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Stop_Button.BackgroundImage")));
            this.Stop_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Stop_Button.Enabled = false;
            this.Stop_Button.FlatAppearance.BorderSize = 0;
            this.Stop_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Stop_Button.Location = new System.Drawing.Point(143, 116);
            this.Stop_Button.Name = "Stop_Button";
            this.Stop_Button.Size = new System.Drawing.Size(43, 31);
            this.Stop_Button.TabIndex = 33;
            this.toolTip4.SetToolTip(this.Stop_Button, "Finalizar coleta");
            this.Stop_Button.UseVisualStyleBackColor = false;
            this.Stop_Button.Click += new System.EventHandler(this.Stop_Button_Click);
            // 
            // Record_Button
            // 
            this.Record_Button.BackColor = System.Drawing.Color.Transparent;
            this.Record_Button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Record_Button.BackgroundImage")));
            this.Record_Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Record_Button.Enabled = false;
            this.Record_Button.FlatAppearance.BorderSize = 0;
            this.Record_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Record_Button.Location = new System.Drawing.Point(56, 116);
            this.Record_Button.Name = "Record_Button";
            this.Record_Button.Size = new System.Drawing.Size(43, 31);
            this.Record_Button.TabIndex = 32;
            this.toolTip3.SetToolTip(this.Record_Button, "Iniciar coleta");
            this.Record_Button.UseVisualStyleBackColor = false;
            this.Record_Button.Click += new System.EventHandler(this.Record_Button_Click);
            // 
            // clock_display
            // 
            this.clock_display.AutoSize = true;
            this.clock_display.Location = new System.Drawing.Point(516, 11);
            this.clock_display.Name = "clock_display";
            this.clock_display.Size = new System.Drawing.Size(71, 20);
            this.clock_display.TabIndex = 2;
            this.clock_display.Text = "00:00:00";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.Ch3CheckBox);
            this.panel1.Controls.Add(this.Ch2CheckBox);
            this.panel1.Controls.Add(this.Ch1CheckBox);
            this.panel1.Controls.Add(this.formsPlot1);
            this.panel1.Controls.Add(this.clock_display);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Location = new System.Drawing.Point(226, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(608, 273);
            this.panel1.TabIndex = 21;
            // 
            // Ch3CheckBox
            // 
            this.Ch3CheckBox.AutoSize = true;
            this.Ch3CheckBox.Checked = true;
            this.Ch3CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Ch3CheckBox.Location = new System.Drawing.Point(529, 91);
            this.Ch3CheckBox.Name = "Ch3CheckBox";
            this.Ch3CheckBox.Size = new System.Drawing.Size(61, 24);
            this.Ch3CheckBox.TabIndex = 27;
            this.Ch3CheckBox.Text = "Ch 3";
            this.Ch3CheckBox.UseVisualStyleBackColor = true;
            this.Ch3CheckBox.CheckedChanged += new System.EventHandler(this.MzCheckBox_CheckedChanged);
            // 
            // Ch2CheckBox
            // 
            this.Ch2CheckBox.AutoSize = true;
            this.Ch2CheckBox.Checked = true;
            this.Ch2CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Ch2CheckBox.Location = new System.Drawing.Point(529, 71);
            this.Ch2CheckBox.Name = "Ch2CheckBox";
            this.Ch2CheckBox.Size = new System.Drawing.Size(61, 24);
            this.Ch2CheckBox.TabIndex = 26;
            this.Ch2CheckBox.Text = "Ch 2";
            this.Ch2CheckBox.UseVisualStyleBackColor = true;
            this.Ch2CheckBox.CheckedChanged += new System.EventHandler(this.MxCheckBox_CheckedChanged);
            // 
            // Ch1CheckBox
            // 
            this.Ch1CheckBox.AutoSize = true;
            this.Ch1CheckBox.Checked = true;
            this.Ch1CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Ch1CheckBox.Location = new System.Drawing.Point(529, 51);
            this.Ch1CheckBox.Name = "Ch1CheckBox";
            this.Ch1CheckBox.Size = new System.Drawing.Size(61, 24);
            this.Ch1CheckBox.TabIndex = 25;
            this.Ch1CheckBox.Text = "Ch 1";
            this.Ch1CheckBox.UseVisualStyleBackColor = true;
            this.Ch1CheckBox.CheckedChanged += new System.EventHandler(this.FyCheckBox_CheckedChanged);
            // 
            // formsPlot1
            // 
            this.formsPlot1.Location = new System.Drawing.Point(15, 32);
            this.formsPlot1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.formsPlot1.Name = "formsPlot1";
            this.formsPlot1.Size = new System.Drawing.Size(590, 231);
            this.formsPlot1.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Mistral", 20.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(10, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 34);
            this.label6.TabIndex = 1;
            this.label6.Text = "Live Graph";
            // 
            // COPFormsPlot
            // 
            this.COPFormsPlot.BackColor = System.Drawing.Color.Transparent;
            this.COPFormsPlot.Location = new System.Drawing.Point(654, 260);
            this.COPFormsPlot.Name = "COPFormsPlot";
            this.COPFormsPlot.Size = new System.Drawing.Size(180, 180);
            this.COPFormsPlot.TabIndex = 28;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.RecordTime_TextBox);
            this.groupBox4.Controls.Add(this.Name_TextBox);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.Stop_Button);
            this.groupBox4.Controls.Add(this.Record_Button);
            this.groupBox4.Location = new System.Drawing.Point(236, 277);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(250, 162);
            this.groupBox4.TabIndex = 34;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Coleta";
            // 
            // RecordTime_TextBox
            // 
            this.RecordTime_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RecordTime_TextBox.Location = new System.Drawing.Point(107, 78);
            this.RecordTime_TextBox.Name = "RecordTime_TextBox";
            this.RecordTime_TextBox.Size = new System.Drawing.Size(126, 23);
            this.RecordTime_TextBox.TabIndex = 35;
            this.RecordTime_TextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RecordTime_TextBox_KeyPress);
            // 
            // Name_TextBox
            // 
            this.Name_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name_TextBox.Location = new System.Drawing.Point(107, 39);
            this.Name_TextBox.Name = "Name_TextBox";
            this.Name_TextBox.Size = new System.Drawing.Size(126, 23);
            this.Name_TextBox.TabIndex = 35;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 20);
            this.label5.TabIndex = 34;
            this.label5.Text = "Tempo:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 20);
            this.label3.TabIndex = 34;
            this.label3.Text = "Nome:";
            // 
            // PesoText
            // 
            this.PesoText.AutoSize = true;
            this.PesoText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PesoText.Location = new System.Drawing.Point(35, 36);
            this.PesoText.Name = "PesoText";
            this.PesoText.Size = new System.Drawing.Size(97, 31);
            this.PesoText.TabIndex = 38;
            this.PesoText.Text = "00.000";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.NewtonText);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.PesoText);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(492, 277);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(156, 162);
            this.groupBox2.TabIndex = 34;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Peso";
            // 
            // NewtonText
            // 
            this.NewtonText.AutoSize = true;
            this.NewtonText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewtonText.Location = new System.Drawing.Point(34, 80);
            this.NewtonText.Name = "NewtonText";
            this.NewtonText.Size = new System.Drawing.Size(97, 31);
            this.NewtonText.TabIndex = 38;
            this.NewtonText.Text = "00.000";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 85);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 20);
            this.label8.TabIndex = 34;
            this.label8.Text = "N:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 20);
            this.label7.TabIndex = 34;
            this.label7.Text = "Kg:";
            // 
            // BioMensurae
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(846, 452);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.COPFormsPlot);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BioMensurae";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BioMensurae Data Acquisition";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button OpenFile_Button;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.ToolTip toolTip3;
        private System.Windows.Forms.ToolTip toolTip4;
        private System.Windows.Forms.ToolTip toolTip5;
        private System.Windows.Forms.ToolTip toolTip6;
        private System.Windows.Forms.Label clock_display;
        private System.Windows.Forms.Button Calib_Button;
        private System.Windows.Forms.Button Plot_Button;
        private System.Windows.Forms.Button Stop_Button;
        private System.Windows.Forms.Button Record_Button;
        private System.Windows.Forms.ComboBox TipoBox;
        private System.Windows.Forms.ComboBox SerialPortsBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Connect_Button;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox EventLogTextBox;
        private ScottPlot.FormsPlot formsPlot1;
        private System.Windows.Forms.CheckBox Ch1CheckBox;
        private System.Windows.Forms.CheckBox Ch2CheckBox;
        private System.Windows.Forms.CheckBox Ch3CheckBox;
        private ScottPlot.FormsPlot COPFormsPlot;
        private System.Windows.Forms.Label connect_label;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label StatusText;
        private System.Windows.Forms.Label PesoText;
        private System.Windows.Forms.TextBox RecordTime_TextBox;
        private System.Windows.Forms.TextBox Name_TextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label NewtonText;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
    }
}

