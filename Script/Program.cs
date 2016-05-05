using System;
using System.IO;
using System.Linq;
using ScriptCaller;
using ScriptExecutor;

namespace Script
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Startup(args);
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

        private static void Startup(string[] args)
        {
            if (args.Length == 0)
            {
                throw new InternalException("missing arguments");
            }

            switch (args[0].ToLower())
            {
                case "init":
                    Init();
                    break;

                case "install":
                    Install(args);
                    break;

                default:
                    throw new InternalException("unknown command.");
            }
        }

        private static void Init()
        {
            if (!Directory.Exists(ScriptHelper.ScriptsDirectory)) Directory.CreateDirectory(ScriptHelper.ScriptsDirectory);

            var target = EnvironmentVariableTarget.User;

            var cmd = CommandLine.Create();
            var envVar = Environment.GetEnvironmentVariable("Path", target);
            if (envVar == null)
            {
                Environment.SetEnvironmentVariable("Path", ScriptHelper.ScriptsDirectory, target);
            }
            else
            {
                var envVars = envVar.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!envVars.Any(z => string.Equals(z.Trim(), ScriptHelper.ScriptsDirectory, StringComparison.OrdinalIgnoreCase)))
                {
                    envVars.Insert(0, ScriptHelper.ScriptsDirectory);
                    Environment.SetEnvironmentVariable("Path", string.Join(";", envVars), target);
                }
            }

            Install("script", new ScriptConfig
            {
                ScriptPath = cmd.ExePath
            }, false);
        }

        private static void Install(string[] args)
        {
            if (args.Length == 1)
            {
                throw new InternalException("missing script path.");
            }
            else
            {
                string scriptPath;
                string commandName;
                if (args.Length > 2)
                {
                    scriptPath = args[2];
                    commandName = args[1];
                    if (commandName.Contains(" ")) throw new InternalException("command cannot contain whitespace.");
                }
                else
                {
                    scriptPath = args[1];
                    commandName = Path.GetFileNameWithoutExtension(args[1]);
                    commandName = commandName.Replace(" ", "-");
                }
                if (!File.Exists(scriptPath)) throw new InternalException("script was not exists.");
                var config = new ScriptConfig
                {
                    ScriptPath = Path.GetFullPath(scriptPath)
                };
                if (scriptPath.ToLower().EndsWith(".py"))
                {
                    config.Executor = "python";
                }
                Install(commandName, config, true);
            }
        }

        private static void Install(string commandName, ScriptConfig config, bool existsThrow)
        {
            if (!Directory.Exists(ScriptHelper.ScriptsDirectory)) Directory.CreateDirectory(ScriptHelper.ScriptsDirectory);

            var configFileName = Path.Combine(ScriptHelper.ScriptsDirectory, commandName + ".config");
            if (File.Exists(configFileName))
            {
                if (existsThrow) throw new InternalException("command already exists.");
            }
            else
            {
                config.SaveToFile(configFileName);
            }

            var caller = Path.Combine(ScriptHelper.EntryDirectory, nameof(ScriptCaller) + ".exe");
            File.Copy(caller, Path.Combine(ScriptHelper.ScriptsDirectory, commandName + ".exe"), true);
        }
    }
}
