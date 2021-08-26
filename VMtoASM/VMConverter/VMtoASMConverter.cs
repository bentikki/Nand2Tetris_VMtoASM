﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMtoASM.VMConverter
{
    class VMtoASMConverter
    {
        private const string ALLOWED_INPUT_FILES = "*.vm";
        private const int THIS_THAT_SIZE = 500;



        // Input and output directories.
        private DirectoryInfo inputDirectory;
        private DirectoryInfo outputDirectory;

        // Stack to contain the converted ASM lines.
        private Queue<string> convertedLinesStack;

        // Simulation of stack functions.
        private Stack<short> globalStack;   // Global stack sim
        private short thisPointer;          // THIS pointer sim
        private short[] thisMemory;         // THIS array to contain memory

        private short thatPointer;          // THAT pointer sim
        private short[] thatMemory;         // THAT array to contain memory

        private short[] localMemory;        // LOCAL array to contain memory
        private short[] argumentMemory;     // ARG array to contain memory
        private short[] tempMemory;         // TEMP array to contain memory
        private short maxTempMemory = 12;   // TEMP max memory location

        private short[] staticMemory;           // STATIC array to contain memory
        private short staticStartMemory = 16;   // STATIC start location




        // Counter to keep track of available label numbers.
        private ulong labelCounter = 0;

        public FileInfo[] AvailableInputFiles { get; private set; }

        public VMtoASMConverter(string inputDirectory, string outputDirectory)
        {
            this.CreateStartupDirectories(inputDirectory, outputDirectory);
            this.AvailableInputFiles = this.GetAvailableInputFiles();

            // Setup this and that arrays
            this.thisMemory = new short[THIS_THAT_SIZE];
            this.thatMemory = new short[THIS_THAT_SIZE];
            this.localMemory = new short[THIS_THAT_SIZE];
            this.argumentMemory = new short[THIS_THAT_SIZE];
            this.tempMemory = new short[THIS_THAT_SIZE];
            this.staticMemory = new short[THIS_THAT_SIZE];
        }

        /// <summary>
        /// Loops through all input files and converts .VM to .ASM
        /// </summary>
        public void ConvertAllInputFiles()
        {
            foreach (FileInfo inputVMFile in this.GetAvailableInputFiles())
            {
                this.ConvertSingle(inputVMFile);
            }
        }

        /// <summary>
        /// Converts provided VM file into .ASM.
        /// Places this file in output directory.
        /// </summary>
        /// <param name="inputVMFile">VM file to convert.</param>
        public void ConvertSingle(FileInfo inputVMFile)
        {
            // Get the lines from input file - with comments and spaces stripped.
            List<string> strippedLinesFromFiles = this.GetStrippedLinesFromFile(inputVMFile);

            // Convert the provided VM lines to ASM code.
            string[] convertedLines = this.ConvertLinesToAsm(strippedLinesFromFiles);

            // Write lines to output .asm file,
            this.WriteLinesToOutputFile(convertedLines, inputVMFile.FullName);
        }

        private string[] ConvertLinesToAsm(List<string> strippedLinesFromFiles)
        {
            this.convertedLinesStack = new Queue<string>();
            this.globalStack = new Stack<short>();

            foreach (string lineRaw in strippedLinesFromFiles)
            {
                int beginningLinesCount = this.convertedLinesStack.Count;

                // Line to be added to output.
                StringBuilder convertedLineSB = new StringBuilder();

                // Convert line to lower, to make it case insensitive.
                string line = lineRaw.ToLower().Trim();

                // If the line is longer than 3 characters, we know its not a logical command (add/sub/neg etc.)
                if(line.Length > 3)
                {
                    string[] pointerArgumentArray = line.Split(" ");
                    // Check if the line is Push
                    if (pointerArgumentArray[0] == "push")
                    {

                        // Check if the line to Enqueue is a constant.
                        if (pointerArgumentArray[1] == "constant")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.AddConstant(constantNum);

                                // Advance the stack pointer.
                                this.PointerAdvance();
                            }
                        }
                        else if (pointerArgumentArray[1] == "pointer") // Check if the line to Enqueue is a constant.
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                if (constantNum == 0)
                                {
                                    this.PushPointer0();
                                }
                                else if (constantNum == 1)
                                {
                                    this.PushPointer1();
                                }
                            }
                        }
                        else if (pointerArgumentArray[1] == "this")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PushThis(constantNum);
                            }
                        }
                        else if (pointerArgumentArray[1] == "that")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PushThat(constantNum);
                            }
                        }
                        else if (pointerArgumentArray[1] == "local")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PushLocal(constantNum);
                            }
                        }
                        else if (pointerArgumentArray[1] == "argument")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PushArgument(constantNum);
                            }
                        }
                        else if (pointerArgumentArray[1] == "temp")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PushTemp(constantNum);
                            }
                        }
                        else if (pointerArgumentArray[1] == "static")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PushStatic(constantNum);
                            }
                        }

                    }
                    else if (pointerArgumentArray[0] == "pop") // Check if POP
                    {

                        // Check if the line to Enqueue is a constant.
                        if (pointerArgumentArray[1] == "pointer")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                if(constantNum == 0)
                                {
                                    this.PopPointer0();
                                }
                                else if (constantNum == 1)
                                {
                                    this.PopPointer1();
                                }
                            }
                        }
                        else if (pointerArgumentArray[1] == "this")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PopThis(constantNum);
                            }
                        }
                        else if (pointerArgumentArray[1] == "that")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PopThat(constantNum);
                            }
                        }
                        else if(pointerArgumentArray[1] == "local")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PopLocal(constantNum);
                            }
                        }
                        else if (pointerArgumentArray[1] == "argument")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PopArgument(constantNum);
                            }
                        }
                        else if (pointerArgumentArray[1] == "temp")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PopTemp(constantNum);
                            }
                        }
                        else if (pointerArgumentArray[1] == "static")
                        {
                            // Check if the constant is a number
                            if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                            {
                                this.PopStatic(constantNum);
                            }
                        }


                    }

                }
                else // line.Length <= 3
                {
                    // The line must be a logical command (add/sub/neg etc.)
                    switch (line)
                    {
                        case "add":
                            this.LogCommandAdd();
                            break;
                        case "eq":
                            this.LogCommandEq();
                            break;
                        case "lt":
                            this.LogCommandLt();
                            break;
                        case "gt":
                            this.LogCommandGt();
                            break;
                        case "sub":
                            this.LogCommandSub();
                            break;
                        case "neg":
                            this.LogCommandNeg();
                            break;
                        case "and":
                            this.LogCommandAnd();
                            break;
                        case "or":
                            this.LogCommandOr();
                            break;
                        case "not":
                            this.LogCommandNot();
                            break;
                        default:
                            break;
                    }
                }

                if (line == string.Empty)
                    throw new Exception("No known conversion was found on line: " + line);

                // Test START
                List<string> currentLinesAdded = new List<string>();
                currentLinesAdded = this.convertedLinesStack.ToList();
                currentLinesAdded.RemoveRange(0, beginningLinesCount);

                List<string> currentLinesAddedDisplay = currentLinesAdded;
                // Test END
                
            }

            return convertedLinesStack.ToArray();
        }

        private void PointerAdvance()
        {
            if (this.convertedLinesStack == null)
                throw new Exception("The Pointer can not be made to advance if the convertedLinesStack is not set.");

            // Advance the StackPointer.
            // Add ASM:
            //*
            // @SP
            // A=M
            // M=D
            // @SP
            // M=M+1
            //*
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("M=M+1");
        }

        private void PointerGoBack()
        {
            // Add ASM:
            //*
            //@SP				// Pointer go Back EQ
            //AM=M-1
            //D=M
            //A=A-1
            //D=M-D
            //*

            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("A=A-1");
            convertedLinesStack.Enqueue("D=M-D");
        }

        private void AddConstant(short constantNum)
        {
            // Stack operation
            this.globalStack.Push(constantNum);

            // Add ASM
            convertedLinesStack.Enqueue("@" + constantNum);
            convertedLinesStack.Enqueue("D=A");
        }

        private void LogCommandAdd()
        {
            // Stack operation
            short y = this.PopFromStack();
            short x = this.PopFromStack();

            int result = x + y;

            this.globalStack.Push((short)result);

            // Add ASM:
            //*
            // @SP
            // A=M
            // D=D+A
            //*

            // Add ASM

            // Old Working version - Works in ADD.VM
            //convertedLinesStack.Enqueue("@SP");
            //convertedLinesStack.Enqueue("A=M");
            //convertedLinesStack.Enqueue("D=D+A");

            // Add ASM:
            //*
            //@SP					// Add
            //AM=M-1
            //D=M
            //A=A-1
            //M=M+D
            //*

            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("A=A-1");
            convertedLinesStack.Enqueue("M=M+D");
        }

        private void LogCommandEq()
        {
            // Stack operation
            short y = this.PopFromStack();
            short x = this.PopFromStack();

            short result = this.BoolToShort(x == y);

            this.globalStack.Push(result);

            // Add ASM:
            //*
            //@SP
            //AM=M-1
            //D=M
            //A=A-1
            //D=M-D
            //@FALSE0
            //D;JNE
            //@SP
            //A=M-1
            //M=-1
            //@CONTINUE0
            //0;JMP
            //(FALSE0)
            //@SP
            //A=M-1
            //M=0
            //(CONTINUE0)
            //*

            // Add ASM
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("A=A-1");
            convertedLinesStack.Enqueue("D=M-D");
            string FALSELABEL = this.GetUniqueLabel("FALSE");
            convertedLinesStack.Enqueue("@" + FALSELABEL);
            convertedLinesStack.Enqueue("D;JNE");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M-1");
            convertedLinesStack.Enqueue("M=-1");
            string CONTINUELABEL = this.GetUniqueLabel("CONTINUE");
            convertedLinesStack.Enqueue("@" + CONTINUELABEL);
            convertedLinesStack.Enqueue("0;JMP");
            convertedLinesStack.Enqueue("(" + FALSELABEL + ")");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M-1");
            convertedLinesStack.Enqueue("M=0");
            convertedLinesStack.Enqueue("(" + CONTINUELABEL + ")");

        }

        private void LogCommandLt()
        {
            // Stack Operation
            short y = this.PopFromStack();
            short x = this.PopFromStack();

            short result = this.BoolToShort(x < y);

            this.globalStack.Push(result);

            // ASM:
            //*
            //@FALSE3			// lt
            //D;JGE
            //@SP
            //A=M-1
            //M=-1
            //@CONTINUE3
            //0;JMP
            //(FALSE3)
            //@SP
            //A=M-1
            //M=0
            //(CONTINUE3)
            //*
            this.PointerGoBack();

            string FALSElabel = this.GetUniqueLabel("FALSE");
            convertedLinesStack.Enqueue("@" + FALSElabel);
            convertedLinesStack.Enqueue("D;JGE");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M-1");
            convertedLinesStack.Enqueue("M=-1");
            string CONTUINUElabel = this.GetUniqueLabel("CONTINUE");
            convertedLinesStack.Enqueue("@" + CONTUINUElabel);
            convertedLinesStack.Enqueue("0;JMP");
            convertedLinesStack.Enqueue($"({FALSElabel})");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M-1");
            convertedLinesStack.Enqueue("M=0");
            convertedLinesStack.Enqueue($"({CONTUINUElabel})");
        }

        private void LogCommandGt()
        {
            // Stack Operation
            short y = this.PopFromStack();
            short x = this.PopFromStack();

            short result = this.BoolToShort(x > y);

            this.globalStack.Push(result);

            // ASM:
            //*
            //@FALSE6				// gt
            //D;JLE
            //@SP
            //A=M-1
            //M=-1
            //@CONTINUE6
            //0;JMP
            //(FALSE6)
            //@SP
            //A=M-1
            //M=0
            //(CONTINUE6)
            //*
            this.PointerGoBack();

            string FALSElabel = this.GetUniqueLabel("FALSE");
            convertedLinesStack.Enqueue("@" + FALSElabel);
            convertedLinesStack.Enqueue("D;JLE");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M-1");
            convertedLinesStack.Enqueue("M=-1");
            string CONTUINUElabel = this.GetUniqueLabel("CONTINUE");
            convertedLinesStack.Enqueue("@" + CONTUINUElabel);
            convertedLinesStack.Enqueue("0;JMP");
            convertedLinesStack.Enqueue($"({FALSElabel})");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M-1");
            convertedLinesStack.Enqueue("M=0");
            convertedLinesStack.Enqueue($"({CONTUINUElabel})");

        }

        private void LogCommandAnd()
        {
            // Stack Operation
            short y = this.PopFromStack();
            short x = this.PopFromStack();

            int result = x & y;

            this.globalStack.Push((short)result);

            // ASM:
            //*
            //@SP					// and
            //AM=M-1
            //D=M
            //A=A-1
            //M=M&D
            //*

            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("A=A-1");
            convertedLinesStack.Enqueue("M=M&D");
        }

        private void LogCommandOr()
        {
            // Stack Operation
            short y = this.PopFromStack();
            short x = this.PopFromStack();

            int result = x | y;

            this.globalStack.Push((short)result);

            // ASM:
            //*
            //@SP					// or
            //AM=M-1
            //D=M
            //A=A-1
            //M=M|D
            //*

            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("A=A-1");
            convertedLinesStack.Enqueue("M=M|D");
        }

        private void LogCommandNot()
        {
            // Stack Operation
            short y = this.PopFromStack();

            int result = ~y;

            this.globalStack.Push((short)result);

            // ASM:
            //*
            //@SP					// not
            //A=M-1
            //M=!M
            //*

            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M-1");
            convertedLinesStack.Enqueue("M=!M");
        }

        private void LogCommandNeg()
        {
            // Stack Operation
            short y = this.PopFromStack();

            int result = y * -1;

            this.globalStack.Push((short)result);

            // ASM:
            //*
            //@SP					// neg
            //A=M-1
            //M=D-M
            //*

            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M-1");
            convertedLinesStack.Enqueue("M=D-M");
        }

        private void LogCommandSub()
        {
            // Stack Operation
            short y = this.PopFromStack();
            short x = this.PopFromStack();

            int result = x - y;

            this.globalStack.Push((short)result);


            // ASM:
            //*
            //@SP					// sub
            //AM=M-1
            //D=M
            //A=A-1
            //M=M-D
            //D=0
            //*

            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("A=A-1");
            convertedLinesStack.Enqueue("M=M-D");
            convertedLinesStack.Enqueue("D=0");
        }

        /// <summary>
        /// Adds pop value to thisPointer.
        /// </summary>
        private void PopPointer0()
        {
            // Stack operation
            this.thisPointer = this.PopFromStack();

            // Print ASM
            //'
            //@THIS		// pop pointer 0
            //D=A
            //@R13
            //M=D
            //@SP
            //AM=M-1
            //D=M
            //@R13
            //A=M
            //M=D
            //*

            convertedLinesStack.Enqueue("@THIS");
            convertedLinesStack.Enqueue("D=A");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");

        }

        /// <summary>
        /// Adds pop value to thatPointer.
        /// </summary>
        private void PopPointer1()
        {
            // Stack operation
            this.thatPointer = this.PopFromStack();

            // Print ASM
            //'
            //@THAT		// pop pointer 1
            //D=A
            //@R13
            //M=D
            //@SP
            //AM=M-1
            //D=M
            //@R13
            //A=M
            //M=D
            //*

            convertedLinesStack.Enqueue("@THAT");
            convertedLinesStack.Enqueue("D=A");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
        }

        private void PopThis(short memoryLocation)
        {
            // Stack operation
            this.thisMemory[memoryLocation] = this.PopFromStack();

            // Print ASM
            //'
            //@THIS		// pop this 2
            //D=M
            //@2
            //D=D+A
            //@R13
            //M=D
            //@SP
            //AM=M-1
            //D=M
            //@R13
            //A=M
            //M=D
            //*

            convertedLinesStack.Enqueue("@THIS");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@" + memoryLocation);
            convertedLinesStack.Enqueue("D=D+A");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");

        }

        private void PushThis(short memoryLocation)
        {
            // Stack operation
            this.globalStack.Push(this.thisMemory[memoryLocation]);

            // Print ASM
            //'
            //@THIS		// push this 2
            //D=M
            //@2
            //A=D+A
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //*

            convertedLinesStack.Enqueue("@THIS");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@" + memoryLocation);
            convertedLinesStack.Enqueue("A=D+A");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("M=M+1");


        }

        private void PopThat(short memoryLocation)
        {
            // Stack operation
            this.thatMemory[memoryLocation] = this.PopFromStack();

            // Print ASM
            //'
            //@THAT		// pop that 6
            //D=M
            //@6
            //D=D+A
            //@R13
            //M=D
            //@SP
            //AM=M-1
            //D=M
            //@R13
            //A=M
            //M=D
            //*

            convertedLinesStack.Enqueue("@THAT");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@" + memoryLocation);
            convertedLinesStack.Enqueue("D=D+A");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");

        }

        private void PushThat(short memoryLocation)
        {
            // Stack operation
            this.globalStack.Push(this.thatMemory[memoryLocation]);

            // Print ASM
            //'
            //@THAT		// push that 6
            //D=M
            //@6			
            //A=D+A
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //*

            convertedLinesStack.Enqueue("@THAT");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@" + memoryLocation);
            convertedLinesStack.Enqueue("A=D+A");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("M=M+1");

        }

        private void PopLocal(short memoryLocation)
        {
            // Stack operation
            short popValue = this.PopFromStack();
            this.localMemory[memoryLocation] = popValue;

            // Print ASM
            //'
            //@LCL	// pop local 0
            //D=M
            //@0
            //D=D+A
            //@R13
            //M=D
            //@SP
            //AM=M-1
            //D=M
            //@R13
            //A=M
            //M=D
            //*

            convertedLinesStack.Enqueue("@LCL");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@" + memoryLocation);
            convertedLinesStack.Enqueue("D=D+A");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");


        }

        private void PushLocal(short memoryLocation)
        {
            // Stack operation
            this.globalStack.Push(this.localMemory[memoryLocation]);

            // Print ASM
            //'
            //@LCL	// push local 0
            //D=M
            //@0
            //A=D+A
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //*

            convertedLinesStack.Enqueue("@LCL");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@" + memoryLocation);
            convertedLinesStack.Enqueue("A=D+A");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("M=M+1");


        }

        private void PopArgument(short memoryLocation)
        {
            // Stack operation
            short popValue = this.PopFromStack();
            this.argumentMemory[memoryLocation] = popValue;

            // Print ASM
            //'
            //@ARG	// pop argument 2
            //D=M
            //@2
            //D=D+A
            //@R13
            //M=D
            //@SP
            //AM=M-1
            //D=M
            //@R13
            //A=M
            //M=D
            //*

            convertedLinesStack.Enqueue("@ARG");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@" + memoryLocation);
            convertedLinesStack.Enqueue("D=D+A");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
        }

        private void PushArgument(short memoryLocation)
        {
            // Stack operation
            this.globalStack.Push(this.argumentMemory[memoryLocation]);

            // Print ASM
            //'
            //@ARG	// push argument 1
            //D=M
            //@1
            //A=D+A
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //*

            convertedLinesStack.Enqueue("@ARG");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@" + memoryLocation);
            convertedLinesStack.Enqueue("A=D+A");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("M=M+1");



        }

        private void PopTemp(short memoryLocation)
        {

            int tempMemoryLocation = 5 + memoryLocation;

            if (tempMemoryLocation > this.maxTempMemory || tempMemoryLocation < 1)
                throw new Exception($"Temp memory location must be between {0} and {this.maxTempMemory}.");

            // Stack operation
            short popValue = this.PopFromStack();
            this.tempMemory[tempMemoryLocation] = popValue;

            // Print ASM
            //'
            //@R5		// pop temp 6
            //D=M
            //@11
            //D=D+A
            //@R13
            //M=D
            //@SP
            //AM=M-1
            //D=M
            //@R13
            //A=M
            //M=D
            //*


            convertedLinesStack.Enqueue("@R5");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@" + tempMemoryLocation);
            convertedLinesStack.Enqueue("D=D+A");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");


        }

        private void PushTemp(short memoryLocation)
        {
            int tempMemoryLocation = 5 + memoryLocation;


            if (tempMemoryLocation > this.maxTempMemory || tempMemoryLocation < 1)
                throw new Exception($"Temp memory location must be between {0} and {this.maxTempMemory}.");

            // Stack operation
            this.globalStack.Push(this.tempMemory[memoryLocation]);
            this.tempMemory[memoryLocation] = 0;

            // Print ASM
            //'
            //@R5		// push temp 6
            //D=M
            //@11
            //A=D+A
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //*

            convertedLinesStack.Enqueue("@R5");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@" + tempMemoryLocation);
            convertedLinesStack.Enqueue("A=D+A");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("M=M+1");

        }


        private void PopStatic(short memoryLocation)
        {

            int staticMemoryLocation = this.staticStartMemory + memoryLocation;

            // Stack operation
            short popValue = this.PopFromStack();
            this.staticMemory[staticMemoryLocation] = popValue;

            // Print ASM
            //'
            //@24			// pop static 8
            //D=A
            //@R13
            //M=D
            //@SP
            //AM=M-1
            //D=M
            //@R13
            //A=M
            //M=D
            //*

            convertedLinesStack.Enqueue("@" + staticMemoryLocation);
            convertedLinesStack.Enqueue("D=A");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("AM=M-1");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@R13");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");

        }

        private void PushStatic(short memoryLocation)
        {
            int staticMemoryLocation = this.staticStartMemory + memoryLocation;

            // Stack operation
            this.globalStack.Push(this.staticMemory[staticMemoryLocation]);
            this.staticMemory[staticMemoryLocation] = 0;

            // Print ASM
            //'
            //@17			// push static 1
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //*

            convertedLinesStack.Enqueue("@" + staticMemoryLocation);
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("M=M+1");

        }


        /// <summary>
        /// Adds pop value to thisPointer.
        /// </summary>
        private void PushPointer0()
        {
            // Stack operation
            this.globalStack.Push(this.thisPointer);

            // Print ASM
            //'
            //@THIS 		// push pointer 0
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //*

            convertedLinesStack.Enqueue("@THIS");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("M=M+1");
        }

        /// <summary>
        /// Adds pop value to thatPointer.
        /// </summary>
        private void PushPointer1()
        {
            // Stack operation
            this.globalStack.Push(this.thatPointer);

            // Print ASM
            //'
            //@THAT		// push pointer 1
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //*

            convertedLinesStack.Enqueue("@THAT");
            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("M=M+1");
        }






        private short PopFromStack()
        {
            try
            {
                short popValue = this.globalStack.Pop();
                return popValue;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private string GetUniqueLabel(string labelName)
        {
            string labelNameWithCounter = labelName + this.labelCounter;
            labelCounter = labelCounter + 1;

            return labelNameWithCounter;
        }

        /// <summary>
        /// Returns the stack value for the provided boolean.
        /// True == -1 False == 0
        /// </summary>
        /// <param name="boolToConvert">The boolean value to convert.</param>
        /// <returns>The stack value for the provided boolean.</returns>
        private short BoolToShort(bool boolToConvert)
        {
            if (boolToConvert)
                return -1;
            else
                return 0;
        }

        #region InputAndOutputFunctionality
        private List<string> GetStrippedLinesFromFile(FileInfo inputfile)
        {
            List<string> allLinesInFile = new List<string>();

            // Read the file and display it line by line. 
            using (StreamReader file = new StreamReader(inputfile.FullName))
            {
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    string outputLineToAdd = line;

                    // check if line is empty
                    if (line.Length < 1 || line == "" || line == string.Empty)
                    {
                        continue;
                    }

                    if (line.Contains("//"))
                    {
                        // Split line by comment.
                        string[] lineCommentArray = line.Split("//");

                        // Set which 
                        if (lineCommentArray[0].ToString().Trim().Length < 1
                            || lineCommentArray[0].ToString().Trim() == "/")
                        {
                            continue;
                        }
                        else
                        {
                            outputLineToAdd = lineCommentArray[0].ToString();
                        }
                    }

                    allLinesInFile.Add(outputLineToAdd);
                }
            }

            return allLinesInFile;
        }

        public void WriteLinesToOutputFile(string[] outputLines, string path)
        {
            try
            {
                // Write to output file.
                using (StreamWriter fileWriter = new StreamWriter(Path.Combine(this.outputDirectory.FullName, Path.GetFileNameWithoutExtension(path) + ".asm")))
                {
                    foreach (string outputLine in outputLines)
                    {
                        fileWriter.WriteLine(outputLine);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while writing to Output file.", e);
            }
        }

        #endregion InputAndOutputFunctionality

        #region Setup

        /// <summary>
        /// Returns a list of available files in the input folder.
        /// </summary>
        /// <returns>string[] of files to assemble.</returns>
        public FileInfo[] GetAvailableInputFiles()
        {
            // Get input ASM files to be translated.
            FileInfo[] inputASMfiles = this.inputDirectory.GetFiles(ALLOWED_INPUT_FILES);
            List<FileInfo> listOfAvailableFiles = new List<FileInfo>();

            foreach (var file in inputASMfiles)
            {
                listOfAvailableFiles.Add(file);
            }

            return listOfAvailableFiles.ToArray();
        }

        /// <summary>
        /// Creates the start directories.
        /// </summary>
        private void CreateStartupDirectories(string inputDirectory, string outputDirectory)
        {
            try
            {
                // Create file folders..
                this.inputDirectory = Directory.CreateDirectory(inputDirectory);
                this.outputDirectory = Directory.CreateDirectory(outputDirectory);
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while creating input and output directory", e);
            }

        }

        #endregion Setup


    }
}
