/* Copyright 2008
 * Author: Jeremy Brooks
 * Please do not use or copy source without permission.
 * */

using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CHIP8
{
    public partial class MainForm : Form
    {
        private LibCHIP8.CHIP8 virtualMachine = null;
        private DebuggerForm debugForm = null;
        private bool running;
        private bool loaded;

        public MainForm()
        {
            InitializeComponent();

            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

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

            virtualMachine = new LibCHIP8.CHIP8();

            UpdateFrameBuffer();
        }

        void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            byte button_bit = (byte)(e.KeyCode - 96);
            if (((virtualMachine.KeysPressed >> button_bit) & 0x1) == 0x1)
            {
                virtualMachine.KeysPressed ^= (short)(0x1 << button_bit);
            }
        }

        void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            byte button_bit = (byte)(e.KeyCode-96);
            virtualMachine.KeysPressed |= (short)(0x1 << button_bit);
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            running = false;
        }

        void btn_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            byte button_bit = (byte)btn.Tag;
            if (((virtualMachine.KeysPressed >> button_bit) & 0x1) == 0x1)
            {
                virtualMachine.KeysPressed ^= (short)(0x1 << button_bit);
            }
        }

        void btn_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            byte button_bit = (byte)btn.Tag;
            virtualMachine.KeysPressed |= (short)(0x1 << button_bit);
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
                fileReader.Read(virtualMachine.MainMemory, 0x200, (0xE9F - 0x200)+1);
                fileReader.Close();
            }

            if (debugForm != null)
            {
                debugForm.Repopulate();
            }
        }

        private void chkShowDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowDebug.Checked)
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
                }
                debugForm = null;
            }
        }

        void debugForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            chkShowDebug.Checked = false;
        }

        byte[] pixels;
        void UpdateFrameBuffer()
        {
            if (loaded)
            {
                for (int y = 0; y < 32; ++y)
                {
                    for (int x = 0; x < 64; ++x)
                    {
                        int offset = y * 64 + x;
                        pixels[offset] = (byte)(virtualMachine.FrameBuffer[offset] * 255);
                    }
                }

                glControl.MakeCurrent();

                GL.TexSubImage2D(   TextureTarget.Texture2D, 0, 0, 0, 64, 32,
                                    OpenTK.Graphics.OpenGL.PixelFormat.Luminance, PixelType.UnsignedByte, pixels);

                GL.Begin(PrimitiveType.Quads);

                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex2(0, 0);

                GL.TexCoord2(1.0, 0.0f);
                GL.Vertex2(1, 0);

                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex2(1, 1);

                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex2(0, 1);

                GL.End();

                glControl.SwapBuffers();
            }

        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            running = false;
            StepEmulator();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            running = true;
            while (running)
            {
                StepEmulator();
                System.Threading.Thread.Sleep(1);
                Application.DoEvents();
            }
        }

        private void StepEmulator()
        {
            virtualMachine.FetchDecodeExecute();
            UpdateFrameBuffer();

            if (debugForm != null)
            {
                debugForm.UpdateValues();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            virtualMachine.Reset();
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            loaded = true;
            glInit();
            UpdateFrameBuffer();
        }

        int textureId;
        private void glInit()
        {
            pixels = new byte[64 * 32];
            Array.Clear(pixels, 0, pixels.Length);

            GL.Disable(EnableCap.CullFace | EnableCap.DepthTest | EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);
            GL.ClearColor(Color.HotPink);

            textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Luminance, PixelType.UnsignedByte, pixels);

            int w = glControl.Width;
            int h = glControl.Height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 1, 1, 0, -1, 1);
            GL.Viewport(0, 0, w, h);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }
    }
}