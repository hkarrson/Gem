using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gem
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] Src = File.ReadAllLines(args[0]);
            List<LexingUtil.Token> Tokens = new List<LexingUtil.Token>();
            foreach (string Line in Src)
            {
                LexingUtil.SetSource(Line);
                LexingUtil.Add("STRING", @"\"".*\""");
                LexingUtil.IgnoreSpaces();
                LexingUtil.Add("GLOBAL", @"global");
                LexingUtil.Add("HIDDEN", @"hidden");
                LexingUtil.Add("END", @"end");
                LexingUtil.Add("NAME", @"[a-zA-z_][a-zA-Z0-9_]*");
                Tokens.AddRange(LexingUtil.Pop());
            }
            foreach (LexingUtil.Token Token in Tokens)
            {
                Console.WriteLine(Token.Name + " ::: " + Token.Value);
            }
            Console.ReadKey();
        }
    }
}
