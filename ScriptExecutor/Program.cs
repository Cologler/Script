using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ScriptCaller;

namespace ScriptExecutor
{
    class Program
    {
        const string ScriptsPath = "Scripts";

        static void Main(string[] args)
        {
            try
            {
                Startup(args);
            }
            catch (InternalException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("input any key to exit.");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("input any key to exit.");
                Console.ReadKey();
            }
        }

        static void Startup(string[] args)
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
                    if (args.Length < 2)
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
                            Debug.Assert(commandName != null);
                            commandName = commandName.Replace(" ", "-");
                        }
                        if (!File.Exists(scriptPath)) throw new InternalException("script was not exists.");

                        Install(scriptPath, commandName);
                    }
                    break;

                default:
                    throw new InternalException("unknown command.");
            }
        }

        static void Init()
        {
            if (!Directory.Exists(ScriptsPath)) Directory.CreateDirectory(ScriptsPath);

            var target = EnvironmentVariableTarget.User;

            var cmd = CommandLine.Create();
            var destDir = Path.Combine(Path.GetDirectoryName(cmd.ExePath), ScriptsPath);
            var envVar = Environment.GetEnvironmentVariable("Path", target);
            if (envVar == null)
            {
                Environment.SetEnvironmentVariable("Path", destDir, target);
                return;
            }
            var envVars = envVar.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (envVars.Any(z => string.Equals(z.Trim(), destDir, StringComparison.OrdinalIgnoreCase))) return;
            envVars.Insert(0, destDir);
            Environment.SetEnvironmentVariable("Path", string.Join(";", envVars), target);
        }

        static void Install(string scriptPath, string commandName)
        {
            if (!Directory.Exists(ScriptsPath)) Directory.CreateDirectory(ScriptsPath);

            var config = new ScriptConfig
            {
                ScriptPath = scriptPath
            };

            config.SaveToFile(Path.Combine(ScriptsPath, commandName + ".config"));
            File.Copy(nameof(ScriptCaller) + ".exe", Path.Combine(ScriptsPath, commandName + ".exe"));
        }
    }
}
