using System;
using System.IO;

namespace Gem
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] Src = File.ReadAllLines(args[0]);
            foreach (string Line in Src)
            {
                Console.WriteLine(Line);
            }
            Console.ReadKey();
        }
    }
}
