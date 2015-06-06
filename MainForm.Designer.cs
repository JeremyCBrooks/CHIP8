namespace CHIP8
{
    partial class MainForm
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
            this.picRenderOutput = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkShowDebug = new System.Windows.Forms.CheckBox();
            this.grpKeys = new System.Windows.Forms.GroupBox();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picRenderOutput)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picRenderOutput
            // 
            this.picRenderOutput.Location = new System.Drawing.Point(12, 27);
            this.picRenderOutput.Name = "picRenderOutput";
            this.picRenderOutput.Size = new System.Drawing.Size(256, 128);
            this.picRenderOutput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picRenderOutput.TabIndex = 0;
            this.picRenderOutput.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(280, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadROMToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadROMToolStripMenuItem
            // 
            this.loadROMToolStripMenuItem.Name = "loadROMToolStripMenuItem";
            this.loadROMToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.loadROMToolStripMenuItem.Text = "Load ROM...";
            this.loadROMToolStripMenuItem.Click += new System.EventHandler(this.loadROMToolStripMenuItem_Click);
            // 
            // chkShowDebug
            // 
            this.chkShowDebug.AutoSize = true;
            this.chkShowDebug.Location = new System.Drawing.Point(12, 373);
            this.chkShowDebug.Name = "chkShowDebug";
            this.chkShowDebug.Size = new System.Drawing.Size(103, 17);
            this.chkShowDebug.TabIndex = 2;
            this.chkShowDebug.Text = "Show Debugger";
            this.chkShowDebug.UseVisualStyleBackColor = true;
            this.chkShowDebug.CheckedChanged += new System.EventHandler(this.chkShowDebug_CheckedChanged);
            // 
            // grpKeys
            // 
            this.grpKeys.Location = new System.Drawing.Point(12, 161);
            this.grpKeys.Name = "grpKeys";
            this.grpKeys.Size = new System.Drawing.Size(256, 196);
            this.grpKeys.TabIndex = 3;
            this.grpKeys.TabStop = false;
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(126, 369);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(41, 23);
            this.btnStep.TabIndex = 4;
            this.btnStep.Text = "Step";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(173, 369);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(43, 23);
            this.btnRun.TabIndex = 5;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(222, 369);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(43, 23);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 402);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.grpKeys);
            this.Controls.Add(this.chkShowDebug);
            this.Controls.Add(this.picRenderOutput);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "C#IP 8";
            ((System.ComponentModel.ISupportInitialize)(this.picRenderOutput)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picRenderOutput;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadROMToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkShowDebug;
        private System.Windows.Forms.GroupBox grpKeys;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnReset;
    }
}

