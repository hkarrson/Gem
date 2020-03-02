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

            [Lexeme("new")]
            NEW = -7000,

            [Lexeme("this")]
            THIS = -6000,

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

            [Lexeme(@"\!\=")]
            NOTEQ = -107,

            [Lexeme(@"\!")]
            NOT = -106,

            [Lexeme(@"\&\&")]
            AND = -105,

            [Lexeme(@"\&")]
            BITAND = -104,

            [Lexeme(@"\|\|")]
            OR = -103,

            [Lexeme(@"\|")]
            BITOR = -102,

            [Lexeme(@"\=\=")]
            EQEQ = -101,

            [Lexeme(@"\=")]
            EQUALS = -100,

            [Lexeme("[0-9]+\\.[0-9]+")]
            DOUBLE = 1,
    
            [Lexeme("[0-9]+")]
            INT = 2,

            [Lexeme("\\+\\+")]
            PLUSPLUS = 3,

            [Lexeme("\\+")]
            PLUS = 4,

            [Lexeme("\\-\\-")]
            MINUSMINUS = 5,

            [Lexeme("\\-")]
            MINUS = 6,

            [Lexeme("[^\\/]\\*[^\\/]")]
            TIMES = 7,

            [Lexeme(@"[^\/*][\/][^\/*]")]
            DIVIDE = 8,

            [Lexeme("for")]
            FOR = 9,

            [Lexeme("while")]
            WHILE = 10,

            [Lexeme("if")]
            IF = 11,

            [Lexeme("else")]
            ELSE = 12,

            [Lexeme("\\<\\=")]
            LESSTHANEQ = 90,

            [Lexeme("\\<")]
            LESSTHAN = 95,

            [Lexeme("\\>\\=")]
            GREATERTHANEQ = 100,

            [Lexeme("\\>")]
            GREATERTHAN = 105,

            [Lexeme("\\(")]
            LPAREN = 110,

            [Lexeme("\\)")]
            RPAREN = 120,

            [Lexeme("\\{")]
            LBRACE = 130,

            [Lexeme("\\}")]
            RBRACE = 140,

            [Lexeme("\\[")]
            LBRACKET = 150,

            [Lexeme("\\]")]
            RBRACKET = 160,

            [Lexeme("[ \\t]+", true)]
            WS = 170,

            [Lexeme("[\\n\\r]+", false, true)]
            EOL = 180
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
