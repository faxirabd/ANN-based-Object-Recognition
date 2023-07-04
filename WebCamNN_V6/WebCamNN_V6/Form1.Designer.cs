namespace WebCamNN_V6
{
    partial class Form1
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_snapshoot = new System.Windows.Forms.Button();
            this.btn_train = new System.Windows.Forms.Button();
            this.btn_recognize = new System.Windows.Forms.Button();
            this.btn_startvideo = new System.Windows.Forms.Button();
            this.lbl_iteration = new System.Windows.Forms.Label();
            this.txb_numbIterations = new System.Windows.Forms.TextBox();
            this.btn_addpattern = new System.Windows.Forms.Button();
            this.lbl_patterncount = new System.Windows.Forms.Label();
            this.lbl_trainerror0 = new System.Windows.Forms.Label();
            this.lbl_trainerror1 = new System.Windows.Forms.Label();
            this.lbl_trainerror2 = new System.Windows.Forms.Label();
            this.lbl_trainerror3 = new System.Windows.Forms.Label();
            this.lbl_trainerror7 = new System.Windows.Forms.Label();
            this.lbl_trainerror6 = new System.Windows.Forms.Label();
            this.lbl_trainerror5 = new System.Windows.Forms.Label();
            this.lbl_trainerror4 = new System.Windows.Forms.Label();
            this.lbl_recognrror7 = new System.Windows.Forms.Label();
            this.lbl_recognrror6 = new System.Windows.Forms.Label();
            this.lbl_recognrror5 = new System.Windows.Forms.Label();
            this.lbl_recognrror4 = new System.Windows.Forms.Label();
            this.lbl_recognrror3 = new System.Windows.Forms.Label();
            this.lbl_recognrror2 = new System.Windows.Forms.Label();
            this.lbl_recognrror1 = new System.Windows.Forms.Label();
            this.lbl_recognrror0 = new System.Windows.Forms.Label();
            this.cmbx_patternnumber = new System.Windows.Forms.ComboBox();
            this.btn_showpattern = new System.Windows.Forms.Button();
            this.cmbx_showpattern = new System.Windows.Forms.ComboBox();
            this.trcbr_brightness = new System.Windows.Forms.TrackBar();
            this.lbl_brightness = new System.Windows.Forms.Label();
            this.txb_patternname = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.saveNN_btn = new System.Windows.Forms.Button();
            this.btn_loadNN = new System.Windows.Forms.Button();
            this.btn_savepattern = new System.Windows.Forms.Button();
            this.btn_loadpattern = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trcbr_brightness)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(-1, -4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(504, 526);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // btn_snapshoot
            // 
            this.btn_snapshoot.Location = new System.Drawing.Point(576, 18);
            this.btn_snapshoot.Name = "btn_snapshoot";
            this.btn_snapshoot.Size = new System.Drawing.Size(102, 29);
            this.btn_snapshoot.TabIndex = 3;
            this.btn_snapshoot.Text = "Snap Shoot";
            this.btn_snapshoot.UseVisualStyleBackColor = true;
            this.btn_snapshoot.Click += new System.EventHandler(this.btn_snapshoot_Click);
            // 
            // btn_train
            // 
            this.btn_train.Location = new System.Drawing.Point(576, 157);
            this.btn_train.Name = "btn_train";
            this.btn_train.Size = new System.Drawing.Size(102, 29);
            this.btn_train.TabIndex = 4;
            this.btn_train.Text = "Train";
            this.btn_train.UseVisualStyleBackColor = true;
            this.btn_train.Click += new System.EventHandler(this.btn_train_Click);
            // 
            // btn_recognize
            // 
            this.btn_recognize.Location = new System.Drawing.Point(576, 397);
            this.btn_recognize.Name = "btn_recognize";
            this.btn_recognize.Size = new System.Drawing.Size(102, 29);
            this.btn_recognize.TabIndex = 6;
            this.btn_recognize.Text = "Recognize";
            this.btn_recognize.UseVisualStyleBackColor = true;
            this.btn_recognize.Click += new System.EventHandler(this.btn_recognize_Click);
            // 
            // btn_startvideo
            // 
            this.btn_startvideo.Location = new System.Drawing.Point(820, 18);
            this.btn_startvideo.Name = "btn_startvideo";
            this.btn_startvideo.Size = new System.Drawing.Size(102, 29);
            this.btn_startvideo.TabIndex = 7;
            this.btn_startvideo.Text = "Start Video";
            this.btn_startvideo.UseVisualStyleBackColor = true;
            this.btn_startvideo.Click += new System.EventHandler(this.btn_startvideo_Click);
            // 
            // lbl_iteration
            // 
            this.lbl_iteration.AutoSize = true;
            this.lbl_iteration.Location = new System.Drawing.Point(820, 261);
            this.lbl_iteration.Name = "lbl_iteration";
            this.lbl_iteration.Size = new System.Drawing.Size(63, 13);
            this.lbl_iteration.TabIndex = 8;
            this.lbl_iteration.Text = "Iteration: 00";
            // 
            // txb_numbIterations
            // 
            this.txb_numbIterations.Location = new System.Drawing.Point(820, 162);
            this.txb_numbIterations.Name = "txb_numbIterations";
            this.txb_numbIterations.Size = new System.Drawing.Size(100, 20);
            this.txb_numbIterations.TabIndex = 9;
            // 
            // btn_addpattern
            // 
            this.btn_addpattern.Location = new System.Drawing.Point(576, 64);
            this.btn_addpattern.Name = "btn_addpattern";
            this.btn_addpattern.Size = new System.Drawing.Size(102, 29);
            this.btn_addpattern.TabIndex = 10;
            this.btn_addpattern.Text = "Add Pattern";
            this.btn_addpattern.UseVisualStyleBackColor = true;
            this.btn_addpattern.Click += new System.EventHandler(this.btn_addpattern_Click);
            // 
            // lbl_patterncount
            // 
            this.lbl_patterncount.AutoSize = true;
            this.lbl_patterncount.Location = new System.Drawing.Point(820, 108);
            this.lbl_patterncount.Name = "lbl_patterncount";
            this.lbl_patterncount.Size = new System.Drawing.Size(93, 13);
            this.lbl_patterncount.TabIndex = 11;
            this.lbl_patterncount.Text = "Pattern Number:-1";
            // 
            // lbl_trainerror0
            // 
            this.lbl_trainerror0.AutoSize = true;
            this.lbl_trainerror0.Location = new System.Drawing.Point(576, 198);
            this.lbl_trainerror0.Name = "lbl_trainerror0";
            this.lbl_trainerror0.Size = new System.Drawing.Size(53, 13);
            this.lbl_trainerror0.TabIndex = 12;
            this.lbl_trainerror0.Text = "Error0: 00";
            // 
            // lbl_trainerror1
            // 
            this.lbl_trainerror1.AutoSize = true;
            this.lbl_trainerror1.Location = new System.Drawing.Point(576, 219);
            this.lbl_trainerror1.Name = "lbl_trainerror1";
            this.lbl_trainerror1.Size = new System.Drawing.Size(53, 13);
            this.lbl_trainerror1.TabIndex = 13;
            this.lbl_trainerror1.Text = "Error1: 00";
            // 
            // lbl_trainerror2
            // 
            this.lbl_trainerror2.AutoSize = true;
            this.lbl_trainerror2.Location = new System.Drawing.Point(576, 240);
            this.lbl_trainerror2.Name = "lbl_trainerror2";
            this.lbl_trainerror2.Size = new System.Drawing.Size(53, 13);
            this.lbl_trainerror2.TabIndex = 14;
            this.lbl_trainerror2.Text = "Error2: 00";
            // 
            // lbl_trainerror3
            // 
            this.lbl_trainerror3.AutoSize = true;
            this.lbl_trainerror3.Location = new System.Drawing.Point(576, 261);
            this.lbl_trainerror3.Name = "lbl_trainerror3";
            this.lbl_trainerror3.Size = new System.Drawing.Size(53, 13);
            this.lbl_trainerror3.TabIndex = 15;
            this.lbl_trainerror3.Text = "Error3: 00";
            // 
            // lbl_trainerror7
            // 
            this.lbl_trainerror7.AutoSize = true;
            this.lbl_trainerror7.Location = new System.Drawing.Point(576, 345);
            this.lbl_trainerror7.Name = "lbl_trainerror7";
            this.lbl_trainerror7.Size = new System.Drawing.Size(53, 13);
            this.lbl_trainerror7.TabIndex = 19;
            this.lbl_trainerror7.Text = "Error7: 00";
            // 
            // lbl_trainerror6
            // 
            this.lbl_trainerror6.AutoSize = true;
            this.lbl_trainerror6.Location = new System.Drawing.Point(576, 324);
            this.lbl_trainerror6.Name = "lbl_trainerror6";
            this.lbl_trainerror6.Size = new System.Drawing.Size(53, 13);
            this.lbl_trainerror6.TabIndex = 18;
            this.lbl_trainerror6.Text = "Error6: 00";
            // 
            // lbl_trainerror5
            // 
            this.lbl_trainerror5.AutoSize = true;
            this.lbl_trainerror5.Location = new System.Drawing.Point(576, 303);
            this.lbl_trainerror5.Name = "lbl_trainerror5";
            this.lbl_trainerror5.Size = new System.Drawing.Size(53, 13);
            this.lbl_trainerror5.TabIndex = 17;
            this.lbl_trainerror5.Text = "Error5: 00";
            // 
            // lbl_trainerror4
            // 
            this.lbl_trainerror4.AutoSize = true;
            this.lbl_trainerror4.Location = new System.Drawing.Point(576, 282);
            this.lbl_trainerror4.Name = "lbl_trainerror4";
            this.lbl_trainerror4.Size = new System.Drawing.Size(53, 13);
            this.lbl_trainerror4.TabIndex = 16;
            this.lbl_trainerror4.Text = "Error4: 00";
            // 
            // lbl_recognrror7
            // 
            this.lbl_recognrror7.AutoSize = true;
            this.lbl_recognrror7.Location = new System.Drawing.Point(579, 708);
            this.lbl_recognrror7.Name = "lbl_recognrror7";
            this.lbl_recognrror7.Size = new System.Drawing.Size(69, 13);
            this.lbl_recognrror7.TabIndex = 27;
            this.lbl_recognrror7.Text = "Recogn7: 00";
            // 
            // lbl_recognrror6
            // 
            this.lbl_recognrror6.AutoSize = true;
            this.lbl_recognrror6.Location = new System.Drawing.Point(579, 671);
            this.lbl_recognrror6.Name = "lbl_recognrror6";
            this.lbl_recognrror6.Size = new System.Drawing.Size(69, 13);
            this.lbl_recognrror6.TabIndex = 26;
            this.lbl_recognrror6.Text = "Recogn6: 00";
            // 
            // lbl_recognrror5
            // 
            this.lbl_recognrror5.AutoSize = true;
            this.lbl_recognrror5.Location = new System.Drawing.Point(579, 634);
            this.lbl_recognrror5.Name = "lbl_recognrror5";
            this.lbl_recognrror5.Size = new System.Drawing.Size(69, 13);
            this.lbl_recognrror5.TabIndex = 25;
            this.lbl_recognrror5.Text = "Recogn5: 00";
            // 
            // lbl_recognrror4
            // 
            this.lbl_recognrror4.AutoSize = true;
            this.lbl_recognrror4.Location = new System.Drawing.Point(579, 597);
            this.lbl_recognrror4.Name = "lbl_recognrror4";
            this.lbl_recognrror4.Size = new System.Drawing.Size(69, 13);
            this.lbl_recognrror4.TabIndex = 24;
            this.lbl_recognrror4.Text = "Recogn4: 00";
            // 
            // lbl_recognrror3
            // 
            this.lbl_recognrror3.AutoSize = true;
            this.lbl_recognrror3.Location = new System.Drawing.Point(579, 560);
            this.lbl_recognrror3.Name = "lbl_recognrror3";
            this.lbl_recognrror3.Size = new System.Drawing.Size(69, 13);
            this.lbl_recognrror3.TabIndex = 23;
            this.lbl_recognrror3.Text = "Recogn3: 00";
            // 
            // lbl_recognrror2
            // 
            this.lbl_recognrror2.AutoSize = true;
            this.lbl_recognrror2.Location = new System.Drawing.Point(579, 523);
            this.lbl_recognrror2.Name = "lbl_recognrror2";
            this.lbl_recognrror2.Size = new System.Drawing.Size(69, 13);
            this.lbl_recognrror2.TabIndex = 22;
            this.lbl_recognrror2.Text = "Recogn2: 00";
            // 
            // lbl_recognrror1
            // 
            this.lbl_recognrror1.AutoSize = true;
            this.lbl_recognrror1.Location = new System.Drawing.Point(579, 486);
            this.lbl_recognrror1.Name = "lbl_recognrror1";
            this.lbl_recognrror1.Size = new System.Drawing.Size(69, 13);
            this.lbl_recognrror1.TabIndex = 21;
            this.lbl_recognrror1.Text = "Recogn1: 00";
            // 
            // lbl_recognrror0
            // 
            this.lbl_recognrror0.AutoSize = true;
            this.lbl_recognrror0.Location = new System.Drawing.Point(579, 449);
            this.lbl_recognrror0.Name = "lbl_recognrror0";
            this.lbl_recognrror0.Size = new System.Drawing.Size(69, 13);
            this.lbl_recognrror0.TabIndex = 20;
            this.lbl_recognrror0.Text = "Recogn0: 00";
            // 
            // cmbx_patternnumber
            // 
            this.cmbx_patternnumber.FormattingEnabled = true;
            this.cmbx_patternnumber.Location = new System.Drawing.Point(820, 69);
            this.cmbx_patternnumber.Name = "cmbx_patternnumber";
            this.cmbx_patternnumber.Size = new System.Drawing.Size(102, 21);
            this.cmbx_patternnumber.TabIndex = 28;
            this.cmbx_patternnumber.SelectedIndexChanged += new System.EventHandler(this.cmbx_patternnumber_SelectedIndexChanged);
            // 
            // btn_showpattern
            // 
            this.btn_showpattern.Location = new System.Drawing.Point(12, 624);
            this.btn_showpattern.Name = "btn_showpattern";
            this.btn_showpattern.Size = new System.Drawing.Size(102, 29);
            this.btn_showpattern.TabIndex = 29;
            this.btn_showpattern.Text = "Show Pattern";
            this.btn_showpattern.UseVisualStyleBackColor = true;
            this.btn_showpattern.Click += new System.EventHandler(this.btn_showpattern_Click);
            // 
            // cmbx_showpattern
            // 
            this.cmbx_showpattern.Enabled = false;
            this.cmbx_showpattern.FormattingEnabled = true;
            this.cmbx_showpattern.Location = new System.Drawing.Point(171, 629);
            this.cmbx_showpattern.Name = "cmbx_showpattern";
            this.cmbx_showpattern.Size = new System.Drawing.Size(102, 21);
            this.cmbx_showpattern.TabIndex = 30;
            this.cmbx_showpattern.SelectedIndexChanged += new System.EventHandler(this.cmbx_showpattern_SelectedIndexChanged);
            // 
            // trcbr_brightness
            // 
            this.trcbr_brightness.Location = new System.Drawing.Point(12, 576);
            this.trcbr_brightness.Maximum = 255;
            this.trcbr_brightness.Name = "trcbr_brightness";
            this.trcbr_brightness.Size = new System.Drawing.Size(491, 45);
            this.trcbr_brightness.TabIndex = 31;
            this.trcbr_brightness.TickFrequency = 10;
            this.trcbr_brightness.Value = 65;
            this.trcbr_brightness.Scroll += new System.EventHandler(this.trcbr_brightness_Scroll);
            // 
            // lbl_brightness
            // 
            this.lbl_brightness.AutoSize = true;
            this.lbl_brightness.Location = new System.Drawing.Point(220, 551);
            this.lbl_brightness.Name = "lbl_brightness";
            this.lbl_brightness.Size = new System.Drawing.Size(68, 13);
            this.lbl_brightness.TabIndex = 32;
            this.lbl_brightness.Text = "Brightness: 0";
            // 
            // txb_patternname
            // 
            this.txb_patternname.Location = new System.Drawing.Point(699, 69);
            this.txb_patternname.Name = "txb_patternname";
            this.txb_patternname.Size = new System.Drawing.Size(100, 20);
            this.txb_patternname.TabIndex = 34;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(711, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Pattern Name";
            // 
            // saveNN_btn
            // 
            this.saveNN_btn.Location = new System.Drawing.Point(818, 397);
            this.saveNN_btn.Name = "saveNN_btn";
            this.saveNN_btn.Size = new System.Drawing.Size(102, 29);
            this.saveNN_btn.TabIndex = 36;
            this.saveNN_btn.Text = "Save Network";
            this.saveNN_btn.UseVisualStyleBackColor = true;
            this.saveNN_btn.Click += new System.EventHandler(this.saveNN_btn_Click);
            // 
            // btn_loadNN
            // 
            this.btn_loadNN.Location = new System.Drawing.Point(818, 449);
            this.btn_loadNN.Name = "btn_loadNN";
            this.btn_loadNN.Size = new System.Drawing.Size(102, 29);
            this.btn_loadNN.TabIndex = 37;
            this.btn_loadNN.Text = "Load Network";
            this.btn_loadNN.UseVisualStyleBackColor = true;
            this.btn_loadNN.Click += new System.EventHandler(this.btn_loadNN_Click);
            // 
            // btn_savepattern
            // 
            this.btn_savepattern.Location = new System.Drawing.Point(576, 108);
            this.btn_savepattern.Name = "btn_savepattern";
            this.btn_savepattern.Size = new System.Drawing.Size(102, 29);
            this.btn_savepattern.TabIndex = 38;
            this.btn_savepattern.Text = "Save Pattern";
            this.btn_savepattern.UseVisualStyleBackColor = true;
            this.btn_savepattern.Click += new System.EventHandler(this.btn_savepattern_Click);
            // 
            // btn_loadpattern
            // 
            this.btn_loadpattern.Location = new System.Drawing.Point(697, 108);
            this.btn_loadpattern.Name = "btn_loadpattern";
            this.btn_loadpattern.Size = new System.Drawing.Size(102, 29);
            this.btn_loadpattern.TabIndex = 39;
            this.btn_loadpattern.Text = "Load Pattern";
            this.btn_loadpattern.UseVisualStyleBackColor = true;
            this.btn_loadpattern.Click += new System.EventHandler(this.btn_loadpattern_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 746);
            this.Controls.Add(this.btn_loadpattern);
            this.Controls.Add(this.btn_savepattern);
            this.Controls.Add(this.btn_loadNN);
            this.Controls.Add(this.saveNN_btn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txb_patternname);
            this.Controls.Add(this.lbl_brightness);
            this.Controls.Add(this.trcbr_brightness);
            this.Controls.Add(this.cmbx_showpattern);
            this.Controls.Add(this.btn_showpattern);
            this.Controls.Add(this.cmbx_patternnumber);
            this.Controls.Add(this.lbl_recognrror7);
            this.Controls.Add(this.lbl_recognrror6);
            this.Controls.Add(this.lbl_recognrror5);
            this.Controls.Add(this.lbl_recognrror4);
            this.Controls.Add(this.lbl_recognrror3);
            this.Controls.Add(this.lbl_recognrror2);
            this.Controls.Add(this.lbl_recognrror1);
            this.Controls.Add(this.lbl_recognrror0);
            this.Controls.Add(this.lbl_trainerror7);
            this.Controls.Add(this.lbl_trainerror6);
            this.Controls.Add(this.lbl_trainerror5);
            this.Controls.Add(this.lbl_trainerror4);
            this.Controls.Add(this.lbl_trainerror3);
            this.Controls.Add(this.lbl_trainerror2);
            this.Controls.Add(this.lbl_trainerror1);
            this.Controls.Add(this.lbl_trainerror0);
            this.Controls.Add(this.lbl_patterncount);
            this.Controls.Add(this.btn_addpattern);
            this.Controls.Add(this.txb_numbIterations);
            this.Controls.Add(this.lbl_iteration);
            this.Controls.Add(this.btn_startvideo);
            this.Controls.Add(this.btn_recognize);
            this.Controls.Add(this.btn_train);
            this.Controls.Add(this.btn_snapshoot);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trcbr_brightness)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btn_snapshoot;
        private System.Windows.Forms.Button btn_train;
        private System.Windows.Forms.Button btn_recognize;
        private System.Windows.Forms.Button btn_startvideo;
        private System.Windows.Forms.Label lbl_iteration;
        private System.Windows.Forms.TextBox txb_numbIterations;
        private System.Windows.Forms.Button btn_addpattern;
        private System.Windows.Forms.Label lbl_patterncount;
        private System.Windows.Forms.Label lbl_trainerror0;
        private System.Windows.Forms.Label lbl_trainerror1;
        private System.Windows.Forms.Label lbl_trainerror2;
        private System.Windows.Forms.Label lbl_trainerror3;
        private System.Windows.Forms.Label lbl_trainerror7;
        private System.Windows.Forms.Label lbl_trainerror6;
        private System.Windows.Forms.Label lbl_trainerror5;
        private System.Windows.Forms.Label lbl_trainerror4;
        private System.Windows.Forms.Label lbl_recognrror7;
        private System.Windows.Forms.Label lbl_recognrror6;
        private System.Windows.Forms.Label lbl_recognrror5;
        private System.Windows.Forms.Label lbl_recognrror4;
        private System.Windows.Forms.Label lbl_recognrror3;
        private System.Windows.Forms.Label lbl_recognrror2;
        private System.Windows.Forms.Label lbl_recognrror1;
        private System.Windows.Forms.Label lbl_recognrror0;
        private System.Windows.Forms.ComboBox cmbx_patternnumber;
        private System.Windows.Forms.Button btn_showpattern;
        private System.Windows.Forms.ComboBox cmbx_showpattern;
        private System.Windows.Forms.TrackBar trcbr_brightness;
        private System.Windows.Forms.Label lbl_brightness;
        private System.Windows.Forms.TextBox txb_patternname;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button saveNN_btn;
        private System.Windows.Forms.Button btn_loadNN;
        private System.Windows.Forms.Button btn_savepattern;
        private System.Windows.Forms.Button btn_loadpattern;
    }
}

