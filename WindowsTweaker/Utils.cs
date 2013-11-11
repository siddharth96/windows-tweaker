using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTweaker
{
    static class Utils
    {
        public static Func<int?, bool> ToBoolean = (int? val) => !(val == 0 || val == null);

        public static Func<bool?, int> ToInteger = (bool? val) => val == true ? 1 : 0;
    }
}
