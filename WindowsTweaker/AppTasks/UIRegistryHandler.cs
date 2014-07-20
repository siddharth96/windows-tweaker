using System;
using System.Windows.Controls;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;

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
        /// <param name="nullAsTrue"></param>
        internal static void SetCheckedState(this CheckBox chk, RegistryKey registryKey, 
            string valueName, bool inverse=false, bool nullAsTrue=false) {
            try {
                int? val = (int?) registryKey.GetValue(valueName);
                if (inverse && !val.HasValue && nullAsTrue) {
                    chk.IsChecked = true;
                    return;
                }
                chk.IsChecked = inverse ? Utils.ReversedIntToBool(val) : Utils.IntToBool(val);
            } catch (InvalidCastException) {
                registryKey.DeleteValue(valueName, false);
                chk.IsChecked = false;
            }
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
        internal static void SetCheckedStateFromString(this CheckBox chk, RegistryKey registryKey,
            string valueName, bool inverse = false) {
            try {
                string val = (string) registryKey.GetValue(valueName);
                chk.IsChecked = inverse ? Utils.ReversedStringToBool(val) : Utils.StringToBool(val);
            } catch (InvalidCastException) {
                registryKey.DeleteValue(valueName, false);
                chk.IsChecked = false;
            }
        }

        /// <summary>
        /// Updates the value of <paramref name="iud" /> from the <paramref name="valueName" /> present in
        /// <paramref name="registryKey"/>. (<paramref name="defaultValStr"/> is returned if <paramref name="valueName"/>
        /// is not in registry)
        /// When <paramref name="maxVal"/> and <paramref name="minVal" /> are specified, and the value in registry is not in the range
        /// then <paramref name="defaultValStr" /> is returned.
        /// </summary>
        /// <param name="iud"></param>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        /// <param name="defaultValStr"></param>
        /// <param name="maxVal"></param>
        /// <param name="minVal"></param>
        internal static void SetValueFromString(this IntegerUpDown iud, RegistryKey registryKey,
            string valueName, string defaultValStr, int? maxVal = null, int? minVal = null) {
            int defaultVal = Int32.Parse(defaultValStr);
            int iudVal;
            try {
                iudVal = Int32.Parse((string) registryKey.GetValue(valueName, defaultVal));
            }
            catch (FormatException) {
                iudVal = defaultVal;
            }
            catch (InvalidCastException) {
                string valStr = Utils.RepairAsStringFromInt(registryKey, valueName);
                iudVal = !String.IsNullOrEmpty(valStr) ? Int32.Parse(valStr) : defaultVal;
            }
            iud.Value = iudVal <= maxVal &&  iudVal >= minVal  ? iudVal : defaultVal;
        }

        /// <summary>
        /// Updates the checked state of the <paramref name="chk"/> if the
        /// <paramref name="registryKey"/> has <paramref name="keyName"/> as sub-key
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        internal static void SetCheckedStateFromSubKey(this CheckBox chk, RegistryKey registryKey, string keyName) {
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
        /// <param name="registryKey"></param>
        /// <param name="chk"></param>
        /// <param name="keyName"></param>
        /// <param name="inverse"></param>
        internal static void SetValue(this RegistryKey registryKey, CheckBox chk, string keyName, bool inverse = false) {
            if (chk.HasUserInteracted()) {
                registryKey.SetValue(keyName, chk.IsChecked, inverse);
            }
        }

        /// <summary>
        /// Sets the value of <paramref name="keyName"/> in <paramref name="registryKey"/> to '1' (REG_SZ) if
        /// <paramref name="chk"/>'s IsChecked is equal to true, otherwise sets it to '0' (REG_SZ).
        /// This behaviour is reversed if <paramref name="inverse"/> is set to true.
        /// Each <paramref name="chk" /> has a tag associated with it, which when set to 1 (HasUserInteracted)
        /// will then be saved to registry, otherwise it'll be ignored.
        /// </summary>
        /// <param name="registryKey"></param>
        /// <param name="chk"></param>
        /// <param name="keyName"></param>
        /// <param name="inverse"></param>
        public static void SetStringValue(this RegistryKey registryKey, CheckBox chk, string keyName,
            bool inverse = false) {
            if (chk.HasUserInteracted()) {
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
        /// <param name="registryKey"></param>
        /// <param name="chk"></param>
        /// <param name="keyName"></param>
        internal static void SetSubKey(this RegistryKey registryKey, CheckBox chk, string keyName) {
            if (chk.HasUserInteracted()) {
                registryKey.SetSubKey(keyName, chk.IsChecked);
            }
        }

        /// <summary>
        /// Creates a sub-key with name equal to <paramref name="subKeyName"/> in <paramref name="registryKey"/> if
        /// <paramref name="val"/> is equal to true, otherwise deletes it.
        /// </summary>
        /// <param name="registryKey"></param>
        /// <param name="subKeyName"></param>
        /// <param name="val"></param>
        internal static void SetSubKey(this RegistryKey registryKey, string subKeyName, bool? val) {
            RegistryKey newKey = registryKey.OpenSubKey(subKeyName, true);
            if (val == true && newKey == null) {
                registryKey.CreateSubKey(subKeyName);
            } else {
                if (newKey != null) {
                    registryKey.DeleteSubKeyTree(subKeyName, false);
                }
            }
            if (newKey != null) {
                newKey.Close();
            }
        }

        /// <summary>
        /// Sets the value of <paramref name="keyName"/> under <paramref name="registryKey"/> to 1 (REG_DWORD)
        /// if <paramref name="val"/> equal to true, otherwise it sets it to 0. This behaviour is reversed if 
        /// <paramref name="inverse"/> is true.
        /// </summary>
        /// <param name="registryKey"></param>
        /// <param name="keyName"></param>
        /// <param name="val"></param>
        /// <param name="inverse"></param>
        internal static void SetValue(this RegistryKey registryKey, string keyName, bool? val, bool inverse = false) {
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
        internal static void SetText(this TextBox txt, RegistryKey registryKey, string valueName) {
            string val = (string) registryKey.GetValue(valueName);
            if (!String.IsNullOrEmpty(val) && val != "\0") {
                val = val.Trim();
                txt.Text = val;
            }
        }

        /// <summary>
        /// Updates the value of <paramref name="valueName"/>
        /// in <paramref name="registryKey"/> from the text present in <paramref name="txt"/>
        /// </summary>
        /// <param name="registryKey"></param>
        /// <param name="txt"></param>
        /// <param name="valueName"></param>
        internal static void SetValue(this RegistryKey registryKey, TextBox txt, string valueName) {
            string txtVal = txt.Text.Trim();
            registryKey.SetValue(valueName, txtVal);
        }

        /// <summary>
        /// Updates the value of <paramref name="valueName"/>
        /// in <paramref name="registryKey"/> from the password present in <paramref name="passwordBox"/>
        /// </summary>
        /// <param name="registryKey"></param>
        /// <param name="passwordBox"></param>
        /// <param name="valueName"></param>
        internal static void SetValue(this RegistryKey registryKey, PasswordBox passwordBox,
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
        internal static void SetPassword(this PasswordBox txtPasswd, RegistryKey registryKey, string valueName) {
            string val = (string) registryKey.GetValue(valueName);
            if (val != null) {
                txtPasswd.Password = val;
            }
        }
    }
}