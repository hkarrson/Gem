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
            [Lexeme("hidden")]
            HIDDEN = 0,

            [Lexeme("public")]
            PUBLIC = 1,

            [Lexeme("->")]
            ARROW = 2,

            [Lexeme("\\$")]
            THIS = 3,

            [Lexeme("[ \\t]+", true)]
            WS = 4,

            [Lexeme("[\\n\\r]+", false, true)]
            EOL = 5,

            [Lexeme("[^\\s\\$]+")]
            JS = 6,
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
