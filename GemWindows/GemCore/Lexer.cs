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
            [Lexeme(@"(?s)\/\*.+\*\/", true)]
            ML_COMMENT = -10000,

            [Lexeme(@"\/{2,}.*", true, true)]
            SL_COMMENT = 9900,

            [Lexeme(@"""[^""]+""")]
            STRING = -9000,

            [Lexeme("return")]
            RETURN = -5000,

            [Lexeme("hidden")]
            HIDDEN = -3000,

            [Lexeme("public")]
            PUBLIC = -2000,

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

            [Lexeme("[^\\/]\\*[^\\/]")]
            TIMES = 6,

            [Lexeme(@"[^\/*][\/][^\/*]")]
            DIVIDE = 7,

            [Lexeme("\\(")]
            LPAREN = 10,

            [Lexeme("\\)")]
            RPAREN = 11,

            [Lexeme("\\{")]
            LBRACE = 12,

            [Lexeme("\\}")]
            RBRACE = 13,

            [Lexeme("\\[")]
            LBRACKET = 14,

            [Lexeme("\\]")]
            RBRACKET = 15,

            [Lexeme("[ \\t]+", true)]
            WS = 16,

            [Lexeme("[\\n\\r]+", false, true)]
            EOL = 17
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
