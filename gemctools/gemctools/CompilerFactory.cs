using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gemctools
{
    public class CompilerFactory
    {
        private static string GemPath;
        private static string CSC;

        public static string GetGemPath()
        {
            return GemPath;
        }

        public static string GetCSC()
        {
            return CSC;
        }

        public static void TryCreateNew(string Version)
        {
            List<string> Lst = Environment.GetCommandLineArgs().ToList();
            Lst.RemoveAt(0);
            string[] args = Lst.ToArray();
            bool Silent = false;
            if (args.Contains("!!!"))
            {
                List<string> Args = args.ToList();
                Args.RemoveAll(a => a == "!!!");
                args = Args.ToArray();
                Silent = true;
            }
            string MonoBin = null;
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            foreach (DictionaryEntry de in environmentVariables)
            {
                if (de.Key.ToString().ToLower() == "path")
                {
                    string str = de.Value.ToString().Split(';')[0];
                    if (str.ToLower().Contains("mono") && str.ToLower().Contains("bin"))
                    {
                        if (Directory.Exists(str))
                        {
                            MonoBin = str;
                        }
                    }
                }
            }
            ConsoleColor cf = Console.ForegroundColor;
            ConsoleColor cb = Console.BackgroundColor;
            if (!string.IsNullOrWhiteSpace(MonoBin))
            {
                if (!Silent)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine("Found Mono Bin @ " + MonoBin);
                }
                string CSC = Path.Combine(MonoBin, "csc.bat");
                if (File.Exists(CSC))
                {
                    if (!Silent)
                    {
                        Console.WriteLine("Found C# Compiler @ " + CSC);
                        Console.WriteLine();
                        Console.ForegroundColor = cf;
                        Console.BackgroundColor = cb;
                        Console.WriteLine("Mono> gemc " + string.Join(" ", args));
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine("Gem Compiler " + Version);
                        Console.WriteLine("-------------------");
                        Console.WriteLine();
                    }
                    if (args.Length != 1)
                    {
                        if (args.Length < 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Error.WriteLine("Error: Missing Gem source path @ arg1... syntax should be: gemc filename.gem");
                            Console.ForegroundColor = cf;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Error.WriteLine("Error: Too many source files given. " +
                                "\nYou only need to enter one " +
                                "\nand any other required files will be added automatically..." +
                                "\nAs a reminder, the syntax should be: gemc filename.gem");
                            Console.ForegroundColor = cf;
                        }
                    }
                    else                    {
                        if (args[0].ToLower().EndsWith(".gem"))
                        {
                            if (File.Exists(args[0]))
                            {
                                if (!Silent)
                                {
                                    Console.WriteLine("Attempting to transpile '" + args[0] + "' and it's dependencies... \nWill compile to binary afterwards...");
                                }
                                GemPath = args[0];
                                CompilerFactory.CSC = CSC;
                                Console.ForegroundColor = cf;
                                Console.BackgroundColor = cb;
                                return;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Error.WriteLine("Error: Gem source file '" + args[0] + "' does not exist!");
                                Console.ForegroundColor = cf;
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Error.WriteLine("Error: Expected *Gem* source path @ arg1... " +
                                "\nsyntax should be: gemc filename.gem - But, the filename given doesn't end with .gem");
                            Console.ForegroundColor = cf;
                        }
                    }
                    Console.ForegroundColor = cf;
                    Console.BackgroundColor = cb;
                }
                else
                {
                    Console.BackgroundColor = cb;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Error: Could not find C# Compiler @ " + CSC);
                    Console.ForegroundColor = cf;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Error: You must run this tool from the Mono command prompt!");
                Console.ForegroundColor = cf;
            }
            Console.ForegroundColor = cf;
            Console.BackgroundColor = cb;
            Environment.Exit(-1);
        }
    }
}
