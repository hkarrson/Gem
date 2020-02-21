using System;
using System.Collections.Generic;
using System.Text;

namespace Gem
{
    public static class Lexer
    {
        public static List<LexingUtil.Token> Lex(string Line)
        {
            List<LexingUtil.Token> Tokens = new List<LexingUtil.Token>();
            LexingUtil.SetSource(Line);
            LexingUtil.Add("COMMENT", @"\/\/.*");
            LexingUtil.Add("STRING", @"\"".*\""");
            LexingUtil.IgnoreSpaces();
            LexingUtil.Add("TOKEN", @"token");
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
    }
}
