using System;
using System.Diagnostics;
using System.IO;

namespace GemBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Gem Builder v1.0.0");
            Process process = Process.Start(Path.Combine(Directory.GetParent(Directory.GetParent
                (Directory.GetParent(Directory.GetCurrentDirectory()).FullName)
                .FullName).FullName, "Build.bat"));
            process.WaitForExit();
            Console.WriteLine("\nBuild Completed... Launching Application\n");
            process = Process.Start(Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent
                (Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName).FullName, "App", "Run.bat"));
            process.WaitForExit();
            Console.ReadKey();
        }
    }
}
