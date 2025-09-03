using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindowerLauncher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // If no arguments were provided, we'll run as a command line application.
            if (args.Length == 0)
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C DOSKEY wl=WindowerLauncher.exe $* && cmd.exe /K title WindowerLauncher Command Prompt",
                    UseShellExecute = false,
                    WorkingDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName,
                });

                process.WaitForExit();
            }
            else
            {
                var commands = new CommandLine(args);
                var app = new App(commands);
                app.Run();
            }
        }
    }
}
