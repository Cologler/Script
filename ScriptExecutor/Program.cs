using System;
using System.IO;
using System.Text;
using ScriptCaller;

namespace ScriptExecutor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Call(args[0]);
            }
            catch (InternalException e)
            {
                Console.WriteLine($"error: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Call(string command)
        {
            var cmd = CommandLine.FromCommandLine(Encoding.UTF8.GetString(Convert.FromBase64String(command)));
            var configPath = Path.Combine(ScriptHelper.ScriptsDirectory, Path.ChangeExtension(Path.GetFileName(cmd.ExePath), "config"));
            if (!File.Exists(configPath)) throw new InternalException("missing script config.");
            var config = ScriptConfig.CreateFromFile(configPath);
            if (config.Upgrade())
            {
                File.Delete(configPath);
                config.SaveToFile(configPath);
            }

            try
            {
                if (string.IsNullOrWhiteSpace(config.Executor))
                {
                    Caller.Call(config.ScriptPath, cmd.Arguments);
                }
                else
                {
                    Caller.Call(config.Executor, $"\"{config.ScriptPath}\" {cmd.Arguments}");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("unknown command.");
            }
        }
    }
}
