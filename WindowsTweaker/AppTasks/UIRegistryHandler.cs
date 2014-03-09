using System;
using System.Windows.Controls;
using WindowsTweaker.Models;
using Microsoft.Win32;

namespace WindowsTweaker.AppTasks {

    internal static class UiRegistryHandler {

        /// <summary>
        /// Updates the checked state of the <paramref name="chk"/> if the <paramref name="valueName"/>
        /// evaluates to true (i.e., 1 (REG_DWORD or REG_QWORD)) for the passed in <paramref name="registryKey"/>. 
        /// However, if <paramref name="inverse"/> is to true, it updates the checkbox state in reverse manner, i.e., 
        /// if the <paramref name="valueName"/> evaluates to true (i.e., 1), checkbox is set to unchecked.
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        /// <param name="inverse"></param>
        internal static void SetUiCheckBoxFromRegistryValue(CheckBox chk, RegistryKey registryKey, 
            string valueName, bool inverse=false) {
            int? val = (int?)registryKey.GetValue(valueName);
            chk.IsChecked = inverse ? Utils.ReversedIntToBool(val) : Utils.IntToBool(val);
        }

        /// <summary>
        /// Updates the checked state of the <paramref name="chk"/> if the <paramref name="valueName"/>
        /// evaluates to true (i.e., '1' (REG_SZ)) for the passed in <paramref name="registryKey"/>. 
        /// However, if <paramref name="inverse"/> is to true, it updates the checkbox state in reverse manner, i.e., 
        /// if the <paramref name="valueName"/> evaluates to true (i.e., 1), checkbox is set to unchecked.
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        /// <param name="inverse"></param>
        internal static void SetUiCheckBoxFromStringRegistryValue(CheckBox chk, RegistryKey registryKey,
            string valueName, bool inverse = false) {
            string val = (string) registryKey.GetValue(valueName);
            chk.IsChecked = inverse ? Utils.ReversedStringToBool(val) : Utils.StringToBool(val);
        }

        /// <summary>
        /// Updates the checked state of the <paramref name="chk"/> if the
        /// <paramref name="registryKey"/> has <paramref name="keyName"/> as sub-key
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        internal static void SetUiCheckBoxFromRegistryKey(CheckBox chk, RegistryKey registryKey, string keyName) {
            RegistryKey key = registryKey.OpenSubKey(keyName);
            if (key != null) {
                chk.IsChecked = true;
                key.Close();
            }
        }

        /// <summary>
        /// Sets the value of <paramref name="keyName"/> in <paramref name="registryKey"/> to 1 (REG_DWORD) if
        /// <paramref name="chk"/>'s IsChecked is equal to true, otherwise sets it to 0 (REG_DWORD).
        /// This behaviour is reversed if <paramref name="inverse"/> is set to true.
        /// Each <paramref name="chk" /> has a tag associated with it, which when set to 1 (HasUserInteracted)
        /// will then be saved to registry, otherwise it'll be ignored.
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        /// <param name="inverse"></param>
        internal static void SetRegistryValueFromUiCheckBox(CheckBox chk, RegistryKey registryKey, string keyName, bool inverse=false) {
            if ((chk.Tag as Byte?) == Constants.HasUserInteracted) {
                SetRegistryValueFromBool(chk.IsChecked, registryKey, keyName, inverse);
            }
        }

        /// <summary>
        /// Sets the value of <paramref name="keyName"/> in <paramref name="registryKey"/> to '1' (REG_SZ) if
        /// <paramref name="chk"/>'s IsChecked is equal to true, otherwise sets it to '0' (REG_SZ).
        /// This behaviour is reversed if <paramref name="inverse"/> is set to true.
        /// Each <paramref name="chk" /> has a tag associated with it, which when set to 1 (HasUserInteracted)
        /// will then be saved to registry, otherwise it'll be ignored.
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        /// <param name="inverse"></param>
        public static void SetStringRegistryValueFromUiCheckBox(CheckBox chk, RegistryKey registryKey, string keyName,
            bool inverse = false) {
            if ((chk.Tag as Byte?) == Constants.HasUserInteracted) {
                string val = inverse ? Utils.ReversedBoolToString(chk.IsChecked) : Utils.BoolToString(chk.IsChecked);
                registryKey.SetValue(keyName, val);
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
        internal static void SetRegistryKeyFromUiCheckBox(CheckBox chk, RegistryKey registryKey, string keyName) {
            if ((chk.Tag as Byte?) == Constants.HasUserInteracted) {
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
        internal static void SetRegistryKeyFromBool(bool? val, RegistryKey registryKey, string keyName) {
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
        internal static void SetRegistryValueFromBool(bool? val, RegistryKey registryKey, string keyName, bool inverse=false) {
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
        internal static void SetUiTextBoxFromRegistryValue(TextBox txt, RegistryKey registryKey, string valueName) {
            string val = (string) registryKey.GetValue(valueName);
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
        internal static void SetRegistryValueFromUiTextBox(TextBox txt, RegistryKey registryKey, string valueName) {
            string txtVal = txt.Text.Trim();
            registryKey.SetValue(valueName, txtVal);
        }

        /// <summary>
        /// Updates the value of <paramref name="valueName"/>
        /// in <paramref name="registryKey"/> from the password present in <paramref name="passwordBox"/>
        /// </summary>
        /// <param name="passwordBox"></param>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        internal static void SetRegistryValueFromUiPasswordBox(PasswordBox passwordBox, RegistryKey registryKey,
            string valueName) {
            registryKey.SetValue(valueName, passwordBox.Password);
        }

        /// <summary>
        /// Updates the text of the <paramref name="txtPasswd"/> if the <paramref name="valueName"/>
        /// has a some password for the passed in <paramref name="registryKey"/>
        /// </summary>
        /// <param name="txtPasswd"></param>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        internal static void SetUiPasswordBoxFromRegistryValue(PasswordBox txtPasswd, RegistryKey registryKey, string valueName) {
            string val = (string) registryKey.GetValue(valueName);
            if (val != null) {
                txtPasswd.Password = val;
            }
        }
    }
}