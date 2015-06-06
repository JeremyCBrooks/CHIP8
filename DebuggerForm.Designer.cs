namespace CHIP8
{
    partial class DebuggerForm
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
            this.grpRegisters = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grdMainMemory = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.lblIndexRegister = new System.Windows.Forms.Label();
            this.lblStackPointer = new System.Windows.Forms.Label();
            this.lblProgramCounter = new System.Windows.Forms.Label();
            this.lblDelayTimer = new System.Windows.Forms.Label();
            this.lblSoundTimer = new System.Windows.Forms.Label();
            this.grpRegisters.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpRegisters
            // 
            this.grpRegisters.Controls.Add(this.flowLayoutPanel1);
            this.grpRegisters.Location = new System.Drawing.Point(12, 12);
            this.grpRegisters.Name = "grpRegisters";
            this.grpRegisters.Size = new System.Drawing.Size(256, 165);
            this.grpRegisters.TabIndex = 4;
            this.grpRegisters.TabStop = false;
            this.grpRegisters.Text = "Registers";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(250, 146);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.grdMainMemory);
            this.groupBox1.Location = new System.Drawing.Point(12, 243);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 178);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Main Memory";
            // 
            // grdMainMemory
            // 
            this.grdMainMemory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.grdMainMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdMainMemory.FullRowSelect = true;
            this.grdMainMemory.HideSelection = false;
            this.grdMainMemory.LabelEdit = true;
            this.grdMainMemory.Location = new System.Drawing.Point(3, 16);
            this.grdMainMemory.MultiSelect = false;
            this.grdMainMemory.Name = "grdMainMemory";
            this.grdMainMemory.Size = new System.Drawing.Size(250, 159);
            this.grdMainMemory.TabIndex = 6;
            this.grdMainMemory.UseCompatibleStateImageBehavior = false;
            this.grdMainMemory.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Address";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Assembly";
            // 
            // lblIndexRegister
            // 
            this.lblIndexRegister.AutoSize = true;
            this.lblIndexRegister.Location = new System.Drawing.Point(12, 190);
            this.lblIndexRegister.Name = "lblIndexRegister";
            this.lblIndexRegister.Size = new System.Drawing.Size(13, 13);
            this.lblIndexRegister.TabIndex = 6;
            this.lblIndexRegister.Text = "I:";
            // 
            // lblStackPointer
            // 
            this.lblStackPointer.AutoSize = true;
            this.lblStackPointer.Location = new System.Drawing.Point(74, 190);
            this.lblStackPointer.Name = "lblStackPointer";
            this.lblStackPointer.Size = new System.Drawing.Size(24, 13);
            this.lblStackPointer.TabIndex = 7;
            this.lblStackPointer.Text = "SP:";
            // 
            // lblProgramCounter
            // 
            this.lblProgramCounter.AutoSize = true;
            this.lblProgramCounter.Location = new System.Drawing.Point(137, 190);
            this.lblProgramCounter.Name = "lblProgramCounter";
            this.lblProgramCounter.Size = new System.Drawing.Size(24, 13);
            this.lblProgramCounter.TabIndex = 8;
            this.lblProgramCounter.Text = "PC:";
            // 
            // lblDelayTimer
            // 
            this.lblDelayTimer.AutoSize = true;
            this.lblDelayTimer.Location = new System.Drawing.Point(12, 218);
            this.lblDelayTimer.Name = "lblDelayTimer";
            this.lblDelayTimer.Size = new System.Drawing.Size(25, 13);
            this.lblDelayTimer.TabIndex = 9;
            this.lblDelayTimer.Text = "DT:";
            // 
            // lblSoundTimer
            // 
            this.lblSoundTimer.AutoSize = true;
            this.lblSoundTimer.Location = new System.Drawing.Point(74, 218);
            this.lblSoundTimer.Name = "lblSoundTimer";
            this.lblSoundTimer.Size = new System.Drawing.Size(24, 13);
            this.lblSoundTimer.TabIndex = 10;
            this.lblSoundTimer.Text = "ST:";
            // 
            // DebuggerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 433);
            this.Controls.Add(this.lblSoundTimer);
            this.Controls.Add(this.lblDelayTimer);
            this.Controls.Add(this.lblProgramCounter);
            this.Controls.Add(this.lblStackPointer);
            this.Controls.Add(this.lblIndexRegister);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpRegisters);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DebuggerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Debugger";
            this.grpRegisters.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpRegisters;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView grdMainMemory;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label lblIndexRegister;
        private System.Windows.Forms.Label lblStackPointer;
        private System.Windows.Forms.Label lblProgramCounter;
        private System.Windows.Forms.Label lblDelayTimer;
        private System.Windows.Forms.Label lblSoundTimer;
    }
}