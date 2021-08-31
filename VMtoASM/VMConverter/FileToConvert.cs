using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMtoASM.VMConverter
{
    class FileToConvert
    {
        public string Name { get; set; }
        public List<string> Lines { get; set; }

        public FileToConvert(string name, List<string> lines)
        {
            Name = name;
            Lines = lines;
        }
    }
}
