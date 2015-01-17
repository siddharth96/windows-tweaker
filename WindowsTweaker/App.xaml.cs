using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using WindowsTweaker.AppTasks;
using WindowsTweaker.Models;

namespace WindowsTweaker {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        private void OnApplicationStartup(object sender, StartupEventArgs e) {
            if (!String.IsNullOrEmpty(Keys.ErrorApiKey) && !String.IsNullOrEmpty(Keys.ErrorApiUrl)) {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            }
            DoLocalization();
            Splash splash = new Splash();
            splash.Show();
            new MainWindow().Show();
            splash.Close();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs) {
            ErrorReportTask.ReportError(unhandledExceptionEventArgs.ExceptionObject as Exception);
            if (unhandledExceptionEventArgs.IsTerminating) {
                Environment.Exit(1);
            }
        }

        private void DoLocalization() {
            Config config = ConfigHandler.GetConfig();
            if (config != null) {
                string cultureName = config.CultureName;
                if (cultureName != null) {
                    bool result = SetUiCulture(cultureName);
                    if (result) {
                        // If the culture name in config file is a valid culture-name, and not some garbage text
                        ConfigHandler.SetCulture(cultureName);
                    }
                }
            }
            SetLanguageDictionary();
        }

        private bool SetUiCulture(string cultureName = "en-US") {
            try {
                CultureInfo ci = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ci;
                return true;
            }
            catch (CultureNotFoundException) {
                string configFile = Utils.GetConfigFilePath();
                if (configFile != null && File.Exists(configFile)) {
                    try {
                        File.Delete(configFile);
                    } catch (IOException) { } catch (UnauthorizedAccessException) { }
                }
            }
            return false;
        }

        private void SetLanguageDictionary() {
            List<ResourceDictionary> resourceDictionaries = new List<ResourceDictionary>();
            foreach (ResourceDictionary mergedDictionary in this.Resources.MergedDictionaries) {
                resourceDictionaries.Add(mergedDictionary);
            }
            string culture = Thread.CurrentThread.CurrentUICulture.ToString();
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
    }
}