using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using WindowsTweaker.Models;

namespace WindowsTweaker.AppTasks {
    internal static class LocalizationHandler {

        internal enum Language { English, German, Russian }

        internal static Language GetCurrentLanguage() {
            switch (Thread.CurrentThread.CurrentUICulture.Name) {
                case "en-US":
                    return Language.English;
                case "de-DE":
                    return Language.German;
                case "ru-RU":
                    return Language.Russian;
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
                default:
                    return "en-US";
            }
        }

        internal static void UpdateCultureInConfig(string cultureName) {
            string configFile = Utils.GetConfigFilePath();
            if (configFile == null)
                return;
            try {
                Config config = new Config { CultureName = cultureName };
                XmlSerializer writer = new XmlSerializer(typeof(Config));
                StreamWriter streamWriter = new StreamWriter(configFile);
                writer.Serialize(streamWriter, config);
                streamWriter.Close();
            } catch (FileNotFoundException) { } catch (IOException) { } catch (InvalidOperationException) { }
        }

        internal static Config  ReadConfig() {
            string configFile = Utils.GetConfigFilePath();
            if (configFile == null || !File.Exists(configFile))
                return null;
            XmlSerializer reader = new XmlSerializer(typeof(Config));
            try {
                StreamReader streamReader = new StreamReader(configFile);
                Config config = (Config) reader.Deserialize(streamReader);
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
