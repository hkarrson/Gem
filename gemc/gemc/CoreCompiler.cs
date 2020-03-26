using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gemc
{
    public static class CoreCompiler
    {
        public static void Compile(string GemPath, string ILasm)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("Failed to compile '" + GemPath + "' - Compiler not yet implemented.");
        }
    }
}
