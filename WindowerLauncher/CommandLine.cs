using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowerLauncher
{
    internal class CommandLine
    {
        private readonly string[] args;

        public CommandLine(string[] args)
        {
            if(args.Length == 0 || !Enum.TryParse(args[0], true, out CommandType type))
            {
                this.Type  = CommandType.None;
                return;
            }

            this.Type = type;
            this.args = args.Skip(1).ToArray();
        }

        public bool GetArgumentBool(string name)
        {
            var options = new[]
            {
                $"-{name}",
                $"/{name}",
                $"-{name}:true",
                $"/{name}:true",
            };

            return this.args.Any(arg => options.Contains(arg, StringComparer.OrdinalIgnoreCase));
        }

        public bool GetArgumentString(string name, out string value, string defaultValue = null)
        {
            var prefix1 = $"-{name}:";
            var prefix2 = $"/{name}:";
            foreach(var arg in this.args)
            {
                if(arg.StartsWith(prefix1, StringComparison.OrdinalIgnoreCase))
                {
                    value = arg.Substring(prefix1.Length);
                    return true;
                }
                else if(arg.StartsWith(prefix2, StringComparison.OrdinalIgnoreCase))
                {
                    value = arg.Substring(prefix2.Length);
                    return true;
                }
            }
            value = defaultValue;
            return false;
        }

        public bool GetArgumentInt(string name, out int value, int defaultValue = 0)
        {
            if(this.GetArgumentString(name, out var strValue))
            {
                if(int.TryParse(strValue, out value))
                {
                    return true;
                }
            }
            value = defaultValue;
            return false;
        }

        public bool GetArgumentFloat(string name, out float value, float defaultValue = 0)
        {
            if (this.GetArgumentString(name, out var strValue))
            {
                if (float.TryParse(strValue, out value))
                {
                    return true;
                }
            }
            value = defaultValue;
            return false;
        }

        public CommandType Type { get; private set; }
    }

    public enum CommandType
    {
        None,
        Save,
        Run,
        Minify,
        New,
        Activate,
        Id,
        Ident,
        Identify,
        Si,
        SysInfo,
        SystemInfo,
        Affinitize,
        Version,
    }
}
