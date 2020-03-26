using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gemctools;

namespace gemc
{
    class Program
    {
        static void Main(string[] args)
        {
            CompilerFactory.TryCreateNew("0.0.3");
            CoreCompiler.Compile
                (CompilerFactory.GetGemPath(),
                CompilerFactory.GetCSC());
        }
    }
}
