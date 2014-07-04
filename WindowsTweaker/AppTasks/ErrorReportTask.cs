using System;

namespace WindowsTweaker.AppTasks {
    internal static class ErrorReportTask {
        internal static void ReportError(Exception exceptionObj) {
            if (exceptionObj == null) return;
            ReportError reportError = new ReportError(exceptionObj);
            reportError.ShowDialog();
        }
    }
}
