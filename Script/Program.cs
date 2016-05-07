using System;
using System.Collections.Generic;
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

            if (!Directory.Exists(ScriptHelper.ScriptsDirectory)) Directory.CreateDirectory(ScriptHelper.ScriptsDirectory);

            switch (args[0].ToLower())
            {
                case "init":
                    Init();
                    break;

                case "install":
                    Install(args);
                    break;

                case "list":
                    List();
                    break;

                case "uninstall":
                    Uninstall(args.Skip(1).ToArray());
                    break;

                default:
                    throw new InternalException("unknown command.");
            }
        }

        private static void Init()
        {
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
                    commandName = (Path.GetFileNameWithoutExtension(args[1]) ?? string.Empty).Replace(" ", "-");
                    if (commandName.Length == 0) throw new InternalException("can not parse command from file name: empty file name.");
                }

                if (!File.Exists(scriptPath)) throw new InternalException("script was not exists.");
                var config = new ScriptConfig
                {
                    ScriptPath = Path.GetFullPath(scriptPath)
                };
                if (config.Executor == null)
                {
                    var scriptFormat = scriptPath.ToLower();
                    if (scriptFormat.EndsWith(".py")) config.Executor = "python";
                    else if (scriptFormat.EndsWith(".js")) config.Executor = "node";
                }
                Install(commandName, config, true);
            }
        }

        private static void Install(string commandName, ScriptConfig config, bool existsThrow)
        {
            var configFilePath = Path.Combine(ScriptHelper.ScriptsDirectory, commandName + ScriptConfig.ExtensionName);
            if (File.Exists(configFilePath))
            {
                if (existsThrow) throw new InternalException("command already exists.");
            }
            else
            {
                config.SaveToFile(configFilePath);
            }

            var caller = Path.Combine(ScriptHelper.EntryDirectory, nameof(ScriptCaller) + ".exe");
            File.Copy(caller, Path.Combine(ScriptHelper.ScriptsDirectory, commandName + ".exe"), true);
        }

        private static void List()
        {
            var commandSet = new HashSet<string>();
            foreach (var file in Directory.EnumerateFiles(ScriptHelper.ScriptsDirectory))
            {
                if (file.EndsWith(ScriptConfig.ExtensionName, StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    var command = Path.GetFileNameWithoutExtension(file);
                    if (commandSet.Add(command)) Console.WriteLine("    " + command);
                }
            }
        }

        private static void Uninstall(string[] args)
        {
            if (args.Length == 0) throw new InternalException("missing command name.");
            var command = args[0];

            var caller = GetCallerPath(command);
            if (File.Exists(caller)) File.Delete(caller);

            var config = GetConfigPath(command);
            if (!File.Exists(config)) throw new InternalException("command not found");
            File.Delete(config);
        }

        private static string GetConfigPath(string command)
            => Path.Combine(ScriptHelper.ScriptsDirectory, command + ScriptConfig.ExtensionName);

        private static string GetCallerPath(string command)
            => Path.Combine(ScriptHelper.ScriptsDirectory, command + ".exe");
    }
}
