using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Gem.Lexer;

namespace Gem
{
    public static class AST
    {
        public static void RunLexerAndGetSyntaxTree(string[] args)
        {
            string[] Src = File.ReadAllLines(args[0]);
            Src = Src.Select(ln => Regex.Replace(ln, @"\t", "    ")).ToArray();
            Console.WriteLine("...\n");
            List<string> Starts = new List<string>();
            List<string> Bodies = new List<string>();
            bool InBody = false;
            foreach (string Line in Src)
            {
                List<LexingUtil.Token> Tokens = Lex(Line);
                foreach (LexingUtil.Token Token in Tokens)
                {
                    if ((Token.Name == "GLOBAL" || Token.Name == "HIDDEN") && !Tokens.Any(t => t.Name == "SEMI"))
                    {
                        Starts.Add(Line);
                        InBody = true;
                    }
                    else if (Token.Name == "END")
                    {
                        string Start = Starts.Last();
                        Starts.RemoveAt(Starts.Count - 1);
                        bool Read = false;
                        string Body = "";
                        foreach (string Line2 in Src)
                        {
                            if (Line2 == Start)
                            {
                                Read = true;
                            }
                            if (Line2 == Line)
                            {
                                Read = false;
                            }
                            if (Read)
                            {
                                Body += Line2 + Environment.NewLine;
                            }
                            if (Line2 == Line && Body.Length > 0)
                            {
                                Body += Line2;
                                break;
                            }
                            int EndCount = Lex(Body).Where(t => t.Name == "END").ToList().Count;
                            if (EndCount > 1)
                            {
                                foreach (string Body1 in Bodies)
                                {
                                    Body = Body.Replace(Body1, "token " + Lex(Body1.Trim().Split(new char[] { '\n', '\r' },
                                        StringSplitOptions.RemoveEmptyEntries)[0])[1].Value);
                                }
                            }
                        }
                        if (Body.Length > 0)
                        {
                            Bodies.Add(Regex.Replace(Body.Trim(), @"\r\n?|\n", Environment.NewLine));
                            Bodies = Bodies.Distinct().ToList();
                        }
                        int EndCount1 = Lex(Body).Where(t => t.Name == "END").ToList().Count;
                        if (EndCount1 > 1)
                        {
                            foreach (string Body1 in Bodies.ToList())
                            {
                                Body = Body.Replace(Body1, "token " + Lex(Body1.Trim().Split(new char[] { '\n', '\r' },
                                    StringSplitOptions.RemoveEmptyEntries)[0])[1].Value);
                                Bodies.Add(Body);
                                Bodies = Bodies.Distinct().ToList();
                            }
                            Bodies = Bodies.Distinct().ToList();
                            Bodies = Bodies.Where(b => Lex(b).Where(t => t.Name == "END").ToList().Count == 1).ToList();
                            foreach (string Body2 in Bodies.ToList())
                            {
                                List<string> Lines = Body2.Split(new char[] { '\n', '\r' },
                                    StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                                Lines.RemoveAll(l => l.Length <= 0);
                                foreach (string ln in Lines)
                                {
                                    if (!ln.StartsWith(@"//"))
                                    {
                                        foreach (LexingUtil.Token t in Lex(ln))
                                        {
                                            t.Print();
                                        }
                                        Console.WriteLine("");
                                    }
                                }
                                Console.WriteLine("\n...\n");
                            }
                        }
                        InBody = false;
                    }
                    else if (Lex(Line).Any(t => t.Name == "SEMI") && !InBody)
                    {
                        foreach (LexingUtil.Token t in Lex(Line.Trim()))
                        {
                            t.Print();
                        }
                        Console.WriteLine("");
                        Console.WriteLine("\n...\n");
                        break;
                    }
                }
            }
        }
    }
}
