using System;
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
            Console.ReadKey();
        }
    }
}
