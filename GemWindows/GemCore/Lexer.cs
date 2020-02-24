using sly.lexer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GemCore
{
    public class Lexer
    {
        public enum Token
        {
            // float number 
            [Lexeme("[0-9]+\\.[0-9]+")]
            DOUBLE = 1,

            // integer        
            [Lexeme("[0-9]+")]
            INT = 3,

            // the + operator
            [Lexeme("\\+")]
            PLUS = 4,

            // the - operator
            [Lexeme("\\-")]
            MINUS = 5,

            // the * operator
            [Lexeme("\\*")]
            TIMES = 6,

            //  the  / operator
            [Lexeme("\\/")]
            DIVIDE = 7,

            // a left paranthesis (
            [Lexeme("\\(")]
            LPAREN = 8,

            // a right paranthesis )
            [Lexeme("\\)")]
            RPAREN = 9,

            // a whitespace
            [Lexeme("[ \\t]+", true)]
            WS = 12,

            [Lexeme("[\\n\\r]+", true, true)]
            EOL = 14
        }

        public static void Lex(string src)
        {
            ILexer<Token> lexer = (ILexer<Token>)LexerBuilder.BuildLexer<Token>();
            var tokens = lexer.Tokenize(src).Tokens;
        }
    }
}
