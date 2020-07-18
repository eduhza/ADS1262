namespace BioMensurae
{
    partial class Form3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.TipoBox = new System.Windows.Forms.ComboBox();
            this.SerialPortsBox = new System.Windows.Forms.ComboBox();
            this.Config_Button = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.MzCheckBox = new System.Windows.Forms.CheckBox();
            this.MxCheckBox = new System.Windows.Forms.CheckBox();
            this.StopCalib_Button = new System.Windows.Forms.Button();
            this.FyCheckBox = new System.Windows.Forms.CheckBox();
            this.Save_Button = new System.Windows.Forms.Button();
            this.ZBox = new System.Windows.Forms.TextBox();
            this.GetValueButton = new System.Windows.Forms.Button();
            this.XBox = new System.Windows.Forms.TextBox();
            this.PesoBox = new System.Windows.Forms.TextBox();
            this.FyLeituraBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.CalibLogTextBox = new System.Windows.Forms.RichTextBox();
            this.formsPlot1 = new ScottPlot.FormsPlot();
            this.COPFormsPlot = new ScottPlot.FormsPlot();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.MxLeituraBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.MzLeituraBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // TipoBox
            // 
            this.TipoBox.FormattingEnabled = true;
            this.TipoBox.Items.AddRange(new object[] {
            "1D - 500x500",
            "3D - 500x500"});
            this.TipoBox.Location = new System.Drawing.Point(73, 23);
            this.TipoBox.Name = "TipoBox";
            this.TipoBox.Size = new System.Drawing.Size(121, 24);
            this.TipoBox.TabIndex = 0;
            this.TipoBox.SelectedIndexChanged += new System.EventHandler(this.TipoBox_SelectedIndexChanged);
            // 
            // SerialPortsBox
            // 
            this.SerialPortsBox.FormattingEnabled = true;
            this.SerialPortsBox.Location = new System.Drawing.Point(73, 51);
            this.SerialPortsBox.Name = "SerialPortsBox";
            this.SerialPortsBox.Size = new System.Drawing.Size(121, 24);
            this.SerialPortsBox.TabIndex = 1;
            this.SerialPortsBox.SelectedIndexChanged += new System.EventHandler(this.SerialPortsBox_SelectedIndexChanged);
            // 
            // Config_Button
            // 
            this.Config_Button.BackColor = System.Drawing.Color.ForestGreen;
            this.Config_Button.Enabled = false;
            this.Config_Button.FlatAppearance.BorderSize = 0;
            this.Config_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Config_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Config_Button.Location = new System.Drawing.Point(6, 84);
            this.Config_Button.Margin = new System.Windows.Forms.Padding(0);
            this.Config_Button.Name = "Config_Button";
            this.Config_Button.Size = new System.Drawing.Size(189, 28);
            this.Config_Button.TabIndex = 2;
            this.Config_Button.Text = "Conectar";
            this.Config_Button.UseVisualStyleBackColor = false;
            this.Config_Button.Click += new System.EventHandler(this.Config_Button_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.Config_Button);
            this.groupBox1.Controls.Add(this.TipoBox);
            this.groupBox1.Controls.Add(this.SerialPortsBox);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(7, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 124);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configurações";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Porta:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Tipo:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.MzCheckBox);
            this.groupBox2.Controls.Add(this.MxCheckBox);
            this.groupBox2.Controls.Add(this.StopCalib_Button);
            this.groupBox2.Controls.Add(this.FyCheckBox);
            this.groupBox2.Controls.Add(this.Save_Button);
            this.groupBox2.Controls.Add(this.ZBox);
            this.groupBox2.Controls.Add(this.GetValueButton);
            this.groupBox2.Controls.Add(this.XBox);
            this.groupBox2.Controls.Add(this.PesoBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(218, 255);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(321, 168);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Calibração";
            // 
            // MzCheckBox
            // 
            this.MzCheckBox.AutoSize = true;
            this.MzCheckBox.Enabled = false;
            this.MzCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MzCheckBox.Location = new System.Drawing.Point(150, 45);
            this.MzCheckBox.Name = "MzCheckBox";
            this.MzCheckBox.Size = new System.Drawing.Size(45, 21);
            this.MzCheckBox.TabIndex = 9;
            this.MzCheckBox.Text = "Mz";
            this.MzCheckBox.UseVisualStyleBackColor = true;
            this.MzCheckBox.CheckedChanged += new System.EventHandler(this.MzCheckBox_CheckedChanged);
            // 
            // MxCheckBox
            // 
            this.MxCheckBox.AutoSize = true;
            this.MxCheckBox.Enabled = false;
            this.MxCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MxCheckBox.Location = new System.Drawing.Point(78, 45);
            this.MxCheckBox.Name = "MxCheckBox";
            this.MxCheckBox.Size = new System.Drawing.Size(44, 21);
            this.MxCheckBox.TabIndex = 9;
            this.MxCheckBox.Text = "Mx";
            this.MxCheckBox.UseVisualStyleBackColor = true;
            this.MxCheckBox.CheckedChanged += new System.EventHandler(this.MxCheckBox_CheckedChanged);
            // 
            // StopCalib_Button
            // 
            this.StopCalib_Button.Enabled = false;
            this.StopCalib_Button.Location = new System.Drawing.Point(201, 95);
            this.StopCalib_Button.Name = "StopCalib_Button";
            this.StopCalib_Button.Size = new System.Drawing.Size(109, 23);
            this.StopCalib_Button.TabIndex = 9;
            this.StopCalib_Button.Text = "Cancelar";
            this.StopCalib_Button.UseVisualStyleBackColor = true;
            this.StopCalib_Button.Click += new System.EventHandler(this.StopCalib_Button_Click);
            // 
            // FyCheckBox
            // 
            this.FyCheckBox.AutoSize = true;
            this.FyCheckBox.Checked = true;
            this.FyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FyCheckBox.Enabled = false;
            this.FyCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FyCheckBox.Location = new System.Drawing.Point(6, 45);
            this.FyCheckBox.Name = "FyCheckBox";
            this.FyCheckBox.Size = new System.Drawing.Size(42, 21);
            this.FyCheckBox.TabIndex = 9;
            this.FyCheckBox.Text = "Fy";
            this.FyCheckBox.UseVisualStyleBackColor = true;
            this.FyCheckBox.CheckStateChanged += new System.EventHandler(this.FyCheckBox_CheckStateChanged);
            // 
            // Save_Button
            // 
            this.Save_Button.Enabled = false;
            this.Save_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Save_Button.Location = new System.Drawing.Point(201, 132);
            this.Save_Button.Name = "Save_Button";
            this.Save_Button.Size = new System.Drawing.Size(109, 23);
            this.Save_Button.TabIndex = 7;
            this.Save_Button.Text = "Salvar";
            this.Save_Button.UseVisualStyleBackColor = true;
            this.Save_Button.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ZBox
            // 
            this.ZBox.Enabled = false;
            this.ZBox.Location = new System.Drawing.Point(125, 95);
            this.ZBox.Name = "ZBox";
            this.ZBox.Size = new System.Drawing.Size(67, 23);
            this.ZBox.TabIndex = 6;
            this.ZBox.Click += new System.EventHandler(this.ZBox_Click);
            this.ZBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ZBox_KeyPress);
            // 
            // GetValueButton
            // 
            this.GetValueButton.Enabled = false;
            this.GetValueButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GetValueButton.Location = new System.Drawing.Point(201, 22);
            this.GetValueButton.Name = "GetValueButton";
            this.GetValueButton.Size = new System.Drawing.Size(109, 61);
            this.GetValueButton.TabIndex = 8;
            this.GetValueButton.Text = "Adicionar Amostra";
            this.GetValueButton.UseVisualStyleBackColor = true;
            this.GetValueButton.Click += new System.EventHandler(this.GetValueButton_Click);
            // 
            // XBox
            // 
            this.XBox.Enabled = false;
            this.XBox.Location = new System.Drawing.Point(27, 95);
            this.XBox.Name = "XBox";
            this.XBox.Size = new System.Drawing.Size(67, 23);
            this.XBox.TabIndex = 5;
            this.XBox.Click += new System.EventHandler(this.XBox_Click);
            this.XBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.XBox_KeyPress);
            // 
            // PesoBox
            // 
            this.PesoBox.Enabled = false;
            this.PesoBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PesoBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.PesoBox.Location = new System.Drawing.Point(89, 134);
            this.PesoBox.Name = "PesoBox";
            this.PesoBox.Size = new System.Drawing.Size(105, 23);
            this.PesoBox.TabIndex = 4;
            this.PesoBox.Click += new System.EventHandler(this.PesoBox_Click);
            this.PesoBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PesoBox_KeyPress);
            // 
            // FyLeituraBox
            // 
            this.FyLeituraBox.Enabled = false;
            this.FyLeituraBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FyLeituraBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FyLeituraBox.Location = new System.Drawing.Point(72, 22);
            this.FyLeituraBox.Name = "FyLeituraBox";
            this.FyLeituraBox.ReadOnly = true;
            this.FyLeituraBox.Size = new System.Drawing.Size(122, 23);
            this.FyLeituraBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(16, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "Peso (Kg)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(107, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(16, 16);
            this.label7.TabIndex = 5;
            this.label7.Text = "Z";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(10, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 16);
            this.label6.TabIndex = 5;
            this.label6.Text = "X";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(9, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "Coordenadas:";
            // 
            // CalibLogTextBox
            // 
            this.CalibLogTextBox.Location = new System.Drawing.Point(7, 249);
            this.CalibLogTextBox.Name = "CalibLogTextBox";
            this.CalibLogTextBox.ReadOnly = true;
            this.CalibLogTextBox.Size = new System.Drawing.Size(200, 174);
            this.CalibLogTextBox.TabIndex = 6;
            this.CalibLogTextBox.TabStop = false;
            this.CalibLogTextBox.Text = "";
            this.CalibLogTextBox.TextChanged += new System.EventHandler(this.CalibLogTextBox_TextChanged);
            // 
            // formsPlot1
            // 
            this.formsPlot1.Location = new System.Drawing.Point(218, 6);
            this.formsPlot1.Name = "formsPlot1";
            this.formsPlot1.Size = new System.Drawing.Size(538, 231);
            this.formsPlot1.TabIndex = 10;
            // 
            // COPFormsPlot
            // 
            this.COPFormsPlot.Location = new System.Drawing.Point(556, 230);
            this.COPFormsPlot.Name = "COPFormsPlot";
            this.COPFormsPlot.Size = new System.Drawing.Size(200, 200);
            this.COPFormsPlot.TabIndex = 11;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.MzLeituraBox);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.MxLeituraBox);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.FyLeituraBox);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.groupBox3.Location = new System.Drawing.Point(7, 134);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 109);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Leituras";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Fy: ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(15, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(28, 16);
            this.label8.TabIndex = 5;
            this.label8.Text = "Mx:";
            // 
            // MxLeituraBox
            // 
            this.MxLeituraBox.Enabled = false;
            this.MxLeituraBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MxLeituraBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MxLeituraBox.Location = new System.Drawing.Point(72, 49);
            this.MxLeituraBox.Name = "MxLeituraBox";
            this.MxLeituraBox.ReadOnly = true;
            this.MxLeituraBox.Size = new System.Drawing.Size(122, 23);
            this.MxLeituraBox.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(15, 80);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 16);
            this.label9.TabIndex = 5;
            this.label9.Text = "Mz:";
            // 
            // MzLeituraBox
            // 
            this.MzLeituraBox.Enabled = false;
            this.MzLeituraBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MzLeituraBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MzLeituraBox.Location = new System.Drawing.Point(72, 77);
            this.MzLeituraBox.Name = "MzLeituraBox";
            this.MzLeituraBox.ReadOnly = true;
            this.MzLeituraBox.Size = new System.Drawing.Size(122, 23);
            this.MzLeituraBox.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(10, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 16);
            this.label10.TabIndex = 5;
            this.label10.Text = "Canais:";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 433);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.COPFormsPlot);
            this.Controls.Add(this.formsPlot1);
            this.Controls.Add(this.CalibLogTextBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form3";
            this.Text = "Calibrate Window";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form3_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox TipoBox;
        private System.Windows.Forms.ComboBox SerialPortsBox;
        private System.Windows.Forms.Button Config_Button;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox PesoBox;
        private System.Windows.Forms.TextBox FyLeituraBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ZBox;
        private System.Windows.Forms.TextBox XBox;
        private System.Windows.Forms.Button GetValueButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox CalibLogTextBox;
        private System.Windows.Forms.Button Save_Button;
        private System.Windows.Forms.CheckBox FyCheckBox;
        private System.Windows.Forms.CheckBox MzCheckBox;
        private System.Windows.Forms.CheckBox MxCheckBox;
        private System.Windows.Forms.Button StopCalib_Button;
        private ScottPlot.FormsPlot formsPlot1;
        private ScottPlot.FormsPlot COPFormsPlot;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox MzLeituraBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox MxLeituraBox;
        private System.Windows.Forms.Label label8;
    }
}