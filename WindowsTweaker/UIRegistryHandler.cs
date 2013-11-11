using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WindowsTweaker {
    static class UIRegistryHandler {
        /// <summary>
        /// Updates the checked state of the <paramref name="chk"/> if the <paramref name="valueName"/> 
        /// evaluates to true (i.e., 1) for the passed in <paramref name="registryKey"/>
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        public static void SetUICheckBoxFromRegistryValue(CheckBox chk, RegistryKey registryKey, String valueName) {
            int? val = (int?)registryKey.GetValue(valueName);
            chk.IsChecked = Utils.ToBoolean(val);
        }

        /// <summary>
        /// Updates the checked state of the <paramref name="chk"/> if the 
        /// <paramref name="registryKey"/> has <paramref name="keyName"/> as sub-key 
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        public static void SetUICheckBoxFromRegistryKey(CheckBox chk, RegistryKey registryKey, String keyName) {
            RegistryKey key = registryKey.OpenSubKey(keyName);
            if (key != null) {
                chk.IsChecked = true;
                key.Close();
            }
            
            
        }

        /// <summary>
        /// Sets the value of <paramref name="keyName"/> in <paramref name="registryKey"/> to 1 if
        /// <paramref name="chk"/>'s IsChecked is equal to true, otherwise sets it to 0.
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        public static void SetRegistryValueFromUICheckBox(CheckBox chk, RegistryKey registryKey, String keyName) {
            SetRegistryValueFromBool(chk.IsChecked, registryKey, keyName);
        }

        /// <summary>
        /// Creates a sub-key with name (if it doesn't exists) <paramref name="keyName"/> under <paramref name="registryKey"/> if
        /// <paramref name="chk"/>'s IsChecked is equal to true, otherwise it deletes it (if present).
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        public static void SetRegistryKeyFromUICheckBox(CheckBox chk, RegistryKey registryKey, String keyName) {
            SetRegistryKeyFromBool(chk.IsChecked, registryKey, keyName);
        }

        /// <summary>
        /// Sets the value of <paramref name="keyName"/> in <paramref name="registryKey"/> to 1 if
        /// <paramref name="val"/> is equal to true, otherwise sets it to 0.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        public static void SetRegistryKeyFromBool(bool? val, RegistryKey registryKey, String keyName) {
            RegistryKey newKey = registryKey.OpenSubKey(keyName, true);
            if (val == true && newKey == null) {
                registryKey.CreateSubKey(keyName);
            } else {
                if (newKey != null) {
                    registryKey.DeleteSubKeyTree(keyName);
                }
            }
            if (newKey != null) {
                newKey.Close();
            }
        }

        /// <summary>
        /// Creates a sub-key with name (if it doesn't exists) <paramref name="keyName"/> under <paramref name="registryKey"/> if
        /// <paramref name="val"/> is equal to true, otherwise it deletes it (if present).
        /// </summary>
        /// <param name="val"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        public static void SetRegistryValueFromBool(bool? val, RegistryKey registryKey, String keyName) {
            int intVal = Utils.ToInteger(val);
            registryKey.SetValue(keyName, intVal);
        }
    }
}
