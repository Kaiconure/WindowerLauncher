using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowerLauncher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var commands = new CommandLine(args);
            var app = new App(commands);
            app.Run();
        }
    }
}
