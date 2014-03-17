using System;

namespace WindowsTweaker.AppTasks {

    internal class WindowsVer {
        public static readonly bool Is64BitMachine = IntPtr.Size == 8;
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
                    return "XP";
                case Windows.Vista:
                    return "Vista";
                case Windows.Seven:
                    return "7";
                case Windows.Eight:
                    return "8";
                case Windows.Blue:
                    return "8.1";
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
    }
}