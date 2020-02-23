using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using static Gem.AST;
using System.Xml.Serialization;

namespace Gem
{
    class Program
    {
        static void GetChildMethods(LineNode ParentNode, string ChildNode, ref List<LineNode> Methods)
        {
            foreach (LineNode Line in ParentNode)
            {
                if (Line.LexedLineLossy.Count >= 2 && (Line.LexedLineLossy[0].Name == "GLOBAL" ||
                    Line.LexedLineLossy[0].Name == "HIDDEN") && Line.LexedLineLossy[1].Name == "NAME" &&
                    Line.LexedLineLossy[1].Value == ChildNode)
                {
                    Methods.Add(Line);
                }
                GetChildMethods(Line, ChildNode, ref Methods);
            }
        }

        static LineNode GetParent(LineNode AST, LineNode ChildNode)
        {
            if (AST.Any(l => l.LexedLineLossy.Count >= 2 && ChildNode.LexedLineLossy.Count >= 2 &&
            l.LexedLineLossy[1].Value == ChildNode.LexedLineLossy[1].Value &&
            (l.LexedLineLossy[0].Name == "GLOBAL" ||
            l.LexedLineLossy[0].Name == "HIDDEN") &&
            l.LexedLineLossy[1].Name == "NAME"))
            {
                return AST;
            }
            if (AST.Any(l => l == ChildNode))
            {
                return AST;
            }
            foreach (LineNode Node in AST)
            {
                LineNode ParentNode = GetParent(Node, ChildNode);
                if (ParentNode != null)
                {
                    return ParentNode;
                }
            }
            return null;
        }

        static int GetDepth(LineNode AST, LineNode Node, int i = 0)
        {
            LineNode ParentNode = GetParent(AST, Node);
            if (ParentNode != null && !ParentNode.LexedLineLossy.Any()) ParentNode = null;
            if (ParentNode == null)
            {
                return i;
            }
            else
            {
                return GetDepth(AST, ParentNode, i + 1);
            }
        }

        static LineNode GetMethod(LineNode AST, LineNode CurrentNode, List<string> StackRaw)
        {
            List<LineNode> Methods = new List<LineNode>();
            GetChildMethods(AST, StackRaw.Last(), ref Methods);
            foreach (LineNode Method in Methods)
            {
                LineNode ParentNode = Method;
                int x = 0;
                List<string> Stack = StackRaw.ToList();
                Stack.Reverse();
                bool B = true;
                foreach (string s in Stack)
                {
                    x++;
                    if (x > 1)
                    {
                        bool b = false;
                        ParentNode = GetParent(AST, ParentNode);
                        try
                        {
                            if (ParentNode.LexedLineLossy[1].Value == s)
                            {
                                b = true;
                            }
                        }
                        catch { }
                        B = B & b;
                    }
                    if (GetParent(AST, CurrentNode) != null && ParentNode.LexedLine.Any() && ParentNode.LexedLineLossy[1].Value == StackRaw[0])
                    {
                        LineNode n = null;
                        try
                        {
                            n = GetParent(AST, CurrentNode);
                        }
                        catch { }
                        if (n != null)
                        {
                            while (true)
                            {
                                if (n != null && n.LexedLine.Count >= 2 && n.LexedLineLossy[1].Value == StackRaw[0] ||
                                    n.Any(l => l != null && l.LexedLine.Count >= 2 && l.LexedLineLossy[1].Value == StackRaw[0]))
                                {
                                    break;
                                }
                                n = GetParent(AST, n);
                                if (n == null)
                                {
                                    B = false;
                                    break;
                                }
                            }
                        }
                        if (!(GetDepth(AST, ParentNode) <= GetDepth(AST, GetParent(AST, CurrentNode))))
                        {
                            B = false;
                        }
                    }
                }
                if (B)
                {
                    return Method;
                }
            }
            return null;
        }

        static void Main(string[] args)
        {
            LineNode AST = RunLexerAndGetSyntaxTree(args);
            List<string> StackOld = new List<string>();
            List<LineNode> TreeStack = new List<LineNode>();
            List<int> CallerIndexStack = new List<int>();
            TreeStack.Add(AST);
            for (int CurrentNodeIndex = 0; CurrentNodeIndex <= TreeStack.Last().Count; CurrentNodeIndex++)
            {
                if (CurrentNodeIndex < TreeStack.Last().Count)
                {
                    LineNode CurrentNode = TreeStack.Last()[CurrentNodeIndex];
                    if (CurrentNode.LexedLineLossy.Any())
                    {
                        if (CurrentNode.LexedLineLossy.Last().Name == "SEMI")
                        {
                            if (CurrentNode.LexedLineLossy.First().Name == "NAME" &&
                                CurrentNode.LexedLineLossy[CurrentNode.LexedLineLossy.Count - 2].Name == "RPAREN")
                            {
                                bool InArgs = false;
                                for (int i = 0; true; i++)
                                {
                                    if (i < CurrentNode.LexedLineLossy.Count)
                                    {
                                        if (InArgs)
                                        {

                                        }
                                        else if (CurrentNode.LexedLineLossy[i].Name == "LPAREN")
                                        {
                                            InArgs = true;
                                            string MethodName = string.Join(".", StackOld);
                                            List<string> StackRaw = StackOld.ToList();
                                            LineNode Method = GetMethod(AST, CurrentNode, StackRaw);
                                            if (Method != null)
                                            {
                                                TreeStack.Add(Method);
                                                CallerIndexStack.Add(CurrentNodeIndex);
                                                CurrentNodeIndex = -1;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Method '" + MethodName + "' does not exist!");
                                            }
                                            StackOld.Clear();
                                            break;
                                        }
                                        else
                                        {
                                            if (CurrentNode.LexedLineLossy[i].Name == "NAME")
                                            {
                                                StackOld.Add(CurrentNode.LexedLineLossy[i].Value);
                                            }
                                            else if (CurrentNode.LexedLineLossy[i].Name == "SEMI")
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if ((CurrentNodeIndex != -1 || TreeStack.Last().Count <= 0) && CurrentNodeIndex >= TreeStack.Last().Count - 1)
                {
                    if (TreeStack.Count > 1)
                    {
                        TreeStack.RemoveAt(TreeStack.Count - 1);
                        int CallerIndex = CallerIndexStack.Last();
                        CallerIndexStack.RemoveAt(CallerIndexStack.Count - 1);
                        CurrentNodeIndex = CallerIndex;
                    }
                }
            }
            Console.ReadKey();
        }

        public static string SerializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
}
