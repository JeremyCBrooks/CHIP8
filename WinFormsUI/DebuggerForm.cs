using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CHIP8
{
    public partial class DebuggerForm : Form
    {
        private LibCHIP8.CHIP8 virtualMachine = null;

        public DebuggerForm(LibCHIP8.CHIP8 vm)
        {
            InitializeComponent();

            virtualMachine = vm;

            Repopulate();
        }

        //e
        public void Repopulate()
        {
            grdMainMemory.Items.Clear();

            //let's examine each opcode
            short opcode;
            for (int i = 0x200; i < 0xEA0; i += 2)
            {
                opcode = (short)((virtualMachine.MainMemory[i] << 8) | virtualMachine.MainMemory[i + 1]);
                ListViewItem row = new ListViewItem(i.ToString("X").PadLeft(4));
                row.Tag = i;
                row.SubItems.Add(opcode.ToString("X").PadLeft(4, '0'));
                row.SubItems.Add(virtualMachine.ToAssembly(opcode));
                grdMainMemory.Items.Add(row);
            }

            flowLayoutPanel1.Controls.Clear();

            //examine each register
            for (int i = 0; i < 16; ++i)
            {
                Label label = new Label();
                label.AutoSize = false;
                label.Width = 55;
                label.Height = 35;
                label.Text = "V" + i.ToString("X").PadLeft(1) + ": " + virtualMachine.V[i].ToString("X").PadLeft(2);
                label.TextAlign = ContentAlignment.MiddleRight;
                label.Visible = true;
                flowLayoutPanel1.Controls.Add(label);
            }

            UpdateValues();
        }

        public void UpdateValues()
        {
            grdMainMemory.Items[(virtualMachine.ProgramCounter-0x200)/2].Selected = true;
            grdMainMemory.EnsureVisible((virtualMachine.ProgramCounter - 0x200) / 2);

            lblIndexRegister.Text = "I: " + virtualMachine.I.ToString("X").PadLeft(2);
            lblProgramCounter.Text = "PC: " + virtualMachine.ProgramCounter.ToString("X").PadLeft(3);
            lblStackPointer.Text = "SP: " + virtualMachine.StackPointer.ToString("X").PadLeft(3);
            lblDelayTimer.Text = "DT: " + virtualMachine.DelayTimer.ToString("X").PadLeft(2);
            lblSoundTimer.Text = "ST: " + virtualMachine.SoundTimer.ToString("X").PadLeft(2);

            //update each register
            for (int i = 0; i < 16; ++i)
            {
                Label label = flowLayoutPanel1.Controls[i] as Label;
                label.Text = "V" + i.ToString("X").PadLeft(1) + ": " + virtualMachine.V[i].ToString("X").PadLeft(2);
            }
        }
    }
}