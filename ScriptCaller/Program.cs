using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ScriptCaller
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Call();
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

        private static void Call()
        {
            var cmd = CommandLine.Create();
            var fileName = Path.GetFileNameWithoutExtension(cmd.ExePath);
            var scriptName = GetScriptName(fileName);
            CallProcess(scriptName, cmd.Arguments);
        }

        private static string GetScriptName(string fileName)
        {
            var configName = fileName + ".config";
            if (File.Exists(configName))
            {
                var config = ScriptConfig.CreateFromFile(configName);
                return config.ScriptPath;
            }

            if (Directory.Exists(fileName))
            {
                var files = Directory.EnumerateFiles(fileName).ToArray();
                var dPath =
                    files.FirstOrDefault(z =>
                        string.Equals(Path.GetFileNameWithoutExtension(z), fileName,
                        StringComparison.OrdinalIgnoreCase)) ??
                    files.FirstOrDefault(z =>
                        string.Equals(Path.GetFileNameWithoutExtension(z), "main",
                        StringComparison.OrdinalIgnoreCase));
                if (dPath != null) return dPath;
            }

            throw new InternalException("missing script path.");
        }

        private static void CallProcess(string fileName, string arguments)
        {
            var info = new ProcessStartInfo();
            info.FileName = fileName;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.Arguments = arguments;
            try
            {
                using (var p = Process.Start(info))
                {
                    if (p == null) return;
                    Console.WriteLine(p.StandardOutput.ReadToEnd());
                    p.WaitForExit();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("unknown command.");
            }
        }
    }
}
