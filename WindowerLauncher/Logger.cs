using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowerLauncher
{
    internal class Logger
    {
        private Logger()
        {

        }

        public void Log(string message)
        {
            //Console.WriteLine("{0}", $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {message}");
            Console.WriteLine("{0}", $"{message}");
        }

        public void Log(string format, params object[] args)
        {
            this.Log(string.Format(format, args));
        }

        public void Log(Exception ex)
        {
            this.Error(ex.ToString());
        }

        public void Error(string format, params object[] args)
        {
            this.Log($"**ERROR: {string.Format(format, args)}");
        }

        public static Logger Instance { get; } = new Logger();
    }
}
