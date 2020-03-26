using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gemctools
{
    public static class Helpers
    {
        public static Int32 StartProcess(string exe, string args)
        {
            ProcessStartInfo ps = new ProcessStartInfo(exe, args);
            ps.UseShellExecute = false;
            Process p = Process.Start(ps);
            p.WaitForExit();
            return p.ExitCode;
        }

        public static string GetParent(string DirName, string p = "")
        {
            if (p == "")
            {
                p = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                if (Path.GetFileNameWithoutExtension(p).ToLower() == DirName.ToLower())
                {
                    return p;
                }
            }
            try
            {
                string str = Directory.GetParent(p).FullName;
                if (Path.GetFileNameWithoutExtension(str).ToLower() == DirName.ToLower())
                {
                    return str;
                }
                else if (Directory.Exists(Path.Combine(str, DirName)))
                {
                    return Path.Combine(str, DirName);
                }
                else
                {
                    return GetParent(DirName, str);
                }
            }
            catch
            {
                return p;
            }
        }
        
        public static string GetFromParent(string DirName, string SubPath)
        {
            return Path.Combine(GetParent(DirName), SubPath);
        }
    }
}
