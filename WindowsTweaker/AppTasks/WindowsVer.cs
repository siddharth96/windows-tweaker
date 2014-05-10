using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowsTweaker.AppTasks {

    internal class WindowsVer {
        private static Windows? _cachedName;

        public enum Windows { 
            Other = 4,
            Xp, 
            Vista,
            Seven,
            Eight,
            Blue 
        };

        public static string AsString(Windows windows) {
            switch (windows) {
                case Windows.Xp:
                    return "Windows XP";
                case Windows.Vista:
                    return "Windows Vista";
                case Windows.Seven:
                    return "Windows 7";
                case Windows.Eight:
                    return "Windows 8";
                case Windows.Blue:
                    return "Windows 8.1";
                default:
                    return "older than XP";
            }
        }

        private WindowsVer() {
        }

        private static readonly WindowsVer instance = new WindowsVer();

        public static WindowsVer Instance {
            get {
                return instance;
            }
        }

        public Windows GetName() {
            if (_cachedName.HasValue)
                return _cachedName.Value;
            _cachedName = _getName();
            return _cachedName.Value;
        }

        private static Windows _getName() {
            Version ver = Environment.OSVersion.Version;
            if (ver.Major == 5 && ver.Minor == 1)
                return Windows.Xp;
            if (ver.Major != 6) return Windows.Other;
            switch (ver.Minor) {
                case 0:
                    return Windows.Vista;

                case 1:
                    return Windows.Seven;

                case 2:
                    return Windows.Eight;

                case 3:
                    return Windows.Blue;
            }
            return Windows.Other;
        }

        // http://stackoverflow.com/a/336729/1293716
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        private static bool InternalCheckIsWow64() {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6) {
                using (Process p = Process.GetCurrentProcess()) {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal)) {
                        return false;
                    }
                    return retVal;
                }
            } else {
                return false;
            }
        }

        internal static bool Is64BitOs() {
            return IntPtr.Size == 8 || InternalCheckIsWow64();
        }
    }
}