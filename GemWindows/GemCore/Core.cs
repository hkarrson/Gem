using Jint;
using sly.lexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GemCore
{
    public static class Core
    {
        static Engine engine = null;
        static string GemFileName = null;

        public static void ExecFile(string Path)
        {
            FileInfo fileInfo = null;
            try
            {
                fileInfo = new FileInfo(Path);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine
                    ("Invalid file path... Full exception below..." +
                    Environment.NewLine + ex.ToString());
                Console.ReadKey();
            }
            if (fileInfo != null) ExecFile(fileInfo);
        }

        private static void Reload(FileInfo fileInfo)
        {
            var session = engine.Global.GetOwnProperties();
            engine = null;
            NewEngine(fileInfo);
            foreach (var v in session)
            {
                engine.Global.FastSetProperty(v.Key, v.Value);
            }
            engine.Global.RemoveOwnProperty("Reloaded");
            engine.Global.FastAddProperty("Reloaded", new Jint.Native.JsValue(true), true, true, true);
            ExecFile(fileInfo, true);
        }

        delegate Type GetTypeDelegate(string TypeName);
        delegate Assembly GetAssemblyDelegate(string AssemblyPath);
        delegate dynamic ActivatorDelegate(Type type);
        delegate dynamic GemFileDelegate(string GemFile);

        private static object GemFile(string GemFile)
        {
            return engine.Global.Get("__" + GemFile.Replace(".", "_") + "__");
        }

        private static void NewEngine(FileInfo fileInfo)
        {
            engine = new Engine();
            engine.Global.FastAddProperty("Reloaded", new Jint.Native.JsValue(false), true, true, true);
            engine.Global.FastAddProperty("IsMain", new Jint.Native.JsValue(false), true, true, true);
            GetTypeDelegate getType = Type.GetType;
            engine = engine.SetValue("DotNetType", getType);
            GetAssemblyDelegate getAssembly = Assembly.LoadFrom;
            engine = engine.SetValue("DotNetAsm", getAssembly);
            ActivatorDelegate activatorDelegate = Activator.CreateInstance;
            engine = engine.SetValue("DotNetNew", activatorDelegate);
            GemFileDelegate gemFile = GemFile;
            engine = engine.SetValue("GemFile", gemFile);
            engine = engine.SetValue("reload", new Action(() => { Reload(fileInfo); }));
        }

        public static List<string> ParsePathEnvironmentVariable()
        {
            List<string> Lst = new List<string>();
            string originalPathEnv = Environment.GetEnvironmentVariable("PATH");
            string[] paths = originalPathEnv.Split(new char[1] { Path.PathSeparator });
            foreach (string s in paths)
            {
                string pathEnv = Environment.ExpandEnvironmentVariables(s);
                if (pathEnv.Length > 0)
                {
                    if (Directory.Exists(pathEnv))
                    {
                        foreach (string fn in Directory.GetFiles(pathEnv))
                        {
                            Lst.Add(fn);
                        }
                    }
                }
            }
            return Lst;
        }

        public static void ExecFile(FileInfo fileInfo, bool Reload = false)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(fileInfo.FullName));
            Console.Title = fileInfo.FullName;
            if (!Reload) NewEngine(fileInfo);
            List<string> Lst = new List<string>();
            Lst.AddRange(ParsePathEnvironmentVariable());
            Lst.Add(fileInfo.FullName);
            Lst = Lst.Where(fn => fn.ToLower().EndsWith(".gem")).ToList();
            foreach (string fn in Lst)
            {
                if (fn == Lst.Last())
                {
                    engine.Global.RemoveOwnProperty("IsMain");
                    engine.Global.FastAddProperty("IsMain", new Jint.Native.JsValue(true), true, true, true);
                }
                string src = "";
                int x = 0;
                foreach (string m in Regex.Split(File.ReadAllText(fn), @"^\\$", RegexOptions.Multiline))
                {
                    if (x % 2 == 0)
                    {
                        src += m + Environment.NewLine;
                    }
                    else
                    {
                        src += m.Replace("\n", " ").Replace("\r", " ") + Environment.NewLine;
                    }
                    x++;
                }
                List<Token<Lexer.LexerToken>> Tokens = Lexer.Lex(src);
                bool InGemFile = false;
                int i = 0;
                if (Tokens != null)
                {
                    List<Token<Lexer.LexerToken>> Line = new List<Token<Lexer.LexerToken>>();
                    foreach (Token<Lexer.LexerToken> EOL in Tokens)
                    {
                        if (EOL.TokenID != Lexer.LexerToken.EOL && !EOL.Value.Contains("\n") && !EOL.Value.Contains("\r"))
                        {
                            Line.Add(EOL);
                        }
                        else
                        {
                            string JS = "";
                            if (i == 0)
                            {
                                foreach (Token<Lexer.LexerToken> Token in Line)
                                {
                                    if (Token.TokenID == Lexer.LexerToken.JS)
                                    {
                                        GemFileName = Token.Value;
                                        GemFileName = "__" + GemFileName.Replace(".", "_") + "__";
                                        if (!Reload)
                                        {
                                            JS = GemFileName + " = {};" + Environment.NewLine;
                                        }
                                        engine.Execute(JS);
                                        JS = "";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Line[0].TokenID == Lexer.LexerToken.HIDDEN || Line[0].TokenID == Lexer.LexerToken.PUBLIC)
                                {
                                    JS += GemFileName + ".";
                                }
                                foreach (Token<Lexer.LexerToken> Token in Line)
                                {
                                    if (Token.TokenID == Lexer.LexerToken.THIS)
                                    {
                                        JS += @"GemFile(""" + GemFileName.Replace("_", " ").Trim().Replace(" ", ".") + @""").";
                                    }
                                    if (Token.TokenID == Lexer.LexerToken.GemFile)
                                    {
                                        InGemFile ^= true;
                                        if (InGemFile)
                                        {
                                            JS += @"GemFile(""";
                                        }
                                        else
                                        {
                                            JS = JS.Substring(0, JS.Length - 1) + @""")";
                                        }
                                    }
                                    else if (Token.TokenID == Lexer.LexerToken.JS)
                                    {
                                        JS += Token.Value + " ";
                                    }
                                    else if (Token.TokenID == Lexer.LexerToken.ARROW)
                                    {
                                        JS += "{ ";
                                    }
                                }
                                if (Line.Any(t => t.TokenID == Lexer.LexerToken.ARROW))
                                {
                                    JS += "}";
                                }
                                engine.Execute(JS);
                            }
                            Line.Clear();
                            i++;
                        }
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
