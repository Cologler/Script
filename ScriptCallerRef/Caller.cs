using System;
using System.Diagnostics;
using System.IO;

namespace ScriptCallerRef
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