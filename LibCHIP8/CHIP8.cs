/* Copyright 2008
 * Author: Jeremy Brooks
 * Please do not use or copy source without permission.
 * */

using System;
/*
 * http://en.wikipedia.org/wiki/CHIP-8
 * http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
 * 
 * opcodes
 * 
0NNN 	Calls RCA 1802 program at address NNN.
00E0 	Clears the screen.
00EE 	Returns from a subroutine.
1NNN 	Jumps to address NNN.
2NNN 	Calls subroutine at NNN.
3XNN 	Skips the next instruction if VX equals NN.
4XNN 	Skips the next instruction if VX doesn't equal NN.
5XY0 	Skips the next instruction if VX equals VY.
6XNN 	Sets VX to NN.
7XNN 	Adds NN to VX.
8XY0 	Sets VX to the value of VY.
8XY1 	Sets VX to VX or VY.
8XY2 	Sets VX to VX and VY.
8XY3 	Sets VX to VX xor VY.
8XY4 	Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't.
8XY5 	VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
8XY6 	Shifts VX right by one. VF is set to the value of the least significant bit of VX before the shift. [1]
8XY7 	Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
8XYE 	Shifts VX left by one. VF is set to the value of the most significant bit of VX before the shift. [1]
9XY0 	Skips the next instruction if VX doesn't equal VY.
ANNN 	Sets I to the address NNN.
BNNN 	Jumps to the address NNN plus V0.
CXNN 	Sets VX to a random number and NN.
DXYN 	Draws a sprite at coordinate (VX, VY) that has a width of 8 pixels and a height of N pixels. As described above, VF is set to 1 if any screen pixels are flipped from set to unset when the sprite is drawn, and to 0 if that doesn't happen.
EX9E 	Skips the next instruction if the key stored in VX is pressed.
EXA1 	Skips the next instruction if the key stored in VX isn't pressed.
FX07 	Sets VX to the value of the delay timer.
FX0A 	A key press is awaited, and then stored in VX.
FX15 	Sets the delay timer to VX.
FX18 	Sets the sound timer to VX.
FX1E 	Adds VX to I.
FX29 	Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
FX33 	Stores the Binary-coded decimal representation of VX at the addresses I, I plus 1, and I plus 2.
FX55 	Stores V0 to VX in memory starting at address I. [2]
FX65 	Fills V0 to VX with values from memory starting at address I. [2] 
*/
namespace LibCHIP8
{
    public class CHIP8
    {
        //main memory
        public byte[] MainMemory = new byte[4096];

        //registers
        public byte[] V = new byte[16];//general purpose register (see docs on VF (V[15]), it has some special uses). technically theses are bytes, but we can use short to store and detect overflows
        public short I = 0;//special address register

        //keypress input (16 keys)
        public short KeysPressed = 0;

        //timers
        public byte DelayTimer = 0;
        public byte SoundTimer = 0;

        //graphics
        public const byte FRAME_WIDTH = 64;
        public const byte FRAME_HEIGHT = 32;
        public byte[] FrameBuffer = new byte[FRAME_WIDTH * FRAME_HEIGHT];

        //internals
        public short StackPointer = 0xEA0;
        public short ProgramCounter = 0x200;
        private Random rng = new Random((int)System.DateTime.Now.Ticks);//random number generator

        //convert opcode to human readable assembly instruction
        public string ToAssembly(short opcode)
        {
            byte instruction = (byte)((opcode & 0xF000) >> 12);
            short parameters = (short)(opcode & 0x0FFF);
            switch (instruction)
            {
                case 0x0:
                {
                    if (parameters == 0x0E0) return "CLS";
                    else if (parameters == 0x0EE) return "RTN";
                    else return "RCA " + parameters.ToString("X").PadLeft(3);
                }

                case 0x1:
                {
                    return "JP " + parameters.ToString("X").PadLeft(3);
                }

                case 0x2:
                {
                    return "CALL " + parameters.ToString("X").PadLeft(3);
                }

                case 0x3:
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte value = (byte)(parameters & 0x00FF);
                    return "SE V" + register.ToString("X").PadLeft(1) + ", " + value.ToString("X").PadLeft(2);
                }

                case 0x4:
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte value = (byte)(parameters & 0x00FF);
                    return "SNE V" + register.ToString("X").PadLeft(1) + ", " + value.ToString("X").PadLeft(2);
                }

                case 0x5:
                {
                    byte registerX = (byte)((parameters & 0x0F00) >> 8);
                    byte registerY = (byte)((parameters & 0x00F0) >> 4);
                    return "SRE V" + registerX.ToString("X").PadLeft(1) + ", V" + registerY.ToString("X").PadLeft(1);
                }

                case 0x6:
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte value = (byte)(parameters & 0x00FF);
                    return "LD V" + register.ToString("X").PadLeft(1) + ", " + value.ToString("X").PadLeft(2);
                }

                case 0x7:
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte value = (byte)(parameters & 0x00FF);
                    return "ADD V" + register.ToString("X").PadLeft(1) + ", " + value.ToString("X").PadLeft(2);
                }

                case 0x8:
                {
                    //the last 4 bits tell what type of operation
                    byte operation = (byte)(parameters & 0x000F);
                    byte registerX = (byte)((parameters & 0x0F00) >> 8);
                    byte registerY = (byte)((parameters & 0x00F0) >> 4);
                    if(operation == 0x0)//8XY0 	Sets VX to the value of VY.
                    {
                        return "LD V" + registerX.ToString("X").PadLeft(1) + ", V" + registerY.ToString("X").PadLeft(1);
                    }
                    else if (operation == 0x1)//8XY1 	Sets VX to VX or VY.
                    {
                        return "OR V" + registerX.ToString("X").PadLeft(1) + ", V" + registerY.ToString("X").PadLeft(1);
                    }
                    else if (operation == 0x2)//8XY2 	Sets VX to VX and VY.
                    {
                        return "AND V" + registerX.ToString("X").PadLeft(1) + ", V" + registerY.ToString("X").PadLeft(1);
                    }
                    else if (operation == 0x3)//8XY3 	Sets VX to VX xor VY.
                    {
                        return "XOR V" + registerX.ToString("X").PadLeft(1) + ", V" + registerY.ToString("X").PadLeft(1);
                    }
                    else if(operation == 0x4)//8XY4 	Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't.
                    {
                        return "ADD V" + registerX.ToString("X").PadLeft(1) + ", V" + registerY.ToString("X").PadLeft(1);
                    }
                    else if(operation == 0x5)//8XY5 	VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                    {
                        return "SUB V" + registerX.ToString("X").PadLeft(1) + ", V" + registerY.ToString("X").PadLeft(1);
                    }
                    else if(operation == 0x6)//8XY6 	Shifts VX right by one. VF is set to the value of the least significant bit of VX before the shift. [1]
                    {
                        return "RSFT V" + registerX.ToString("X").PadLeft(1);
                    }
                    else if(operation == 0x7)//8XY7 	Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                    {
                        return "SUB2 V" + registerX.ToString("X").PadLeft(1) + ", V" + registerY.ToString("X").PadLeft(1);
                    }
                    else if (operation == 0xE)//8XYE 	Shifts VX left by one. VF is set to the value of the most significant bit of VX before the shift. [1]
                    {
                        return "LSFT V" + registerX.ToString("X").PadLeft(1);
                    }

                    break;
                }

                case 0x9:
                {
                    byte registerX = (byte)((parameters & 0x0F00) >> 8);
                    byte registerY = (byte)((parameters & 0x00F0) >> 4);
                    return "SNE V" + registerX.ToString("X").PadLeft(1) + ", V" + registerY.ToString("X").PadLeft(1);
                }

                case 0xA:
                {
                    return "LD I, " + parameters.ToString("X").PadLeft(3);
                }

                case 0xB:
                {
                    return "JMP " + parameters.ToString("X").PadLeft(3) + " + V0";
                }

                case 0xC:
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte value = (byte)(parameters & 0x00FF);
                    return "RND V" + register.ToString("X").PadLeft(1) + ", " + value.ToString("X").PadLeft(2);
                }

                case 0xD:
                {
                    byte registerX = (byte)((parameters & 0x0F00) >> 8);
                    byte registerY = (byte)((parameters & 0x00F0) >> 4);
                    byte value = (byte)(parameters & 0x000F);
                    return "DRW V" + registerX.ToString("X").PadLeft(1) + ", V" + registerY.ToString("X").PadLeft(1) + ", " + value.ToString("X").PadLeft(1);
                }

                case 0xE:
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte operation = (byte)(parameters & 0x00FF);
                    if(operation == 0x9E)//EX9E 	Skips the next instruction if the key stored in VX is pressed.
                    {
                        return "SKP V" + register.ToString("X").PadLeft(1);
                    }
                    else if(operation == 0xA1)//EXA1 	Skips the next instruction if the key stored in VX isn't pressed.
                    {
                        return "SKNP V" + register.ToString("X").PadLeft(1);
                    }
                    break;
                }

                case 0xF:
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte operation = (byte)(parameters & 0x00FF);
                    if (operation == 0x07)//FX07 	Sets VX to the value of the delay timer.
                    {
                        return "LD V" + register.ToString("X").PadLeft(1) + ", DT";
                    }
                    else if (operation == 0x0A)//FX0A 	A key press is awaited, and then stored in VX.
                    {
                        return "WKP V" + register.ToString("X").PadLeft(1);
                    }
                    else if (operation == 0x15)//FX15 	Sets the delay timer to VX.
                    {
                        return "LD DT, V" + register.ToString("X").PadLeft(1);
                    }
                    else if (operation == 0x18)//FX18 	Sets the sound timer to VX.
                    {
                        return "LD ST, V" + register.ToString("X").PadLeft(1);
                    }
                    else if (operation == 0x1E)//FX1E 	Adds VX to I.
                    {
                        return "ADD I, V" + register.ToString("X").PadLeft(1);
                    }
                    else if (operation == 0x29)//FX29 	Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
                    {
                        return "FX29 I, V" + register.ToString("X").PadLeft(1);
                    }
                    else if (operation == 0x33)//FX33 	Stores the Binary-coded decimal representation of VX at the addresses I, I plus 1, and I plus 2.
                    {
                        return "FX33 I, V" + register.ToString("X").PadLeft(1);
                    }
                    else if (operation == 0x55)//FX55 	Stores V0 to VX in memory starting at address I. [2]
                    {
                        return "FX55 I, V" + register.ToString("X").PadLeft(1);
                    }
                    else if (operation == 0x65)//FX65 	Fills V0 to VX with values from memory starting at address I. [2] 
                    {
                        return "FX65 I, V" + register.ToString("X").PadLeft(1);
                    }
                    break;
                }
            }
            return "invalide opcode";
        }

        //resets internal buffers, but leaves program instruction and parameters area as they were before call to Reset
        public void Reset()
        {
            StackPointer = 0xEA0;
            ProgramCounter = 0x200;

            //only clear stack area, leave program intact
            Array.Clear(MainMemory, 0xEA0, 0x1000 - 0xEA0);

            //clear registers
            Array.Clear(V, 0, V.Length);
            I = 0;

            //clear keys
            KeysPressed = 0;

            //reset timers
            DelayTimer = 0;
            SoundTimer = 0;

            //clear frame buffer
            Array.Clear(FrameBuffer, 0, FrameBuffer.Length);
        }

        //execute one instruction and return
        public void FetchDecodeExecute()
        {
            //update timers
            if (DelayTimer > 0)
            {
                --DelayTimer;
            }

            if (SoundTimer > 0)
            {
                --SoundTimer;
            }

            //fetch
            short opcode = (short)(MainMemory[ProgramCounter] << 8 | MainMemory[ProgramCounter + 1]);

            //decode and execute
            int instruction = (int)(opcode & 0xF000);
            short parameters = (short)(opcode & 0x0FFF);
            switch (instruction)
            {
                case 0x0000:
                {
                    if (parameters == 0x0E0)//clear screen
                    {
                        Array.Clear(FrameBuffer, 0, FrameBuffer.Length);
                    }
                    else if (parameters == 0x0EE)
                    {
                        //decrement stack pointer
                        StackPointer -= 2;
                        //get address off of the stack
                        ProgramCounter = (short)( (MainMemory[StackPointer]<<8) | MainMemory[StackPointer+1] );
                    }
                    ProgramCounter+=2;

                    break;
                }

                case 0x1000:
                {
                    ProgramCounter = parameters;

                    break;
                }

                case 0x2000:
                {
                    //push program counter onto stack
                    MainMemory[StackPointer] = (byte)( (ProgramCounter >> 8) & 0xFF );
                    MainMemory[StackPointer+1] = (byte)( ProgramCounter & 0xFF );
                    //increment stack pointer
                    StackPointer+=2;
                    //jump to new address
                    ProgramCounter = parameters;

                    break;
                }

                case 0x3000://skip if equal
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte value = (byte)(parameters & 0x00FF);
                    if(V[register] == value)
                    {
                        ProgramCounter+=2;
                    }
                    ProgramCounter+=2;

                    break;
                }

                case 0x4000://skip if not equal
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte value = (byte)(parameters & 0x00FF);
                    if(V[register] != value)
                    {
                        ProgramCounter+=2;
                    }
                    ProgramCounter+=2;

                    break;
                }

                case 0x5000://skip if registers are equal
                {
                    byte registerX = (byte)((parameters & 0x0F00) >> 8);
                    byte registerY = (byte)((parameters & 0x00F0) >> 4);
                    if(V[registerX] == V[registerY])
                    {
                        ProgramCounter+=2;
                    }
                    ProgramCounter+=2;

                    break;
                }

                case 0x6000:
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    V[register] = (byte)(parameters & 0x00FF);
                    ProgramCounter+=2;

                    break;
                }

                case 0x7000:
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte value = (byte)(parameters & 0x00FF);
                    short result = (short)(V[register] + value);
                    V[register] = (byte)(result & 0x00FF);
                    V[0xF] = (byte)((result >> 8) & 0x1);

                    ProgramCounter+=2;

                    break;
                }

                case 0x8000:
                {
                    //the last 4 bits tell what type of operation
                    byte operation = (byte)(parameters & 0x000F);
                    byte registerX = (byte)((parameters & 0x0F00) >> 8);
                    byte registerY = (byte)((parameters & 0x00F0) >> 4);
                    if(operation == 0x0)//8XY0 	Sets VX to the value of VY.
                    {
                        V[registerX] = V[registerY];
                    }
                    else if (operation == 0x1)//8XY1 	Sets VX to VX or VY.
                    {
                        V[registerX] |= V[registerY];
                    }
                    else if (operation == 0x2)//8XY2 	Sets VX to VX and VY.
                    {
                        V[registerX] &= V[registerY];
                    }
                    else if (operation == 0x3)//8XY3 	Sets VX to VX xor VY.
                    {
                        V[registerX] ^= V[registerY];
                    }
                    else if(operation == 0x4)//8XY4 	Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't.
                    {
                        short result = (short)(V[registerX] + V[registerY]);
                        V[registerX] = (byte)(result & 0xFF);
                        V[0xF] = (byte)((result >> 8)>0?1:0);
                    }
                    else if(operation == 0x5)//8XY5 	VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                    {
                        short result = (short)((short)V[registerX] - (short)V[registerY]);
                        V[registerX] = (byte)(result & 0xFF);
                        V[0xF] = (byte)((result >> 8)>0?0:1);
                    }
                    else if(operation == 0x6)//8XY6 	Shifts VX right by one. VF is set to the value of the least significant bit of VX before the shift. [1]
                    {
                        V[0xF] = (byte)(V[registerX] & 0x01);
                        V[registerX] = (byte)(V[registerX] >> 1);
                    }
                    else if(operation == 0x7)//8XY7 	Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                    {
                        short result = (short)((short)V[registerY] - (short)V[registerX]);
                        V[registerX] = (byte)(result & 0xFF);
                        V[0xF] = (byte)((result >> 8) > 0 ? 0 : 1);
                    }
                    else if (operation == 0xE)//8XYE 	Shifts VX left by one. VF is set to the value of the most significant bit of VX before the shift. [1]
                    {
                        V[0xF] = (byte)(V[registerX] >> 7);
                        V[registerX] = (byte)(V[registerX] << 1);
                    }
                    ProgramCounter+=2;

                    break;
                }

                case 0x9000://skip if registers are not equal
                {
                    byte registerX = (byte)((parameters & 0x0F00) >> 8);
                    byte registerY = (byte)((parameters & 0x00F0) >> 4);
                    if(V[registerX] != V[registerY])
                    {
                        ProgramCounter+=2;
                    }
                    ProgramCounter+=2;

                    break;
                }

                case 0xA000:
                {
                    I = parameters;
                    ProgramCounter+=2;

                    break;
                }

                case 0xB000://jump V[0] + XXX
                {
                    ProgramCounter = (short)(parameters + V[0]);

                    break;
                }

                case 0xC000://random
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte value =    (byte)(parameters & 0x00FF);
                    V[register] =   (byte)(rng.Next(0, 255) & value);
                    ProgramCounter+=2;

                    break;
                }

                case 0xD000://draw sprite to frame buffer
                {
                    byte registerX = (byte)((parameters & 0x0F00) >> 8);//location of x
                    byte registerY = (byte)((parameters & 0x00F0) >> 4);//location of y

                    byte x = V[registerX];
                    byte start_y = V[registerY];
                    byte sprite_height = (byte)(parameters & 0x000F);
                    int y_end = start_y + sprite_height;
                    V[0xF] = 0x0;//assume no collision at first
                    for (int y = start_y; y < y_end && y < FRAME_HEIGHT; ++y)
                    {
                        //fetch sprite from main memory
                        byte sprite = MainMemory[I+y-start_y];
                        int index = y * FRAME_WIDTH + x;
                        for (int pixelX = 0; x + pixelX < FRAME_WIDTH && pixelX < 8; ++pixelX)
                        {
                            //collision occurs if pixel was flipped from active to inactive, but we only need to set the flag once per draw command
                            if (((sprite << pixelX) & 0x80) == 0x80)
                            {
                                int pxoff = index + pixelX;
                                if (FrameBuffer[pxoff] == 0x1)
                                {
                                    V[0xF] = 0x1;//collision
                                }
                                FrameBuffer[pxoff] ^= 0x1;
                            }
                        }
                    }
                    ProgramCounter+=2;

                    break;
                }

                case 0xE000:
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte operation = (byte)(parameters & 0x00FF);
                    if(operation == 0x9E)//EX9E 	Skips the next instruction if the key stored in VX is pressed.
                    {
                        if (((KeysPressed >> V[register]) & 0x1) == 0x1)
                        {
                            ProgramCounter+=2;
                        }
                    }
                    else if(operation == 0xA1)//EXA1 	Skips the next instruction if the key stored in VX isn't pressed.
                    {
                        if (!(((KeysPressed >> V[register]) & 0x1) == 0x1))
                        {
                            ProgramCounter+=2;
                        }
                    }
                    ProgramCounter+=2;

                    break;
                }

                case 0xF000:
                {
                    byte register = (byte)((parameters & 0x0F00) >> 8);
                    byte operation = (byte)(parameters & 0x00FF);
                    if (operation == 0x07)//FX07 	Sets VX to the value of the delay timer.
                    {
                        V[register] = DelayTimer;
                    }
                    else if (operation == 0x0A)//FX0A 	wait for keypress, and then stored in VX.
                    {
                        if (KeysPressed > 0)
                        {
                            for (byte i = 0; i < 16; ++i)
                            {
                                if ( ((KeysPressed >> i) & 0x1) == 0x1)
                                {
                                    V[register] = i;
                                    break;
                                }
                            }
                        }
                    }
                    else if (operation == 0x15)//FX15 	Sets the delay timer to VX.
                    {
                        DelayTimer = V[register];
                    }
                    else if (operation == 0x18)//FX18 	Sets the sound timer to VX.
                    {
                        SoundTimer = V[register];
                    }
                    else if (operation == 0x1E)//FX1E 	Adds VX to I.
                    {
                        I += V[register];
                    }
                    else if (operation == 0x29)//FX29 	Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
                    {
                        I = (short)(V[register]*5);
                    }
                    else if (operation == 0x33)//FX33 	Stores the Binary-coded decimal representation of VX at the addresses I, I plus 1, and I plus 2.
                    {
                        MainMemory[I + 0] = (byte)((V[register] / 100) & 0x0F);
                        MainMemory[I + 1] = (byte)((V[register] / 10) & 0x0F);
                        MainMemory[I + 2] = (byte)(V[register] & 0x0F);
                    }
                    else if (operation == 0x55)//FX55 	Stores V0 to VX in memory starting at address I. [2]
                    {
                        for (int i = 0; i <= register; ++i)
                        {
                            MainMemory[I + i] = (byte)( V[i] & 0xFF);
                        }
                    }
                    else if (operation == 0x65)//FX65 	Fills V0 to VX with values from memory starting at address I. [2] 
                    {
                        for (int i = 0; i <= register; ++i)
                        {
                            V[i] = MainMemory[I + i];
                        }
                    }
                    ProgramCounter += 2;

                    break;
                }
            }
        }
    }
}
