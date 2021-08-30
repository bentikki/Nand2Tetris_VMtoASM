using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace UnitTest
{
    public class Tests
    {
        private const string ALLOWED_INPUT_FILES = "*.ASM";

        private string _outputPath;
        private string _comparePath;
        private DirectoryInfo OUTPUT_DIRECTORY;
        private DirectoryInfo COMPARE_DIRECTORY;

        [SetUp]
        public void Setup()
        {
            this._outputPath = @"D:\Nand2Tetris\VMtoASM\VMtoASM\VMtoASM\FileDir\OUTPUT_ASM";
            this._comparePath = @"D:\Nand2Tetris\VMtoASM\VMtoASM\VMtoASM\FileDir\OUTPUT_ASM\COMPARE_FINISHED";

            this.OUTPUT_DIRECTORY = Directory.CreateDirectory(_outputPath);
            this.COMPARE_DIRECTORY = Directory.CreateDirectory(_comparePath);
        }

        [Test]
        public void CompareNewToFinished()
        {
            // Arrange

            // Get outputted ASM files in outputdirectory,
            // and compare files in compare directory.
            FileInfo[] inputASMfiles = OUTPUT_DIRECTORY.GetFiles(ALLOWED_INPUT_FILES);
            FileInfo[] compareASMfiles = COMPARE_DIRECTORY.GetFiles(ALLOWED_INPUT_FILES);

            List<FileInfo> filesWithDiffrences = new List<FileInfo>();

            // Act
            foreach (FileInfo file in inputASMfiles)
            {
                string comparePathToSingleFile = _comparePath + "\\" + file.Name;

                if (File.Exists(comparePathToSingleFile))
                {
                    FileInfo compareFile = new FileInfo(comparePathToSingleFile);

                    if(file.Length != compareFile.Length)
                    {
                        filesWithDiffrences.Add(file);
                    }
                }
            }


            // Assert
            Assert.IsEmpty(filesWithDiffrences);
        }
    }
}