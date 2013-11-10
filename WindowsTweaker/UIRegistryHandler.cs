using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WindowsTweaker {
    static class UIRegistryHandler {
        public static void SetUICheckBoxFromRegistryValue(CheckBox chk, RegistryKey registryKey, String valueName) {
            int? val = (int?)registryKey.GetValue(valueName);
            chk.IsChecked = Utils.ToBoolean(val);
        }

        public static void SetUICheckBoxFromRegistryKey(CheckBox chk, RegistryKey registryKey, String keyName) {
            RegistryKey key = registryKey.OpenSubKey(keyName);
            chk.IsChecked = key != null;
        }

    }
}
