using System.IO;

namespace ScriptCallerRef
{
    public static class ScriptHelper
    {
        public static string EntryPath { get; }

        public static string EntryDirectory { get; }

        private const string ScriptsFolderName = "Scripts";

        public static string ScriptsDirectory { get; }

        static ScriptHelper()
        {
            EntryPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            EntryDirectory = Path.GetDirectoryName(EntryPath);
            ScriptsDirectory = Path.Combine(EntryDirectory, ScriptsFolderName);
        }
    }
}