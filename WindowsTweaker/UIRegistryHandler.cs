using Microsoft.Win32;
using System;
using System.Windows.Controls;

namespace WindowsTweaker {

    internal static class UIRegistryHandler {

        /// <summary>
        /// Updates the checked state of the <paramref name="chk"/> if the <paramref name="valueName"/>
        /// evaluates to true (i.e., 1) for the passed in <paramref name="registryKey"/>. 
        /// However, if <paramref name="inverse"/> is to true, it updates the reverses the checkbox state, i.e., 
        /// if the <paramref name="valueName"/> evaluates to true (i.e., 1), checkbox is set to unchecked.
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        /// <param name="inverse"></param>
        public static void SetUICheckBoxFromRegistryValue(CheckBox chk, RegistryKey registryKey, 
            String valueName, bool inverse=false) {
            int? val = (int?)registryKey.GetValue(valueName);
            bool result = Utils.IntToBool(val);
            chk.IsChecked = inverse ? !result : result;
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
        /// This behaviour is reversed if <paramref name="inverse"/> is set to true.
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        /// <param name="inverse"></param>
        public static void SetRegistryValueFromUICheckBox(CheckBox chk, RegistryKey registryKey, String keyName, bool inverse=false) {
            SetRegistryValueFromBool(chk.IsChecked, registryKey, keyName, inverse);
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
        /// <paramref name="val"/> is equal to true, otherwise it deletes it (if present). This behaviour is reversed if 
        /// <paramref name="inverse"/> is true.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        /// <param name="inverse"></param>
        public static void SetRegistryValueFromBool(bool? val, RegistryKey registryKey, String keyName, bool inverse=false) {
            int intVal = inverse ? Utils.ReversedBoolToInt(val) : Utils.BoolToInt(val);
            registryKey.SetValue(keyName, intVal);
        }

        /// <summary>
        /// Updates the text of the <paramref name="txt"/> if the <paramref name="valueName"/>
        /// has a some text for the passed in <paramref name="registryKey"/>
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        public static void SetUITextBoxFromRegistryValue(TextBox txt, RegistryKey registryKey, String valueName) {
            String val = (String)registryKey.GetValue(valueName);
            if (val != null) {
                val = val.Trim();
                txt.Text = val;
            }
        }

        /// <summary>
        /// Updates the value of <paramref name="valueName"/>
        /// in <paramref name="registryKey"/> from the text present in <paramref name="txt"/>
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        public static void SetRegistryValueFromUITextBox(TextBox txt, RegistryKey registryKey, String valueName) {
            String txtVal = txt.Text.Trim();
            registryKey.SetValue(valueName, txtVal);
        }
    }
}