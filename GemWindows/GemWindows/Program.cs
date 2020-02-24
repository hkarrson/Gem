﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GemCore;

namespace GemWindows
{
    class Program
    {
        const string ExampleAppPath = "ExampleApp/Main.gem";

        static void Main(string[] args)
        {
            if (args.Any())
                Core.ExecFile(args[0]);
            else if (File.Exists(ExampleAppPath))
                Core.ExecFile(ExampleAppPath);
            else
            {
                Console.Error.WriteLine("No file specified.");
                Console.ReadKey();
            }
        }
    }
}