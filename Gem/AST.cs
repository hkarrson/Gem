using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Gem.Lexer;
using static Gem.LexingUtil;

namespace Gem
{
    public static class AST
    {
        public class Block
        {
            public List<List<Token>> Code = new List<List<Token>>();
            public List<Block> Children = new List<Block>();
        }

        private static List<Block> MoveBlocksA = new List<Block>();
        private static List<Block> MoveBlocksB = new List<Block>();

        private static void Remove(Block block, Block main)
        {
            foreach (Block b in main.Children.ToList())
            {
                if (b == block)
                {
                    main.Children.Remove(b);
                }
                else
                {
                    Remove(block, b);
                }
            }
        }

        private static Block Find(string name, Block main)
        {
            foreach (Block b in main.Children.ToList())
            {
                if (b.Code.Any() && b.Code[0].Count >= 2 && 
                    (b.Code[0][0].Name == "GLOBAL" || 
                    b.Code[0][0].Name == "HIDDEN") &&
                    b.Code[0][1].Name == "NAME" &&
                    b.Code[0][1].Value == name)
                {
                    return b;
                }
                else
                {
                    Block b1 = Find(name, b);
                    if (b1 != null)
                    {
                        return b1;
                    }
                }
            }
            return null;
        }

        public static void RunLexerAndGetSyntaxTree(string[] args)
        {
            string[] Src = File.ReadAllLines(args[0]);
            Src = Src.Select(ln => Regex.Replace(ln, @"\t", "    ")).ToArray();
            Block main = new Block();
            Console.WriteLine("...\n");
            List<string> Starts = new List<string>();
            List<string> Bodies = new List<string>();
            bool InBody = false;
            foreach (string Line in Src)
            {
                List<Token> Tokens = Lex(Line);
                foreach (Token Token in Tokens)
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
                                Block block = new Block();
                                List<string> Lines = Body2.Split(new char[] { '\n', '\r' },
                                    StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                                Lines.RemoveAll(l => l.Length <= 0);
                                foreach (string ln in Lines)
                                {
                                    List<Token> tokens = new List<Token>();
                                    if (!ln.StartsWith(@"//"))
                                    {
                                        foreach (Token t in Lex(ln))
                                        {
                                            t.Print();
                                            if (t.Name != "COMMENT")
                                            {
                                                tokens.Add(t);
                                            }
                                        }
                                        Console.WriteLine("");
                                    }
                                    if (tokens.Any())
                                    {
                                        block.Code.Add(tokens);
                                    }
                                }
                                Console.WriteLine("\n...\n");
                                main.Children.Add(block);
                            }
                        }
                        InBody = false;
                    }
                    else if (Lex(Line).Any(t => t.Name == "SEMI") && !InBody)
                    {
                        if (!Line.Trim().StartsWith(@"//"))
                        {
                            List<Token> tokens = new List<Token>();
                            foreach (Token t in Lex(Line.Trim()))
                            {
                                if (t.Name != "COMMENT")
                                {
                                    t.Print();
                                    tokens.Add(t);
                                }
                            }
                            Console.WriteLine("");
                            Console.WriteLine("\n...\n");
                            if (tokens.Any())
                            {
                                main.Code.Add(tokens);
                            }
                        }
                        break;
                    }
                }
            }
            foreach (Block b in main.Children)
            {
                for (int i = b.Code.Count - 1; i >= 0; i--)
                {
                    List<Token> ln = b.Code[i];
                    if (ln.Count == 2 && ln[0].Name == "TOKEN" && ln[1].Name == "NAME")
                    {
                        string name = ln[1].Value;
                        Block MoveFrom = Find(name, main);
                        Block MoveTo = b;
                        MoveBlocksA.Add(MoveFrom);
                        MoveBlocksB.Add(MoveTo);
                        b.Code.RemoveAt(i);
                    }
                }
            }
            int i1 = 0;
            foreach (Block A in MoveBlocksA)
            {
                Remove(A, main);
                MoveBlocksB[i1].Children.Add(A);
                i1++;
            }
            MoveBlocksA.Clear();
            MoveBlocksB.Clear();
            Console.WriteLine(string.Join("\n", main.Children.Select(b => string.Join
            ("\n", b.Code.Select(l => string.Join(" ", l.Select(t => t.Value)))))));
        }
    }
}
