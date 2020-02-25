using Jint;
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

        public static void AppendToMethod(Engine engine, string FunctionName, string src)
        {
            engine.Execute(FunctionName + @" = (function() {
            var cached_function = " + FunctionName + @";

            return function() {
                var result = cached_function.apply(this, arguments);

                " + src + @"

                return result;
            };
        })();");
        }

        public static void ExecFile(FileInfo fileInfo)
        {
            Console.Title = fileInfo.FullName;
            List<Token<Lexer.LexerToken>> Tokens = Lexer.Lex(File.ReadAllText(fileInfo.FullName));
            var engine = new Engine().SetValue("log", new Action<object>(Console.WriteLine));
            engine.Execute("function hello() { }");
            AppendToMethod(engine, "hello", "log('Hello World!');");
            AppendToMethod(engine, "hello", "log('Hello Gem!');");
            engine.Execute("hello();");
            //if (Tokens != null)
            //{
            //    foreach (Token<Lexer.LexerToken> Token in Tokens)
            //    {
            //        Console.WriteLine(Token.ToString());
            //    }
            //}
            Console.ReadKey();
        }
    }
}
