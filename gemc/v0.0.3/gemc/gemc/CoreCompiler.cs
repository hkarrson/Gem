using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gemc
{
    public static class CoreCompiler
    {
        public static string CompileFile(string GemPath, string CSC)
        {
            ConsoleColor cf = Console.ForegroundColor;
            ConsoleColor cb = Console.BackgroundColor;
            string src = File.ReadAllText(GemPath);
            string cs = "";
            if (src.ToLower().StartsWith("filemode cs") && Regex.Replace(src, @"[\s\n\r]", "").ToLower().StartsWith("filemodecs{") &&
                Regex.Replace(src, @"[\s\n\r]", "").ToLower().EndsWith("}"))
            {
                cs = src.Substring(11, src.LastIndexOf("}") - 11);
                List<char> Lst = cs.ToCharArray().ToList();
                bool Add = false;
                cs = "";
                foreach (char c in Lst)
                {
                    if (Add)
                    {
                        cs += c.ToString();
                    }
                    if (c == '{')
                    {
                        Add = true;
                    }
                }
                return cs;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Syntax Error: File mode must be CS! @ " + GemPath);
                Console.ForegroundColor = cf;
                return null;
            }
        }

        internal static class PortableExecutableHelper
        {
            internal static bool IsDotNetAssembly(string peFile)
            {
                uint peHeader;
                uint peHeaderSignature;
                ushort machine;
                ushort sections;
                uint timestamp;
                uint pSymbolTable;
                uint noOfSymbol;
                ushort optionalHeaderSize;
                ushort characteristics;
                ushort dataDictionaryStart;
                uint[] dataDictionaryRVA = new uint[16];
                uint[] dataDictionarySize = new uint[16];


                Stream fs = new FileStream(peFile, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fs);

                //PE Header starts @ 0x3C (60). Its a 4 byte header.
                fs.Position = 0x3C;

                peHeader = reader.ReadUInt32();

                //Moving to PE Header start location...
                fs.Position = peHeader;
                peHeaderSignature = reader.ReadUInt32();

                //We can also show all these value, but we will be       
                //limiting to the CLI header test.

                machine = reader.ReadUInt16();
                sections = reader.ReadUInt16();
                timestamp = reader.ReadUInt32();
                pSymbolTable = reader.ReadUInt32();
                noOfSymbol = reader.ReadUInt32();
                optionalHeaderSize = reader.ReadUInt16();
                characteristics = reader.ReadUInt16();

                /*
                    Now we are at the end of the PE Header and from here, the
                                PE Optional Headers starts...
                        To go directly to the datadictionary, we'll increase the      
                        stream’s current position to with 96 (0x60). 96 because,
                                28 for Standard fields
                                68 for NT-specific fields
                    From here DataDictionary starts...and its of total 128 bytes. DataDictionay has 16 directories in total,
                    doing simple maths 128/16 = 8.
                    So each directory is of 8 bytes.
                                In this 8 bytes, 4 bytes is of RVA and 4 bytes of Size.

                    btw, the 15th directory consist of CLR header! if its 0, its not a CLR file :)
             */
                dataDictionaryStart = Convert.ToUInt16(Convert.ToUInt16(fs.Position) + 0x60);
                fs.Position = dataDictionaryStart;
                for (int i = 0; i < 15; i++)
                {
                    dataDictionaryRVA[i] = reader.ReadUInt32();
                    dataDictionarySize[i] = reader.ReadUInt32();
                }
                if (dataDictionaryRVA[14] == 0)
                {
                    fs.Close();
                    return false;
                }
                else
                {
                    fs.Close();
                    return true;
                }
            }
        }

        public static int Compile(string GemPath, string CSC)
        {
            List<string> args = new List<string>();
            string cs = "";
            GemPath = Path.GetFullPath(GemPath);
            foreach (string fn in Directory.EnumerateFiles(Path.GetDirectoryName(GemPath), "*.*", SearchOption.AllDirectories))
            {
                if (fn.ToLower().EndsWith(".dll"))
                {
                    if (PortableExecutableHelper.IsDotNetAssembly(fn))
                    {
                        args.Add("-reference:" + fn);
                    }
                }
                if (fn.ToLower().EndsWith(".gem"))
                {
                    string IL = CompileFile(fn, CSC);
                    if (IL != null)
                    {
                        cs += Environment.NewLine + Environment.NewLine + IL;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            string CSPath = Regex.Replace(GemPath, @"\.gem", ".cs", RegexOptions.IgnoreCase);
            File.WriteAllText(CSPath, cs);
            ProcessStartInfo ps = new ProcessStartInfo(CSC, CSPath + " " + string.Join(" ", args));
            ps.UseShellExecute = false;
            Process p = Process.Start(ps);
            p.WaitForExit();
            File.Delete(CSPath);
            return 0;
        }
    }
}
