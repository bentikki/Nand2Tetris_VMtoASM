using System;
using VMtoASM.VMConverter;

namespace VMtoASM
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup directories to be used for input and output files.
            string inputDirectory = @"D:\Nand2Tetris\VMtoASM\VMtoASM\VMtoASM\FileDir\INPUT_VM";
            string outputDirectory = @"D:\Nand2Tetris\VMtoASM\VMtoASM\VMtoASM\FileDir\OUTPUT_ASM";

            // Initialize assembler object.
            VMtoASMConverter vmToAsmConverter = new VMtoASMConverter(inputDirectory, outputDirectory);

            // List the available files in input directory.
            Console.WriteLine("Available files in input directory:");
            foreach (var inputFile in vmToAsmConverter.AvailableInputFiles)
            {
                Console.WriteLine(inputFile.Name);
            }
            Console.WriteLine();

            try
            {
                vmToAsmConverter.ConvertAllInputFiles();
            }
            catch (Exception e)
            {
                Console.WriteLine("An Error occured...");
                Console.WriteLine(nameof(e));
                Console.WriteLine(e.Message);

            }
        }
    }
}
