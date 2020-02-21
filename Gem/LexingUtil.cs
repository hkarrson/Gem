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
        private static List<string> IgnorePatterns = new List<string>();

        public static void SetSource(string Src)
        {
            IgnorePatterns.Clear();
            Matches1.Clear();
            Matches2.Clear();
            LexingUtil.Src = Src;
        }

        public static void Ignore(string Pattern)
        {
            IgnorePatterns.Add(Pattern);
        }

        public static void Add(string Name, string Pattern)
        {
            List<string> Src1 = new List<string>();
            Src1.Add(Src);
            foreach (string p in IgnorePatterns)
            {
                string[] Src3 = Src1.ToArray();
                Src1.Clear();
                foreach (string Src2 in Src3)
                {
                    Src1.AddRange(Regex.Split(Src2, p).ToList());
                }
            }
            foreach (string Src4 in Src1)
            {
                foreach (Match m in Regex.Matches(Src4, Pattern))
                {
                    Matches1.Add(m.Index, m.Value);
                    Matches2.Add(m.Index, Name);
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
            IgnorePatterns.Clear();
            return Lst;
        }
    }
}