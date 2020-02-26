using sly.lexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GemCore
{
    public static class Core
    {
        public static string JS = "";

        public static void ExecFile(string Path)
        {
            FileInfo fileInfo = null;
            try
            {
                fileInfo = new FileInfo(Path);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine
                    ("Invalid file path... Full exception below..." +
                    Environment.NewLine + ex.ToString());
                Console.ReadKey();
            }
            if (fileInfo != null) ExecFile(fileInfo);
        }

        public static void ExecFile(FileInfo fileInfo)
        {
            Console.Title = fileInfo.FullName;
            List<Token<Lexer.LexerToken>> Tokens = Lexer.Lex(File.ReadAllText(fileInfo.FullName));
            if (Tokens != null)
            {
                List<Token<Lexer.LexerToken>> Line = new List<Token<Lexer.LexerToken>>();
                foreach (Token<Lexer.LexerToken> EOL in Tokens)
                {
                    Line.Add(EOL);
                    if (EOL.TokenID == Lexer.LexerToken.EOL)
                    {
                        Line.RemoveAt(Line.Count - 1);
                        if (!Line.Any(T => T.TokenID == Lexer.LexerToken.SEMI))
                        {
                            // Not a statement
                        }
                        else
                        {
                            // Statement groups
                        }
                        Line.Clear();
                    }
                }
            }
            Console.WriteLine(JS);
            Console.ReadKey();
        }
    }
}
