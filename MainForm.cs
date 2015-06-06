/* Copyright 2008
 * Author: Jeremy Brooks
 * Please do not use or copy source without permission.
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CHIP8
{
    public partial class MainForm : Form
    {
        private bool run = false;
        private CHIP8 virtualMachine = null;
        private DebuggerForm debugForm = null;
        private Utility.FastBitmap frameBuffer = null;
        public MainForm()
        {
            InitializeComponent();

            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

            frameBuffer = new Utility.FastBitmap(new Bitmap(64, 32));
            picRenderOutput.Image = frameBuffer.Bitmap;

            //set up keypad
            //lets put '0' key bottom center
            Button btn = new Button();
            btn.Width = grpKeys.Width / 3 - 10;
            btn.Left = (btn.Width + 5) * 1 + 10;
            btn.Top = (btn.Height + 5) * 5 + 15;
            btn.Text = "0";//display as hex value
            btn.Tag = (byte)0;
            btn.MouseDown += new MouseEventHandler(btn_MouseDown);
            btn.MouseUp += new MouseEventHandler(btn_MouseUp);
            grpKeys.Controls.Add(btn);

            KeyDown += new KeyEventHandler(MainForm_KeyDown);
            KeyUp += new KeyEventHandler(MainForm_KeyUp);

            //lay out remaining keys on a 3x4 grid
            for (byte i = 1; i < 16; ++i)
            {
                btn = new Button();
                btn.Width = grpKeys.Width / 3 - 10;
                btn.Left = (btn.Width+5) * ((i-1) % 3) + 10;
                btn.Top = (btn.Height + 5) * (4 - ((i - 1) / 3)) + 15;
                btn.Text = i.ToString("X");//display as hex value
                btn.Tag = i;
                btn.MouseDown += new MouseEventHandler(btn_MouseDown);
                btn.MouseUp += new MouseEventHandler(btn_MouseUp);
                grpKeys.Controls.Add(btn);
            }

            virtualMachine = new CHIP8();

            UpdateFrameBuffer();
        }

        void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            byte button_bit = (byte)(e.KeyCode - 96);
            if (((virtualMachine.key_press >> button_bit) & 0x1) == 0x1)
            {
                virtualMachine.key_press ^= (short)(0x1 << button_bit);
            }
        }

        void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            byte button_bit = (byte)(e.KeyCode-96);
            virtualMachine.key_press |= (short)(0x1 << button_bit);
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            run = false;
        }

        void btn_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            byte button_bit = (byte)btn.Tag;
            if (((virtualMachine.key_press >> button_bit) & 0x1) == 0x1)
            {
                virtualMachine.key_press ^= (short)(0x1 << button_bit);
            }
        }

        void btn_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            byte button_bit = (byte)btn.Tag;
            virtualMachine.key_press |= (short)(0x1 << button_bit);
        }

        private void loadROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //let user browse for file
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            if (DialogResult.OK == fileDialog.ShowDialog())
            {
                virtualMachine.Reset();
                UpdateFrameBuffer();

                //open file for reading
                System.IO.FileStream fileReader = new System.IO.FileStream(fileDialog.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //read into main memory array, starting at location 0x200, and with location 0xE9F as the upper bound
                fileReader.Read(virtualMachine.mainMemory, 0x200, (0xE9F - 0x200)+1);
                fileReader.Close();
            }

            if (debugForm != null)
            {
                debugForm.Repopulate();
            }
        }

        private void chkShowDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowDebug.Checked == true)
            {
                debugForm = new DebuggerForm(virtualMachine);
                debugForm.FormClosing += new FormClosingEventHandler(debugForm_FormClosing);
                debugForm.Show(this);
            }
            else
            {
                if (debugForm != null)
                {
                    debugForm.Close();
                    debugForm = null;
                }
            }
        }

        void debugForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            chkShowDebug.Checked = false;
        }

        //draw virtual machine internal framebuffer to our picture box for viewing
        void UpdateFrameBuffer()
        {
            frameBuffer.LockBitmap();
            unsafe
            {
                for (int y = 0; y < 32; ++y)
                {
                    for (int x = 0; x < 64; ++x)
                    {
                        byte pixel = virtualMachine.frameBuffer[y * 64 + x];
                        Color color = ( pixel == 0x1 ) ? Color.White : Color.Black;
                        Utility.PixelData* pPixel = frameBuffer[x, y];
                        pPixel->red = color.R;
                        pPixel->green = color.G;
                        pPixel->blue = color.B;
                    }
                }
                frameBuffer.UnlockBitmap();
            }

            picRenderOutput.Refresh();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            run = false;
            virtualMachine.FetchDecodeExecute();
            UpdateFrameBuffer();

            if (debugForm != null)
            {
                debugForm.UpdateValues();
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            run = true;
            while (run)
            {
                virtualMachine.FetchDecodeExecute();
                UpdateFrameBuffer();

                if (debugForm != null)
                {
                    debugForm.UpdateValues();
                }
                Application.DoEvents();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            virtualMachine.Reset();
        }
    }
}