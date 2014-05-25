using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using WindowsTweaker.Models;

namespace WindowsTweaker.AppTasks {
    internal static class UpdateCheckTask {

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
