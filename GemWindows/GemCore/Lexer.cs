using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using sly.lexer;
using sly.buildresult;

namespace GemCore
{
    public class Lexer
    {
        public enum LexerToken
        {
            [Lexeme(@""".+""")]
            STRING = -10000,

            [Lexeme("[a-zA-z_][a-zA-Z0-9_]*")]
            NAME = -1000,

            [Lexeme(@";")]
            SEMI = -300,

            [Lexeme(@"\.")]
            DOT = -200,

            [Lexeme(@"\=")]
            EQUALS = -100,

            [Lexeme("[0-9]+\\.[0-9]+")]
            DOUBLE = 1,
    
            [Lexeme("[0-9]+")]
            INT = 3,

            [Lexeme("\\+")]
            PLUS = 4,

            [Lexeme("\\-")]
            MINUS = 5,

            [Lexeme("\\*")]
            TIMES = 6,

            [Lexeme("\\/")]
            DIVIDE = 7,

            [Lexeme("\\(")]
            LPAREN = 8,

            [Lexeme("\\)")]
            RPAREN = 9,

            [Lexeme("[ \\t]+", true)]
            WS = 12,

            [Lexeme("[\\n\\r]+", true, true)]
            EOL = 14
        }

        public static List<Token<LexerToken>> Lex(string src)
        {
            BuildResult<ILexer<LexerToken>> lexer = LexerBuilder.BuildLexer<LexerToken>();
            if (lexer.IsOk)
            {
                LexerResult<LexerToken> R = lexer.Result.Tokenize(src);
                if (R.IsOk)
                {
                    return R.Tokens;
                }
                else
                {
                    Console.Error.WriteLine(R.Error.ErrorMessage);
                }
            }
            else
            {
                foreach (InitializationError Error in lexer.Errors)
                {
                    Console.Error.WriteLine(Error.Message);
                }
            }
            return null;
        }
    }
}
