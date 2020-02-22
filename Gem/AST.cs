using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static Gem.Lexer;
using static Gem.LexingUtil;

namespace Gem
{
    public static class AST
    {
        public class LineNode : List<LineNode>
        {
            public List<Token> LexedLine = new List<Token>();
            public List<Token> LexedLineLossy
            {
                get
                {
                    List<Token> Lst = new List<Token>();
                    foreach (Token t in LexedLine)
                    {
                        if (t.Name != "COMMENT")
                        {
                            Lst.Add(t);
                        }
                    }
                    return Lst;
                }
            }

            public LineNode(List<Token> LexedLine)
            {
                this.LexedLine = LexedLine;
            }

            public LineNode() { }
        }

        public static LineNode RunLexerAndGetSyntaxTree(string[] args)
        {
            string[] Src = File.ReadAllLines(args[0]);
            Src = Src.Select(ln => Regex.Replace(ln, @"\t", "    ")).ToArray();
            List<LineNode> PathStack = new List<LineNode>();
            LineNode RootNode = new LineNode();
            PathStack.Add(RootNode);
            foreach (string Line in Src)
            {
                List<Token> LexedLine = Lex(Line);
                LineNode CurrentNode = new LineNode(LexedLine);
                if (LexedLine.Count >= 2 && (LexedLine[0].Name == "GLOBAL" || LexedLine[0].Name == "HIDDEN") &&
                    LexedLine[1].Name == "NAME" && !LexedLine.Any(ln => ln.Name == "SEMI"))
                {
                    PathStack[PathStack.Count - 1].Add(CurrentNode);
                    PathStack.Add(CurrentNode);
                }
                else if (LexedLine.Count >= 1 && LexedLine[0].Name == "END")
                {
                    PathStack.RemoveAt(PathStack.Count - 1);
                }
                else
                {
                    PathStack[PathStack.Count - 1].Add(CurrentNode);
                }
            }
            RootNode = PathStack[0];
            return RootNode;
        }
    }
}
