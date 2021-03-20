using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace quoteblok2net.BotSystem
{
    public static class ConfigManager
    {
        public static readonly Config config = new Config();
        public static readonly bool loaded;
        private static readonly String _configFile = "./config.json";
        private static FileStream _file;
        private static Config _newConfig;

        private static readonly JsonSerializer Serializer = new JsonSerializer {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        static ConfigManager() {
            loaded = File.Exists(_configFile) ? LoadFromJson() : CreateJson();
            if (loaded) {
                config = new Config(_newConfig);
            }            
        }

        private static bool LoadFromJson() {
            try {
                using( StreamReader sr = new StreamReader(_configFile)) {
                    using (JsonReader reader = new JsonTextReader(sr)) {
                        _newConfig = Serializer.Deserialize<Config>(reader);
                    }
                }
            }
            catch( Exception ex ) {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);

                return false;
            }
            return true;
        }

        private static bool CreateJson() {
            try {
                using( StreamWriter sw = new StreamWriter(_configFile)) {
                    using (JsonWriter writer = new JsonTextWriter(sw)) {
                        Serializer.Serialize(writer, config);
                    }
                }
            }
            catch( Exception ex ) {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public static bool WriteToJson(bool doLoading=false) {
            try {
                using( StreamWriter sw = new StreamWriter(_configFile)) {
                    using (JsonWriter writer = new JsonTextWriter(sw)) {
                        Serializer.Serialize(writer, config);
                    }
                }
            }
            catch( Exception ex ) {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);


                return false;
            }

            if( doLoading ) {
                LoadFromJson();
            }

            return true;
        }

    }
}
