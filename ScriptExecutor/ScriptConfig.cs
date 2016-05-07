using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScriptExecutor
{
    public class ScriptConfig
    {
        public string ScriptId { get; set; }

        public string Executor { get; set; }

        public string ScriptPath { get; set; }

        public IEnumerable<string> WriteToLines()
        {
            yield return $"{nameof(this.ScriptId)}={this.ScriptId}";
            yield return $"{nameof(this.Executor)}={this.Executor}";
            yield return $"{nameof(this.ScriptPath)}={this.ScriptPath}";
        }

        public void ReadFromLine(string line)
        {
            var keyValuePair = line.Split('=');
            if (keyValuePair.Length != 2) throw new InternalException($"config error on line: {line}");
            keyValuePair = keyValuePair.Select(z => z.Trim()).ToArray();

            switch (keyValuePair[0])
            {
                case nameof(this.ScriptPath):
                    this.ScriptPath = keyValuePair[1];
                    break;

                case nameof(this.Executor):
                    this.Executor = keyValuePair[1];
                    break;

                case nameof(this.ScriptId):
                    this.ScriptId = keyValuePair[1];
                    break;
            }
        }

        public bool Upgrade()
        {
            var upgrade = false;
            if (this.ScriptId == null)
            {
                this.ScriptId = Guid.NewGuid().ToString().ToUpper();
                upgrade = true;
            }
            return upgrade;
        }

        public void SaveToFile(string path) => File.WriteAllLines(path, this.WriteToLines());

        public static ScriptConfig CreateFromFile(string path)
        {
            var config = new ScriptConfig();
            using (var reader = File.OpenText(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    config.ReadFromLine(line);
                }
            }
            return config;
        }
    }
}