using sly.lexer;
using System;
using System.Collections.Generic;
using System.IO;

namespace GemCore
{
    public static class Core
    {
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
                foreach (Token<Lexer.LexerToken> Token in Tokens)
                {
                    Console.WriteLine(Token.ToString());
                }
            }
            Console.ReadKey();
        }
    }
}
