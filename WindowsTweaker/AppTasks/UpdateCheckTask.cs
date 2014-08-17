using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using WindowsTweaker.Models;

namespace WindowsTweaker.AppTasks {
    internal static class UpdateCheckTask {

        internal const string Manual = "Manual";
        internal const string Auto = "Auto";
        internal const long UpdateInterval = 300; // 5 minutes

        internal static bool IsTimeToCheck(long lastUpdateChkVal) {
            if (lastUpdateChkVal <= 0) {
                return true;
            }
            long ticks = DateTime.UtcNow.Ticks;
            ticks /= 10000000; // Convert to seconds
            long interval = ticks - lastUpdateChkVal;
            return interval < UpdateInterval;
        }

        internal static string GetUpdateInfoFile() {
            using (WebClient webClient = new WebClient()) {
                try {
                    byte[] responseBytes = webClient.DownloadData(Keys.UpdateApiUrl);
                    string response = Encoding.UTF8.GetString(responseBytes);
                    return response;
                } catch (ArgumentException) { } catch (WebException) { } catch (InvalidOperationException) { }
            }
            return null;
        }

        internal static TweakerUpdate ReadTweakerUpdateInfo(String xmlData) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TweakerUpdate));
            try {
                TweakerUpdate tweakerUpdate = (TweakerUpdate) xmlSerializer.Deserialize(
                    new XmlTextReader(new StringReader(xmlData)));
                return tweakerUpdate;
            } catch (IOException) { } 
            return null;
        }
    }
}
