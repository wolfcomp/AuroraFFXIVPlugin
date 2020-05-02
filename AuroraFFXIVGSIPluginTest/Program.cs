using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuroraFFXIVGSIPlugin;
using SharpDX.RawInput;

namespace AuroraFFXIVGSIPluginTest
{
    class Program
    {
        static void Main(string[] args)
        {
            new Main().MainAsync(args).GetAwaiter().GetResult();
        }
    }
}