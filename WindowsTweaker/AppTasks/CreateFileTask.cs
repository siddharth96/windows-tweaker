using System;
using System.Collections.Generic;
using System.IO;
using File = System.IO.File;

namespace WindowsTweaker.AppTasks {

    internal static class CreateFileTask {

        internal static Dictionary<String, long> SizeDataDict = new Dictionary<string, long>() {
            {"1 MB", 1048576},
            {"100 MB", 104857600},
            {"1 GB", 1073741824},
            {"10 GB", 10737418240},
            {"1 TB", 1099511627776},
            {"10 TB", 10995116277760}
        };

        internal static void Create(string filePath, long sizeInBytes, Message message, MainWindow window) {
            try {
                if (File.Exists(filePath)) {
                    File.Delete(filePath);
                }
                ProcessWrapper.ExecuteDosCmd(String.Format("fsutil file createnew \"{0}\" {1}",
                        filePath, sizeInBytes));
                message.Success(String.Format("{0} \"{1}\"", window.FindResource("SuccessfullyCreated"), filePath));
                return;
            } catch (IOException) { } catch (UnauthorizedAccessException) { }
            message.Error(String.Format("{0} \"{1}\"", window.FindResource("FailedToCreate"), filePath));
        }
    }
}
