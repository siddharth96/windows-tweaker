using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using WindowsTweaker.Models;

namespace WindowsTweaker.AppTasks {
    internal static class ConfigHandler {

        internal enum Language { English, German, Russian, French }

        private static Config _config;

        internal static Config GetConfig() {
            if (_config == null) {
                _config = ReadConfig();
            }
            return _config ?? (_config = new Config() {UpdateMethod = UpdateCheckTask.Auto});
        }

        internal static Language GetCurrentLanguage() {
            switch (Thread.CurrentThread.CurrentUICulture.Name) {
                case "en-US":
                    return Language.English;
                case "de-DE":
                    return Language.German;
                case "ru-RU":
                    return Language.Russian;
                case "fr-FR":
                    return Language.French;
                default:
                    return Language.English;
            }
        }

        internal static string ToLanguageString(Language language) {
            switch (language) {
                case Language.English:
                    return "en-US";
                case Language.German:
                    return "de-DE";
                case Language.Russian:
                    return "ru-RU";
                case Language.French:
                    return "fr-FR";
                default:
                    return "en-US";
            }
        }

        internal static void SetCulture(string cultureName) {
            _config = GetConfig();
            if (_config.CultureName == cultureName) return;
            _config.CultureName = cultureName;

            UpdateConfig();
        }

        internal static void SetLastUpdateChkVal() {
            _config = GetConfig();
            long ticks = DateTime.UtcNow.Ticks;
            ticks /= 10000000; // Convert to seconds
            _config.LastUpdateChk = ticks;
            UpdateConfig();
        }

        internal static void SetUpdateChkMethod(string methodName) {
            _config = GetConfig();
            if (_config.UpdateMethod == methodName) return;
            _config.UpdateMethod = methodName;
            UpdateConfig();
        }

        private static void UpdateConfig() {
            string configFile = Utils.GetConfigFilePath();
            if (configFile == null)
                return;
            try {
                XmlSerializer writer = new XmlSerializer(typeof(Config));
                StreamWriter streamWriter = new StreamWriter(configFile);
                writer.Serialize(streamWriter, _config);
                streamWriter.Close();
            } catch (FileNotFoundException) { } catch (IOException) { } catch (InvalidOperationException) { }
        }

        private static Config ReadConfig() {
            string configFile = Utils.GetConfigFilePath();
            if (configFile == null || !File.Exists(configFile))
                return null;
            XmlSerializer reader = new XmlSerializer(typeof(Config));
            try {
                StreamReader streamReader = new StreamReader(configFile);
                Config config = (Config) reader.Deserialize(streamReader);
                if (config.UpdateMethod == null) {
                    config.UpdateMethod = UpdateCheckTask.Auto;
                }
                streamReader.Close();
                return config;
            }
            catch (FileNotFoundException) {}
            catch (IOException) {}
            catch (InvalidOperationException)  {
                try { File.Delete(configFile); } catch (IOException) { } catch (UnauthorizedAccessException) { }
            }
            return null;
        }
    }
}
