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
        static void Main(string[] args)
        {
            LineNode AST = RunLexerAndGetSyntaxTree(args);
            List<string> Stack = new List<string>();
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
                                            LineNode b1 = AST;
                                            string MethodName = string.Join(".", Stack);
                                            bool OK = true;
                                            while (Stack.Any())
                                            {
                                                string n = Stack.First();
                                                Stack.RemoveAt(0);
                                                bool ok = false;
                                                foreach (LineNode Ln in b1)
                                                {
                                                    if (Ln.LexedLineLossy.Count >= 2 && (Ln.LexedLineLossy[0].Name == "GLOBAL" ||
                                                        Ln.LexedLineLossy[0].Name == "HIDDEN") && Ln.LexedLineLossy[1].Name == "NAME")
                                                    {
                                                        if (Ln.LexedLineLossy[1].Value == n)
                                                        {
                                                            b1 = Ln;
                                                            ok = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (!ok || b1 == AST)
                                                {
                                                    OK = false;
                                                    break;
                                                }
                                            }
                                            if (b1 == AST || Stack.Any() || !OK)
                                            {
                                                Console.WriteLine("Method '" + MethodName + "' does not exist!");
                                            }
                                            else
                                            {
                                                TreeStack.Add(b1);
                                                CallerIndexStack.Add(CurrentNodeIndex);
                                                CurrentNodeIndex = -1;
                                            }
                                            Stack.Clear();
                                            break;
                                        }
                                        else
                                        {
                                            if (CurrentNode.LexedLineLossy[i].Name == "NAME")
                                            {
                                                Stack.Add(CurrentNode.LexedLineLossy[i].Value);
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
