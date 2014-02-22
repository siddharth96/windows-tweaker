using System;
using System.Windows.Controls;
using WindowsTweaker.Models;
using Microsoft.Win32;

namespace WindowsTweaker.AppTasks {

    internal static class UIRegistryHandler {

        /// <summary>
        /// Updates the checked state of the <paramref name="chk"/> if the <paramref name="valueName"/>
        /// evaluates to true (i.e., 1) for the passed in <paramref name="registryKey"/>. 
        /// However, if <paramref name="inverse"/> is to true, it updates the checkbox state in reverse manner, i.e., 
        /// if the <paramref name="valueName"/> evaluates to true (i.e., 1), checkbox is set to unchecked.
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        /// <param name="inverse"></param>
        public static void SetUICheckBoxFromRegistryValue(CheckBox chk, RegistryKey registryKey, 
            string valueName, bool inverse=false) {
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
        public static void SetUICheckBoxFromRegistryKey(CheckBox chk, RegistryKey registryKey, string keyName) {
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
        /// Each <paramref name="chk" /> has a tag associated with it, which when set to 1 (HasUserInteracted)
        /// will then be saved to registry, otherwise it'll be ignored.
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        /// <param name="inverse"></param>
        public static void SetRegistryValueFromUICheckBox(CheckBox chk, RegistryKey registryKey, string keyName, bool inverse=false) {
            if (chk.Tag != null && (chk.Tag as Byte?) == Constants.HasUserInteracted) {
                SetRegistryValueFromBool(chk.IsChecked, registryKey, keyName, inverse);
            }
        }

        /// <summary>
        /// Creates a sub-key with name (if it doesn't exists) <paramref name="keyName"/> under <paramref name="registryKey"/> if
        /// <paramref name="chk"/>'s IsChecked is equal to true, otherwise it deletes it (if present).
        /// Each <paramref name="chk" /> has a tag associated with it, which when set to 1 (HasUserInteracted)
        /// will then be saved to registry, otherwise it'll be ignored.
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        public static void SetRegistryKeyFromUICheckBox(CheckBox chk, RegistryKey registryKey, string keyName) {
            if (chk.Tag != null && (chk.Tag as Byte?) == Constants.HasUserInteracted) {
                SetRegistryKeyFromBool(chk.IsChecked, registryKey, keyName);
            }
        }

        /// <summary>
        /// Sets the value of <paramref name="keyName"/> in <paramref name="registryKey"/> to 1 if
        /// <paramref name="val"/> is equal to true, otherwise sets it to 0.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        public static void SetRegistryKeyFromBool(bool? val, RegistryKey registryKey, string keyName) {
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
        public static void SetRegistryValueFromBool(bool? val, RegistryKey registryKey, string keyName, bool inverse=false) {
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
        public static void SetUITextBoxFromRegistryValue(TextBox txt, RegistryKey registryKey, string valueName) {
            string val = (string)registryKey.GetValue(valueName);
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
        public static void SetRegistryValueFromUITextBox(TextBox txt, RegistryKey registryKey, string valueName) {
            string txtVal = txt.Text.Trim();
            registryKey.SetValue(valueName, txtVal);
        }
    }
}