using Jint;
using sly.lexer;
using System;
using System.Collections.Generic;
using System.IO;

namespace GemCore
{
    public static class Core
    {
        static Engine engine = null;

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

        private static void Reload(FileInfo fileInfo)
        {
            var session = engine.Global.GetOwnProperties();
            engine = null;
            NewEngine(fileInfo);
            foreach (var v in session)
            {
                engine.Global.FastSetProperty(v.Key, v.Value);
            }
            engine.Global.RemoveOwnProperty("Reloaded");
            engine.Global.FastAddProperty("Reloaded", new Jint.Native.JsValue(true), true, true, true);
            ExecFile(fileInfo, true);
        }

        private static void NewEngine(FileInfo fileInfo)
        {
            engine = new Engine();
            engine.Global.FastAddProperty("Reloaded", new Jint.Native.JsValue(false), true, true, true);
            engine = engine.SetValue("log", new Action<object>(Console.WriteLine));
            engine = engine.SetValue("pause", new Action(() => { Console.ReadKey(); }));
            engine = engine.SetValue("reload", new Action(() => { Reload(fileInfo); }));
        }

        public static void ExecFile(FileInfo fileInfo, bool Reload = false)
        {
            Console.Title = fileInfo.FullName;
            List<Token<Lexer.LexerToken>> Tokens = Lexer.Lex(File.ReadAllText(fileInfo.FullName));
            if (!Reload) NewEngine(fileInfo);
            if (Tokens != null)
            {
                foreach (Token<Lexer.LexerToken> Token in Tokens)
                {
                    Console.WriteLine(Token.ToString());
                }
            }
            engine.Execute(File.ReadAllText(fileInfo.FullName.Replace(".gem", ".js")));
            Console.ReadKey();
        }
    }
}
