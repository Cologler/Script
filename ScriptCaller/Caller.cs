using System;
using System.Diagnostics;
using System.IO;

namespace ScriptCaller
{
    public static class Caller
    {
        public static void Call(string fileName, string arguments)
        {
            var info = new ProcessStartInfo();
            info.FileName = fileName;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.Arguments = arguments;
            try
            {
                using (var p = new Process())
                {
                    p.StartInfo = info;
                    p.OutputDataReceived += (sender, a) => Console.WriteLine(a.Data);
                    p.Start();
                    p.BeginOutputReadLine();
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