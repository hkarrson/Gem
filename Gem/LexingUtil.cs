using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Gem
{
    public static class LexingUtil
    {
        private static string Src = "";
        private static Dictionary<int, string> Matches1 = new Dictionary<int, string>();
        private static Dictionary<int, string> Matches2 = new Dictionary<int, string>();
        private static Dictionary<string, char> Chars = new Dictionary<string, char>();
        private static bool ignoreSpaces = false;

        public static void SetSource(string Src)
        {
            ignoreSpaces = false;
            Matches1.Clear();
            Matches2.Clear();
            Chars.Clear();
            LexingUtil.Src = Src;
        }

        public static void IgnoreSpaces()
        {
            ignoreSpaces = true;
        }

        public static void Add(string Name, string Pattern)
        {
            try
            {
                Chars.Add(Name, (char)(Chars.Last().Value + (char)1));
            }
            catch
            {
                Chars.Add(Name, 'a');
            }
            foreach (Match m1 in Regex.Matches(Src, ignoreSpaces ? "[^" + @"\s+" + "]+" : ".+"))
            {
                if (m1.Value.Length > 0)
                {
                    foreach (Match m2 in Regex.Matches(m1.Value, Pattern))
                    {
                        if (m2.Value.Length > 0)
                        {
                            int shift = 0;
                            int pre = 0;
                            bool add = true;
                            foreach (KeyValuePair<int, string> m3 in Matches1)
                            {
                                pre = m3.Key;
                                shift = m3.Key + m3.Value.Length;
                                if (m1.Index + m2.Index < shift && m1.Index + m2.Index > pre)
                                {
                                    add = false;
                                }
                            }
                            if (add)
                            {
                                if (!Matches1.ContainsKey(m1.Index + m2.Index))
                                {
                                    Matches1.Add(m1.Index + m2.Index, m2.Value);
                                    Matches2.Add(m1.Index + m2.Index, Name);
                                    pre = Matches1.Last().Key;
                                    shift = Matches1.Last().Key + Matches1.Last().Value.Length;
                                }
                            }
                        }
                    }
                }
            }
            List<KeyValuePair<int, string>> Lst = Matches1.ToList().OrderBy(KVP => KVP.Key).ToList();
            Matches1.Clear();
            foreach (KeyValuePair<int, string> KVP in Lst)
            {
                Matches1.Add(KVP.Key, KVP.Value);
            }
            Lst = Matches2.ToList().OrderBy(KVP => KVP.Key).ToList();
            Matches2.Clear();
            foreach (KeyValuePair<int, string> KVP in Lst)
            {
                Matches2.Add(KVP.Key, KVP.Value);
            }
        }

        public class Token
        {
            public string Name, Value;
            public void Print()
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("TOKEN");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(Name);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(", ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Value);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("] ");
            }
            public char ToChar()
            {
                return Chars[Name];
            }
        }

        public static List<Token> Pop()
        {
            List<Token> Lst = new List<Token>();
            int i = 0;
            foreach (string value in Matches1.Values.ToList())
            {
                string name = Matches2.Values.ToList()[i];
                Lst.Add(new Token() { Name = name, Value = value });
                i++;
            }
            Src = "";
            Matches1.Clear();
            Matches2.Clear();
            ignoreSpaces = false;
            return Lst;
        }
    }
}