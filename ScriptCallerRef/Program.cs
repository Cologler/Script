using System;
using System.IO;
using System.Text;

namespace ScriptCallerRef
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Path.GetFileNameWithoutExtension(ScriptHelper.EntryPath) == nameof(ScriptCallerRef)) return;
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(Environment.CommandLine));
            Caller.Call(
                Path.Combine(Path.GetDirectoryName(ScriptHelper.EntryDirectory), "ScriptCaller.exe"),
                base64);
        }
    }
}
