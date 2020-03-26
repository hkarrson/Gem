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
        public delegate int CoreCompiler(string GemPath, string ILasm);

        public static void CreateNew(CoreCompiler Core, string Version)
        {
            int ret = -1;
            List<string> Lst = Environment.GetCommandLineArgs().ToList();
            Lst.RemoveAt(0);
            string[] args = Lst.ToArray();
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
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine("Found Mono Bin @ " + MonoBin);
                string ILasm = Path.Combine(MonoBin, "ILasm.bat");
                if (File.Exists(ILasm))
                {
                    Console.WriteLine("Found IL Assembler @ " + ILasm);
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
                    else
                    {
                        if (args[0].ToLower().EndsWith(".gem"))
                        {
                            if (File.Exists(args[0]))
                            {
                                Console.WriteLine("Attempting to compile '" + args[0] + "' and it's dependencies...");
                                ret = Core(args[0], ILasm);
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
                    Console.Error.WriteLine("Error: Could not find IL Assembler @ " + ILasm);
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
            Environment.Exit(ret);
        }
    }
}
