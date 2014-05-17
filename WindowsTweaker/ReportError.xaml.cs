using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;
using WindowsTweaker.AppTasks;

namespace WindowsTweaker {
    /// <summary>
    /// Interaction logic for ReportError.xaml
    /// </summary>
    public partial class ReportError : Window {
        public ReportError(Exception exceptionObj) {
            InitializeComponent();
            this._exceptionObj = exceptionObj;
            this._reportErrorBackgroundWorker = (BackgroundWorker) this.FindResource("reportErrorBackgroundWorker");
            txtErrorMsg.Text = GetDisplayMsg();
        }

        private readonly Exception _exceptionObj;
        private readonly BackgroundWorker _reportErrorBackgroundWorker;

        private void OnButtonCancelClick(object sender, RoutedEventArgs e) {
            if (_reportErrorBackgroundWorker.IsBusy) 
                _reportErrorBackgroundWorker.CancelAsync();
            this.Close();
        }

        private void OnReportErrorButtonClick(object sender, RoutedEventArgs e) {
            if (!_reportErrorBackgroundWorker.IsBusy) {
                progressBar.Visibility = Visibility.Visible;
                _reportErrorBackgroundWorker.RunWorkerAsync();
            }
        }

        internal class ErrorReportResponse {
            // ReSharper disable once InconsistentNaming
            public bool success { get; set; }
        }

        private string GetDisplayMsg() {
            return this.FindResource("SomethingWrong") + Environment.NewLine +
                   this.FindResource("NoPersonalInfo") + Environment.NewLine +
                   this.FindResource("AllWeNeed") + Environment.NewLine +
                   this.FindResource("WillExit") + Environment.NewLine;
        }

        internal bool SendErrorReport() {
            if (_exceptionObj == null) return false;
            String osName = WindowsVer.AsString(WindowsVer.Instance.GetName());
            String architecture = WindowsVer.Is64BitOs() ? "x64" : "x86";
            String exceptionMsg = _exceptionObj.Message;
            String exceptionStackTrace = _exceptionObj.ToString();
            String appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            using (WebClient webClient = new WebClient()) {
                NameValueCollection nameValueCollection = new NameValueCollection {
                    {"key", Keys.ErrorApiKey},
                    {"os_name", WebUtility.UrlEncode(osName)},
                    {"architecture", architecture},
                    {"exception_msg", WebUtility.UrlEncode(exceptionMsg)},
                    {"exception_traceback", WebUtility.UrlEncode(exceptionStackTrace)},
                    {"app_version", appVersion}
                };
                try {
                    byte[] responseBytes = webClient.UploadValues(Keys.ErrorApiUrl, "POST", nameValueCollection);
                    string response = Encoding.UTF8.GetString(responseBytes);
                    ErrorReportResponse errorReportResponse = 
                        new JavaScriptSerializer().Deserialize<ErrorReportResponse>(response);
                    return errorReportResponse != null && errorReportResponse.success;
                }
                catch (ArgumentException) {
                    return false;
                }
                catch (WebException) {
                    return false;
                }
                catch (InvalidOperationException) {
                    return false;
                }
            }
        }

        private void ShowNetworkErrorDialog() {
            MessageBox.Show(this.FindResource("UnableToReport") + Environment.NewLine +
                            this.FindResource("BadBadInternet") + Environment.NewLine +
                            this.FindResource("ThanksForTime"), this.FindResource("UhOh").ToString(),
                            MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void OnReportErrorWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            progressBar.Visibility = Visibility.Collapsed;
            if (e.Cancelled) return;
            if (!((bool) e.Result)) {
                ShowNetworkErrorDialog();
            }
            this.Close();
        }

        private void OnReportErrorDoWork(object sender, DoWorkEventArgs e) {
            e.Result = SendErrorReport();
        }
    }
}
