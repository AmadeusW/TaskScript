using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScript.Wpf
{
    public static class Settings
    {
        public static dynamic Current { get; private set; }

        private const string SettingsPath = @"scripts.json";

        public static void Initialize()
        {
            try
            {
                var json = File.ReadAllText(SettingsPath);
                Current = JsonConvert.DeserializeObject(json);
            }
            catch { }
        }

        public static void Save()
        {
            try
            {
                //var json = JsonConvert.SerializeObject(LoadedScripts, Formatting.Indented);
                var json = JsonConvert.SerializeObject(Current);
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                // Notify.
            }   
        }
    }
}
