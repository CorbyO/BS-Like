using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Corby.Frameworks
{
    public static class Localization
    {
        private static readonly string DefaultLanguage = "en";
        private static readonly string LocalePath = "Locales";

        public static string CurrentLocale { get; private set; } = string.Empty;
        private static Dictionary<string, string> _localization;
        
        // get localization file list
        public static IReadOnlyList<string> GetLocales()
        {
            var localizationFiles = new List<string>();
            BetterStreamingAssets.Initialize();
            

            if (BetterStreamingAssets.DirectoryExists(LocalePath))
            {
                var sb = new StringBuilder(256);
                sb.AppendLine("Locales:");
                var files = BetterStreamingAssets.GetFiles(LocalePath);
                foreach (var file in files)
                {
                    if (!file.EndsWith(".json")) continue;
                    
                    localizationFiles.Add(file);
                    sb.AppendLine(file);
                }
                Dbug.Log(typeof(Localization), sb.ToString(), 1);
            }
            else
            {
                Dbug.Warning(typeof(Localization), "Localization folder not found", 1);
            }

            return localizationFiles;
        }
        
        public static void Load(string locale)
        {
            CurrentLocale = locale;
            _localization.Clear();
            
            var localizationFiles = GetLocales();
            foreach (var file in localizationFiles)
            {
                if (Path.GetFileName(file) != locale) continue;
                
                var json = BetterStreamingAssets.ReadAllText(file);
                _localization = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                
                return;
            }
            
            Dbug.Warning(typeof(Localization), $"Localization file not found: {locale}", 1);
        }
    }
}