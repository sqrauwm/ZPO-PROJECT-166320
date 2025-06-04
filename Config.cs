using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using WardrobeApp.Exceptions;

namespace WardrobeApp
{
    public sealed class Config
    {
        private static readonly Lazy<Config> _lazyInstance = new Lazy<Config>(() => LoadConfig());
        public static Config Instance => _lazyInstance.Value;

        public string DataFilePath { get; set; }
        public string WindowTitle { get; set; }

        private Config() { }    

        private static Config LoadConfig() 
        {   
            try
            {
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                string configPath = Path.Combine(exePath, "config.json");
                if (!File.Exists(configPath))
                    throw new DataLoadException($"Nie znaleziono pliku konfiguracyjnego: {configPath}");

                string json = File.ReadAllText(configPath);
                var cfg = JsonSerializer.Deserialize<Config>(json);
                if (cfg == null)
                    throw new DataLoadException("Deserializacja pliku konfiguracyjnego zwróciła null. ");

                return cfg;
            }
            catch (Exception ex) when (!(ex is DataLoadException))
            {
                throw new DataLoadException("Błąd podczas wczytywania konfiguracji. ", ex);
            }
        }
    }
}
