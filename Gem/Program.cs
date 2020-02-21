using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Gem
{
    class Program
    {
        static List<LexingUtil.Token> Lexer(string Line)
        {
            List<LexingUtil.Token> Tokens = new List<LexingUtil.Token>();
            LexingUtil.SetSource(Line);
            LexingUtil.Add("COMMENT", @"\/\/.*");
            LexingUtil.Add("STRING", @"\"".*\""");
            LexingUtil.IgnoreSpaces();
            LexingUtil.Add("GLOBAL", @"global");
            LexingUtil.Add("HIDDEN", @"hidden");
            LexingUtil.Add("END", @"end");
            LexingUtil.Add("ASSIGNMENT_EQUALS", @"\=");
            LexingUtil.Add("SEMI", @"\;");
            LexingUtil.Add("LPAREN", @"\(");
            LexingUtil.Add("RPAREN", @"\)");
            LexingUtil.Add("COMMA", @"\,");
            LexingUtil.Add("DOT", @"\.");
            LexingUtil.Add("NAME", @"[a-zA-z_][a-zA-Z0-9_]*");
            Tokens.AddRange(LexingUtil.Pop());
            return Tokens;
        }

        static void Main(string[] args)
        {
            string[] Src = File.ReadAllLines(args[0]);
            Src = Src.Select(ln => Regex.Replace(ln, @"\t", "    ")).ToArray();
            List<string> Starts = new List<string>();
            List<string> Bodies = new List<string>();
            bool InBody = false;
            foreach (string Line in Src)
            {
                List<LexingUtil.Token> Tokens = Lexer(Line);
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
                            int EndCount = Lexer(Body).Where(t => t.Name == "END").ToList().Count;
                            if (EndCount > 1)
                            {
                                foreach (string Body1 in Bodies)
                                {
                                    Body = Body.Replace(Body1, "token " + Lexer(Body1.Trim().Split(new char[] { '\n', '\r' }, 
                                        StringSplitOptions.RemoveEmptyEntries)[0])[1].Value);
                                }
                            }
                        }
                        if (Body.Length > 0)
                        {
                            Bodies.Add(Regex.Replace(Body.Trim(), @"\r\n?|\n", Environment.NewLine));
                            Bodies = Bodies.Distinct().ToList();
                        }
                        int EndCount1 = Lexer(Body).Where(t => t.Name == "END").ToList().Count;
                        if (EndCount1 > 1)
                        {
                            foreach (string Body1 in Bodies.ToList())
                            {
                                Body = Body.Replace(Body1, "token " + Lexer(Body1.Trim().Split(new char[] { '\n', '\r' },
                                    StringSplitOptions.RemoveEmptyEntries)[0])[1].Value);
                                Bodies.Add(Body);
                                Bodies = Bodies.Distinct().ToList();
                            }
                            Bodies = Bodies.Distinct().ToList();
                            Bodies = Bodies.Where(b => Lexer(b).Where(t => t.Name == "END").ToList().Count == 1).ToList();
                            foreach (string Body2 in Bodies.ToList())
                            {
                                Console.WriteLine("---------------------------------------");
                                Console.WriteLine(Body2);
                                Console.WriteLine("---------------------------------------\n");
                            }
                        }
                        InBody = false;
                    }
                    else if (Lexer(Line).Any(t => t.Name == "SEMI") && !InBody)
                    {
                        Console.WriteLine("---------------------------------------");
                        Console.WriteLine(Line);
                        Console.WriteLine("---------------------------------------\n");
                        break;
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
