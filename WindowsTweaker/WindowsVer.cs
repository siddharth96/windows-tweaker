using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTweaker
{
    public class WindowsVer
    {
        public enum Windows { Other=4, XP, Vista, Seven, Eight, Blue };

        private WindowsVer() { }

        private static readonly WindowsVer instance = new WindowsVer();

        public static WindowsVer Instance
        {
            get
            {
                return instance;
            }
        }

        public Windows GetName() {
            Version ver = Environment.OSVersion.Version;
            if (ver.Major == 5 && ver.Minor == 1)
                return Windows.XP;
            else if (ver.Major == 6)
            {
                switch (ver.Minor)
                {
                    case 0:
                        return Windows.Vista;
                    case 1:
                        return Windows.Seven;
                    case 2:
                        return Windows.Eight;
                    case 3:
                        return Windows.Blue;
                }
            }
            return Windows.Other;
        }
    }
}
