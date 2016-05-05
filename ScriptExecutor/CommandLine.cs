using System;

namespace ScriptExecutor
{
    public struct CommandLine
    {
        public string ExePath { get; set; }

        public string Arguments { get; set; }

        public static CommandLine Create() => FromCommandLine(Environment.CommandLine);

        public static CommandLine FromCommandLine(string cmd)
        {
            string exePath;
            string args;
            if (cmd.StartsWith("\""))
            {
                var index = cmd.IndexOf("\"", 1, StringComparison.Ordinal);
                if (index < 0)
                {
                    exePath = cmd.Substring(1);
                    args = string.Empty;
                }
                else
                {
                    exePath = cmd.Substring(1, index - 1);
                    args = index + 1 < cmd.Length ? cmd.Substring(index + 1) : string.Empty;
                }
            }
            else
            {
                var index = cmd.IndexOf(" ", StringComparison.Ordinal);
                if (index < 0)
                {
                    exePath = cmd;
                    args = string.Empty;
                }
                else
                {
                    exePath = cmd.Substring(0, index);
                    args = index + 1 < cmd.Length ? cmd.Substring(index + 1) : string.Empty;
                }
            }
            return new CommandLine
            {
                ExePath = exePath,
                Arguments = args
            };
        }
    }
}