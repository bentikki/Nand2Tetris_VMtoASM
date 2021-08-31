using System;
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


        // This no longer gives a clear image of the stack functionality
        // because loops and function support has not been implemented in the simulation.
        // I found out that there was no need to keep track of stack values...
        //
        // Simulation of stack functions.
        private Stack<short> globalStack;   // Global stack sim
        private short thisPointer;          // THIS pointer sim
        private short[] thisMemory;         // THIS array to contain memory

        private short thatPointer;          // THAT pointer sim
        private short[] thatMemory;         // THAT array to contain memory

        private short[] localMemory;            // LOCAL array to contain memory
        private short[] argumentMemory;         // ARG array to contain memory
        private short[] tempMemory;             // TEMP array to contain memory
        private short maxTempMemory = 12;       // TEMP max memory location
        private short tempStartMemory = 5;      // TEMP start location
        private short tempAvailableMemory = 8;  // TEMP memory locations available

        private short[] staticMemory;           // STATIC array to contain memory
        private short staticStartMemory = 16;   // STATIC start location

        private string currentFunction;
        private int currentFunctionStaticCounter;

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
            this.tempMemory = new short[tempAvailableMemory];
            this.staticMemory = new short[THIS_THAT_SIZE];
        }

        /// <summary>
        /// Loops through all input files and converts .VM to .ASM
        /// </summary>
        public void ConvertAllInputFiles()
        {
            foreach (FileToConvert inputVMFile in this.GetFilesToConvert())
            {
                this.ConvertSingle(inputVMFile);
            }
        }

        /// <summary>
        /// Converts provided VM file into .ASM.
        /// Places this file in output directory.
        /// </summary>
        /// <param name="inputVMFile">VM file to convert.</param>
        public void ConvertSingle(FileToConvert inputVMFile)
        {
            // Get the lines from input file - with comments and spaces stripped.
            List<string> strippedLinesFromFiles = this.GetStrippedLinesFromFile(inputVMFile);

            // Convert the provided VM lines to ASM code.
            string[] convertedLines = this.ConvertLinesToAsm(strippedLinesFromFiles);

            // Write lines to output .asm file,
            this.WriteLinesToOutputFile(convertedLines, inputVMFile.Name);
        }

        private string[] ConvertLinesToAsm(List<string> strippedLinesFromFiles)
        {
            this.convertedLinesStack = new Queue<string>();
            this.globalStack = new Stack<short>();

            Dictionary<string, ulong> labelLines = new Dictionary<string, ulong>();

            bool repeatLines = false;
            ulong startLine = 0;
            ulong endLine =  (ulong)strippedLinesFromFiles.Count();

            // Reset label counter and add Bootstrap Lines
            this.labelCounter = 0;
            this.currentFunction = string.Empty;
            this.currentFunctionStaticCounter = 0;
            this.AddBootstrapLines();

            do
            {
                repeatLines = false;

                for (ulong lineNumber = startLine; lineNumber < endLine; lineNumber++)
                {
                    // Line to be added to output.
                    StringBuilder convertedLineSB = new StringBuilder();
                    int beginningLinesCount = this.convertedLinesStack.Count;

                    // Convert line to lower, to make it case insensitive.
                    string line = strippedLinesFromFiles[(int)lineNumber].ToLower().Trim();

                    this.convertedLinesStack.Enqueue("");
                    this.convertedLinesStack.Enqueue($"// {lineNumber}: {line}");
                    this.convertedLinesStack.Enqueue("");

                    // Split the line to seperate command, argument and value parts.
                    string[] pointerArgumentArray = line.Split(" ");
                    string lineCommandPart = pointerArgumentArray[0];

                    // If the line first part is push or pop, we know its not a logical command (add/sub/neg etc.)
                    if (lineCommandPart == "push" || lineCommandPart == "pop")
                    {

                        if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                        {
                            // Check if the line is Push
                            if (lineCommandPart == "push")
                            {
                                this.PushSelection(pointerArgumentArray[1], constantNum);
                            }
                            else if (lineCommandPart == "pop") // Check if POP
                            {
                                this.PopSelection(pointerArgumentArray[1], constantNum);
                            }
                        }
                        else
                        {
                            throw new ArgumentException($"A {lineCommandPart} command was called with no value. Line: {line}", pointerArgumentArray[2]);
                        }

                    }
                    else if (lineCommandPart == "label") // Check if the command is a label
                    {
                        labelLines.Add(pointerArgumentArray[1], lineNumber + 1);
                        this.AddLabel(pointerArgumentArray[1]);
                    }
                    else if (lineCommandPart == "if-goto")   // Check if the comand is a if-goto
                    {
                        this.AddIfGoto(pointerArgumentArray[1]);
                    }
                    else if (lineCommandPart == "goto")   // Check if the comand is a if-goto
                    {
                        this.AddGoto(pointerArgumentArray[1]);

                        //startLine = labelLines[pointerArgumentArray[1]];
                        //repeatLines = true;
                        //break;
                    }
                    else if (lineCommandPart == "function")
                    {
                        this.currentFunction = pointerArgumentArray[1];
                        this.currentFunctionStaticCounter = 0;

                        if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                        {
                            this.AddFunctionInit(pointerArgumentArray[1], constantNum);
                        }
                    }
                    else if (lineCommandPart == "call")
                    {

                        if (Int16.TryParse(pointerArgumentArray[2], out short constantNum))
                        {
                            this.AddFunctionCall(pointerArgumentArray[1], constantNum);
                        }
                    }
                    else if(lineCommandPart == "return")
                    {
                        this.AddReturn();
                    }
                    else // Else it must be a logical command.
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



            } while (repeatLines);
            
           

            return convertedLinesStack.ToArray();
        }

        private void AddBootstrapLines()
        {
            // ASM
            //@256
            //D=A
            //@SP
            //M=D
            this.convertedLinesStack.Enqueue("@256");
            this.convertedLinesStack.Enqueue("D=A");
            this.convertedLinesStack.Enqueue("@SP");
            this.convertedLinesStack.Enqueue("M=D");
        }

        private void PushSelection(string type, short value)
        {
            // Switch on the type on pop command.
            switch (type)
            {
                case "constant":
                    this.AddConstant(value);

                    // Advance the stack pointer.
                    this.PointerAdvance();
                    break;

                case "pointer":
                    if (value == 0)
                    {
                        this.PushPointer0();
                    }
                    else if (value == 1)
                    {
                        this.PushPointer1();
                    }
                    break;

                case "this":
                    this.PushThis(value);
                    break;

                case "that":
                    this.PushThat(value);
                    break;

                case "local":
                    this.PushLocal(value);
                    break;

                case "argument":
                    this.PushArgument(value);
                    break;

                case "temp":
                    this.PushTemp(value);
                    break;

                case "static":
                    this.PushStatic(value);
                    break;

                default:
                    throw new Exception("An unknown command was sent to PUSH. The command sent was:" + type);
            }

        }

        private void PopSelection(string type, short value)
        {
            // Switch on the type on pop command.
            switch (type)
            {
                case "pointer":
                    if (value == 0)
                    {
                        this.PopPointer0();
                    }
                    else if (value == 1)
                    {
                        this.PopPointer1();
                    }
                    break;

                case "this":
                    this.PopThis(value);
                    break;

                case "that":
                    this.PopThat(value);
                    break;

                case "local":
                    this.PopLocal(value);
                    break;

                case "argument":
                    this.PopArgument(value);
                    break;

                case "temp":
                    this.PopTemp(value);
                    break;

                case "static":
                    this.PopStatic(value);
                    break;

                default:
                    throw new Exception("An unknown command was sent to POP. The command sent was:" + type);
            }

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

        private void AddLabel(string labelName)
        {
            // Setup
            string labelNametoAdd = labelName;

            // Stack operation



            // ASM operation
            this.convertedLinesStack.Enqueue("(" + labelNametoAdd + ")");
        }

        private void AddIfGoto(string labelName)
        {
            // Setup
            string labelNametoAdd = labelName;

            // Stack operation

            // ASM operation
            //@SP				// if-goto LOOP_START
            //AM=M-1
            //D=M
            //A=A-1
            //@LOOP_START
            //D;JNE

            this.convertedLinesStack.Enqueue("@SP");
            this.convertedLinesStack.Enqueue("AM=M-1");
            this.convertedLinesStack.Enqueue("D=M");
            this.convertedLinesStack.Enqueue("A=A-1");
            this.convertedLinesStack.Enqueue("@" + labelNametoAdd);
            this.convertedLinesStack.Enqueue("D;JNE");
        }

        private void AddGoto(string labelName)
        {
            // Setup
            string labelNametoAdd = labelName;

            // Stack operation

            // ASM operation
            //@END_PROGRAM		// goto END_PROGRAM
            //0;JMP

            this.convertedLinesStack.Enqueue("@" + labelNametoAdd);
            this.convertedLinesStack.Enqueue("0;JMP");
        }

        #region Functions

        private void AddFunctionCall(string functionName, short functionArguments)
        {
            // Add ASM
            //@RETURN_LABEL0		// sys:11:function Sys.init 0  // start label
            //D=A
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //@LCL
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //@ARG
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //@THIS
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //@THAT
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1
            //@SP
            //D=M
            //@5
            //D=D-A
            //@0				// sys:11:function Sys.init 0 // argument number
            //D=D-A
            //@ARG
            //M=D
            //@SP
            //D=M
            //@LCL
            //M=D
            //@Sys.init         // function name
            //0;JMP
            //(RETURN_LABEL0)   // start label

            string labelName = this.GetUniqueLabel(functionName +"_RETURN_LABEL");
            this.convertedLinesStack.Enqueue("@" + labelName);
            this.convertedLinesStack.Enqueue("D=A");
            this.convertedLinesStack.Enqueue("@SP");
            this.convertedLinesStack.Enqueue("A=M");
            this.convertedLinesStack.Enqueue("M=D");
            this.convertedLinesStack.Enqueue("@SP");
            this.convertedLinesStack.Enqueue("M=M+1");//M=M+1
            this.convertedLinesStack.Enqueue("@LCL");//@LCL
            this.convertedLinesStack.Enqueue("D=M");//D=M
            this.convertedLinesStack.Enqueue("@SP");//@SP

            this.convertedLinesStack.Enqueue("A=M");//A=M
            this.convertedLinesStack.Enqueue("M=D");//M=D
            this.convertedLinesStack.Enqueue("@SP");//@SP
            this.convertedLinesStack.Enqueue("M=M+1");//M=M+1
            this.convertedLinesStack.Enqueue("@ARG");//@ARG
            this.convertedLinesStack.Enqueue("D=M");//D=M
            this.convertedLinesStack.Enqueue("@SP"); //@SP
            this.convertedLinesStack.Enqueue("A=M");//A=M
            this.convertedLinesStack.Enqueue("M=D");//M=D
            this.convertedLinesStack.Enqueue("@SP");//@SP
            this.convertedLinesStack.Enqueue("M=M+1");//M=M+1
            this.convertedLinesStack.Enqueue("@THIS");//@THIS
            this.convertedLinesStack.Enqueue("D=M");//D=M
            this.convertedLinesStack.Enqueue("@SP");//@SP
            this.convertedLinesStack.Enqueue("A=M");//A=M
            this.convertedLinesStack.Enqueue("M=D");//M=D
            this.convertedLinesStack.Enqueue("@SP");//@SP
            this.convertedLinesStack.Enqueue("M=M+1");//M=M+1
            this.convertedLinesStack.Enqueue("@THAT");//@THAT
            this.convertedLinesStack.Enqueue("D=M");//D=M
            this.convertedLinesStack.Enqueue("@SP");//@SP
            this.convertedLinesStack.Enqueue("A=M");//A=M
            this.convertedLinesStack.Enqueue("M=D");//M=D
            this.convertedLinesStack.Enqueue("@SP");//@SP
            this.convertedLinesStack.Enqueue("M=M+1");//M=M+1
            this.convertedLinesStack.Enqueue("@SP");//@SP
            this.convertedLinesStack.Enqueue("D=M");//D=M
            this.convertedLinesStack.Enqueue("@5");//@5
            this.convertedLinesStack.Enqueue("D=D-A");//D=D-A
            this.convertedLinesStack.Enqueue("@" + functionArguments);//@0				// sys:11:function Sys.init 0 // argument number
            this.convertedLinesStack.Enqueue("D=D-A"); //D=D-A
            this.convertedLinesStack.Enqueue("@ARG");//@ARG
            this.convertedLinesStack.Enqueue("M=D");//M=D
            this.convertedLinesStack.Enqueue("@SP");//@SP
            this.convertedLinesStack.Enqueue("D=M");//D=M
            this.convertedLinesStack.Enqueue("@LCL");//@LCL
            this.convertedLinesStack.Enqueue("M=D");//M=D
            this.convertedLinesStack.Enqueue("@" + functionName); //@Sys.init         // function name
            this.convertedLinesStack.Enqueue("0;JMP");//0;JMP
            this.convertedLinesStack.Enqueue("(" + labelName + ")");//(RETURN_LABEL0)   // start label

        }

        private void AddFunctionInit(string functionName, short functionArguments)
        {
            if(functionName == "sys.init")
            {
                this.AddFunctionCall(functionName, functionArguments);
            }

            this.convertedLinesStack.Enqueue("(" + functionName + ")");
        }

        private void AddReturn()
        {
            // ASM
            //@LCL				// main:19:return
            //D=M
            //@FRAME
            //M=D
            //@5
            //A=D-A
            //D=M
            //@RET
            //M=D
            //@ARG
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
            //@ARG
            //D=M
            //@SP
            //M=D+1
            //@FRAME
            //D=M-1
            //AM=D
            //D=M
            //@THAT
            //M=D
            //@FRAME
            //D=M-1
            //AM=D
            //D=M
            //@THIS
            //M=D
            //@FRAME
            //D=M-1
            //AM=D
            //D=M
            //@ARG
            //M=D
            //@FRAME
            //D=M-1
            //AM=D
            //D=M
            //@LCL
            //M=D
            //@RET
            //A=M
            //0;JMP



            convertedLinesStack.Enqueue("@LCL");//@LCL				// main:19:return
            convertedLinesStack.Enqueue("D=M");//D=M
            convertedLinesStack.Enqueue("@FRAME");//@FRAME
            convertedLinesStack.Enqueue("M=D");//M=D
            convertedLinesStack.Enqueue("@5");//@5
            convertedLinesStack.Enqueue("A=D-A");//A=D-A
            convertedLinesStack.Enqueue("D=M");//D=M
            convertedLinesStack.Enqueue("@RET");//@RET
            convertedLinesStack.Enqueue("M=D");//M=D
            convertedLinesStack.Enqueue("@ARG");//@ARG
            convertedLinesStack.Enqueue("D=M");//D=M
            convertedLinesStack.Enqueue("@0");//@0
            convertedLinesStack.Enqueue("D=D+A");//D=D+A
            convertedLinesStack.Enqueue("@R13");//@R13
            convertedLinesStack.Enqueue("M=D");//M=D
            convertedLinesStack.Enqueue("@SP");//@SP
            convertedLinesStack.Enqueue("AM=M-1");//AM=M-1
            convertedLinesStack.Enqueue("D=M");//D=M
            convertedLinesStack.Enqueue("@R13");//@R13
            convertedLinesStack.Enqueue("A=M");//A=M
            convertedLinesStack.Enqueue("M=D");//M=D
            convertedLinesStack.Enqueue("@ARG");//@ARG
            convertedLinesStack.Enqueue("D=M");//D=M
            convertedLinesStack.Enqueue("@SP");//@SP
            convertedLinesStack.Enqueue("M=D+1");//M=D+1
            convertedLinesStack.Enqueue("@FRAME");//@FRAME
            convertedLinesStack.Enqueue("D=M-1");//D=M-1
            convertedLinesStack.Enqueue("AM=D");//AM=D
            convertedLinesStack.Enqueue("D=M");//D=M
            convertedLinesStack.Enqueue("@THAT");//@THAT
            convertedLinesStack.Enqueue("M=D");//M=D
            convertedLinesStack.Enqueue("@FRAME");//@FRAME
            convertedLinesStack.Enqueue("D=M-1");//D=M-1
            convertedLinesStack.Enqueue("AM=D");//AM=D
            convertedLinesStack.Enqueue("D=M");//D=M
            convertedLinesStack.Enqueue("@THIS");//@THIS
            convertedLinesStack.Enqueue("M=D");//M=D
            convertedLinesStack.Enqueue("@FRAME");//@FRAME
            convertedLinesStack.Enqueue("D=M-1");//D=M-1
            convertedLinesStack.Enqueue("AM=D");//AM=D
            convertedLinesStack.Enqueue("D=M");//D=M
            convertedLinesStack.Enqueue("@ARG");//@ARG
            convertedLinesStack.Enqueue("M=D");//M=D
            convertedLinesStack.Enqueue("@FRAME");//@FRAME
            convertedLinesStack.Enqueue("D=M-1");//D=M-1
            convertedLinesStack.Enqueue("AM=D");//AM=D
            convertedLinesStack.Enqueue("D=M");//D=M
            convertedLinesStack.Enqueue("@LCL");//@LCL
            convertedLinesStack.Enqueue("M=D");//M=D
            convertedLinesStack.Enqueue("@RET");//@RET
            convertedLinesStack.Enqueue("A=M");//A=M
            convertedLinesStack.Enqueue("0;JMP");//0;JMP


        }

        #endregion Functions

        #region LogicalOps

        private void LogCommandAdd()
        {
            // Stack operation
            short y = this.PopFromStack();
            short x = this.PopFromStack();

            int result = x + y;

            this.globalStack.Push((short)result);

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

        #endregion LogicalOps

        #region PushAndPop

        private void AddConstant(short constantNum)
        {
            // Stack operation
            this.globalStack.Push(constantNum);

            // Add ASM
            convertedLinesStack.Enqueue("@" + constantNum);
            convertedLinesStack.Enqueue("D=A");
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

            // Updated from functions
            //@ARG		// push argument 0
            //D=M
            //@0
            //D=D+A
            //A=D
            //D=M
            //@SP
            //A=M
            //M=D
            //@SP
            //M=M+1


        }

        private void PopTemp(short memoryLocation)
        {

            int tempMemoryLocation = tempStartMemory + memoryLocation;

            if (tempMemoryLocation > this.maxTempMemory || tempMemoryLocation < 1)
                throw new Exception($"Temp memory location must be between {0} and {this.maxTempMemory}.");

            // Stack operation
            short popValue = this.PopFromStack();
            this.tempMemory[memoryLocation] = popValue;

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
            int tempMemoryLocation = tempStartMemory + memoryLocation;


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
            if (this.currentFunction != string.Empty)
            {
                string[] functionNameArr = this.currentFunction.Split(".");

                string staticMemoryName = $"{functionNameArr[0]}.static{this.currentFunctionStaticCounter}";
                convertedLinesStack.Enqueue("@" + staticMemoryName);
                this.currentFunctionStaticCounter++;
            }
            else
            {
                int staticMemoryLocation = this.staticStartMemory + memoryLocation;
                convertedLinesStack.Enqueue("@" + staticMemoryLocation);
            }

            // Stack operation
            // short popValue = this.PopFromStack();
            // this.staticMemory[staticMemoryLocation] = popValue;

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
            if (this.currentFunction != string.Empty)
            {
                string[] functionNameArr = this.currentFunction.Split(".");

                string staticMemoryName = $"{functionNameArr[0]}.static{this.currentFunctionStaticCounter}";
                convertedLinesStack.Enqueue($"@" + staticMemoryName);
                this.currentFunctionStaticCounter++;
            }
            else
            {
                int staticMemoryLocation = this.staticStartMemory + memoryLocation;
                convertedLinesStack.Enqueue("@" + staticMemoryLocation);
            }

            //int staticMemoryLocation = this.staticStartMemory + memoryLocation;

            // Stack operation
            //this.globalStack.Push(this.staticMemory[staticMemoryLocation]);
            //this.staticMemory[staticMemoryLocation] = 0;

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

            convertedLinesStack.Enqueue("D=M");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("A=M");
            convertedLinesStack.Enqueue("M=D");
            convertedLinesStack.Enqueue("@SP");
            convertedLinesStack.Enqueue("M=M+1");

        }


        #endregion PushAndPop

        #region HelperFunctions

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

        #endregion HelperFunctions

        #region InputAndOutputFunctionality
        private List<string> GetStrippedLinesFromFile(FileToConvert inputfile)
        {
            List<string> allLinesInFile = new List<string>();

            foreach (string line in inputfile.Lines)
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

        private List<FileToConvert> GetFilesToConvert()
        {
            // Dictionary to hold the name and content of all files to convert.
            List<FileToConvert> filesToConvertList = new List<FileToConvert>();

            FileInfo[] fileArray = this.GetAvailableInputFiles();

            foreach (var inputfile in fileArray)
            {
                List<string> allLinesInFile = this.StripLinesFromFile(inputfile).ToList();
                filesToConvertList.Add(new FileToConvert(inputfile.Name, allLinesInFile));
            }

            // Find directories.
            DirectoryInfo[] singleAsmDirectories = this.inputDirectory.GetDirectories();

            foreach (DirectoryInfo directory in singleAsmDirectories)
            {
                // Compine all files in a directory, give the new "file" the name of the directory.
                List<FileInfo> inputASMfiles = directory.GetFiles(ALLOWED_INPUT_FILES).ToList();
                List<FileInfo> inputASMfilesSorted = new List<FileInfo>();

                FileInfo sysFile = null;
                int sysfileLocation = 0;

                // Sort the input files in order to put sys.vm first.
                for (int i = 0; i < inputASMfiles.Count; i++)
                {
                    if(inputASMfiles[i].Name.ToLower() == "sys.vm")
                    {
                        sysFile = inputASMfiles[i];
                        sysfileLocation = i;
                    }
                }

                if(sysFile != null)
                {
                    inputASMfiles.RemoveAt(sysfileLocation);
                    inputASMfilesSorted.Add(sysFile);
                }
                inputASMfilesSorted.AddRange(inputASMfiles);


                List<string> allLinesInFile = new List<string>();

                foreach (var file in inputASMfilesSorted)
                {
                    allLinesInFile.AddRange(this.StripLinesFromFile(file));
                }

                filesToConvertList.Add(new FileToConvert(directory.Name, allLinesInFile));
            }

            return filesToConvertList;
        }

        private IEnumerable<string> StripLinesFromFile(FileInfo fileInfo)
        {
            List<string> allLinesInFile = new List<string>();

            // Read the file and display it line by line. 
            using (StreamReader file = new StreamReader(fileInfo.FullName))
            {
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    allLinesInFile.Add(line);
                }
            }

            return allLinesInFile;
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
