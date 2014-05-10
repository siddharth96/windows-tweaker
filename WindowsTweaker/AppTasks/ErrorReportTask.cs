using System;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;

namespace WindowsTweaker.AppTasks {
    internal static class ErrorReportTask {
        internal static void ReportError(Exception exceptionObj) {
            if (exceptionObj == null) return;
            ReportError reportError = new ReportError(exceptionObj);
            reportError.ShowDialog();
        }
    }
}
