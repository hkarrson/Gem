using Jint;
using sly.lexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GemCore
{
    public static class Core
    {
        static Engine engine = null;

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

        private static void NewEngine(FileInfo fileInfo)
        {
            engine = new Engine();
            engine.Global.FastAddProperty("Reloaded", new Jint.Native.JsValue(false), true, true, true);
            GetTypeDelegate getType = Type.GetType;
            engine = engine.SetValue("DotNetType", getType);
            GetAssemblyDelegate getAssembly = Assembly.LoadFrom;
            engine = engine.SetValue("DotNetAsm", getAssembly);
            engine = engine.SetValue("reload", new Action(() => { Reload(fileInfo); }));
        }

        public static void ExecFile(FileInfo fileInfo, bool Reload = false)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(fileInfo.FullName));
            Console.Title = fileInfo.FullName;
            List<Token<Lexer.LexerToken>> Tokens = Lexer.Lex(File.ReadAllText(fileInfo.FullName) + Environment.NewLine);
            if (!Reload) NewEngine(fileInfo);
            string JS = "";
            List<string> MethodNames = new List<string>();
            bool SkipNames = false;
            if (Tokens != null)
            {
                List<Token<Lexer.LexerToken>> Line = new List<Token<Lexer.LexerToken>>();
                foreach (Token<Lexer.LexerToken> EOL in Tokens)
                {
                    if (EOL.TokenID != Lexer.LexerToken.EOL)
                    {
                        Line.Add(EOL);
                    }
                    else
                    {
                        int i = 0;
                        foreach (Token<Lexer.LexerToken> Token in Line)
                        {
                            if (Token.TokenID == Lexer.LexerToken.LPAREN || Token.TokenID == Lexer.LexerToken.EQUALS)
                            {
                                SkipNames = false;
                            }
                            if (Token.TokenID == Lexer.LexerToken.PUBLIC || Token.TokenID == Lexer.LexerToken.HIDDEN)
                            {
                                if (Line.Count > i + 1)
                                {
                                    if (Line[i + 1].TokenID == Lexer.LexerToken.NAME)
                                    {
                                        if ((Line.Count > i + 2 && Line[i + 2].TokenID == Lexer.LexerToken.LPAREN) || i + 2 >= Line.Count)
                                        {
                                            if (Line.Count > i + 2 && Line[i + 2].TokenID == Lexer.LexerToken.LPAREN)
                                            {
                                                MethodNames.Add(Line[i + 1].Value + "(");
                                            }
                                            else
                                            {
                                                MethodNames.Add(Line[i + 1].Value + "{");
                                            }
                                            JS += "this." + MethodNames.Last().Substring(0, MethodNames.Last().Length - 1) + " = function";
                                            if (i + 2 >= Line.Count)
                                            {
                                                JS += "()";
                                            }
                                            SkipNames = true;
                                        }
                                        else
                                        {
                                            JS += "this.__" + Line[i + 1].Value + "__";
                                            SkipNames = true;
                                        }
                                    }
                                }
                            }
                            else if (!SkipNames && Token.TokenID == Lexer.LexerToken.NAME)
                            {
                                if (i + 1 < Line.Count && Line[i + 1].TokenID != Lexer.LexerToken.LPAREN && Token.TokenID == Lexer.LexerToken.NAME)
                                {
                                    JS += "__" + Token.Value + "__" + " ";
                                }
                                else
                                {
                                    JS += Token.Value + " ";
                                }
                            }
                            else if (Token.TokenID != Lexer.LexerToken.NAME)
                            {
                                JS += Token.Value + " ";
                                if (Token.TokenID == Lexer.LexerToken.LBRACE)
                                {
                                    SkipNames = false;
                                }
                                else if (Token.TokenID == Lexer.LexerToken.RBRACE)
                                {
                                    if (MethodNames.Any())
                                    {
                                        JS += Environment.NewLine;
                                        string Name = MethodNames.Last().Substring(0, MethodNames.Last().Length - 1);
                                        if (MethodNames.Last().EndsWith("{"))
                                        {
                                            JS += "this.__" + Name + "__ = new this." + Name + "();";
                                        }
                                        MethodNames.RemoveAt(MethodNames.Count - 1);
                                    }
                                }
                            }
                            i++;
                        }
                        Line.Clear();
                    }
                    JS += Environment.NewLine;
                }
            }
            //Console.WriteLine(JS);
            engine.Execute(JS);
            Console.ReadKey();
        }
    }
}
