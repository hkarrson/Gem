using sly.buildresult;
using sly.lexer;
using sly.parser;
using sly.parser.generator;
using sly.parser.syntax.tree;
using System;
using System.Collections.Generic;
using System.Text;

namespace GemCore
{
    public class AST
    {
        [Production("primary: INT")]
        public int Primary(Token<Lexer.LexerToken> intToken)
        {
            return intToken.IntValue;
        }

        public static ISyntaxNode<Lexer.LexerToken> Parse(string src)
        {
            AST ast = new AST();
            ParserBuilder<Lexer.LexerToken, int> PB = new ParserBuilder<Lexer.LexerToken, int>();
            BuildResult<Parser<Lexer.LexerToken, int>> parser = PB.BuildParser(ast, ParserType.LL_RECURSIVE_DESCENT, "0");
            if (parser.IsOk)
            {
                ParseResult<Lexer.LexerToken, int> R = parser.Result.Parse(src);
                if (R.IsOk)
                {
                    return R.SyntaxTree;
                }
                else
                {
                    foreach (ParseError Error in R.Errors)
                    {
                        Console.Error.WriteLine(Error.ErrorMessage);
                    }
                }
            }
            else
            {
                foreach (InitializationError Error in parser.Errors)
                {
                    Console.Error.WriteLine(Error.Message);
                }
            }
            return null;
        }
    }
}
