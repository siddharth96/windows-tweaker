using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace WindowsTweaker {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        private void SetUiCulture(string cultureName = "en-US") {
            try {
                CultureInfo ci = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ci;
            } catch (CultureNotFoundException) {
                
            }
        }

        private void SetLanguageDictionary() {
            List<ResourceDictionary> resourceDictionaries = new List<ResourceDictionary>();
            foreach (ResourceDictionary mergedDictionary in this.Resources.MergedDictionaries) {
                resourceDictionaries.Add(mergedDictionary);
            }
            string culture = Thread.CurrentThread.CurrentCulture.ToString();
            string lang = culture.Contains("-") ? culture.Split('-')[0] : culture;
            string requestedCulture = String.Format("Resources/Strings/StringResources.{0}.xaml", lang);
            ResourceDictionary resourceDictionary = resourceDictionaries.FirstOrDefault(x => x.Source.OriginalString == requestedCulture);
            if (resourceDictionary == null) {
                requestedCulture = "Resources/Strings/StringResources.xaml";
                resourceDictionary = resourceDictionaries.FirstOrDefault(x => x.Source.OriginalString == requestedCulture);
            }
            if (resourceDictionary != null) {
                this.Resources.MergedDictionaries.Remove(resourceDictionary);
                this.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }

        private void OnApplicationStartup(object sender, StartupEventArgs e) {
            //SetUiCulture("de-DE");
            //SetUiCulture("ru-RU");
            SetLanguageDictionary();
        }
    }
}