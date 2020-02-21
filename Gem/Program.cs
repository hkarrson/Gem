using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using static Gem.AST;

namespace Gem
{
    class Program
    {
        static void Main(string[] args)
        {
            RunLexerAndGetSyntaxTree(args);
            Console.ReadKey();
        }
    }
}
