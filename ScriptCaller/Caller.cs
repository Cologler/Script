using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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
                    p.Start();
                    BeginReadOutput(p);
                    p.WaitForExit();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("unknown command.");
            }
        }

        public static void BeginReadOutput(Process p)
        {
            var reader = p.StandardOutput;
            Task.Run(() =>
            {
                try
                {
                    int readed;
                    while ((readed = reader.Read()) >= 0)
                    {
                        Console.Write((char)readed);
                    }
                }
                catch (Exception)
                {
                    return;
                }
            });
        }
    }
}