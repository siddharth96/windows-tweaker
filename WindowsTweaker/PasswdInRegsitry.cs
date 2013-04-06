using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Security;
using System.Security.Cryptography;

namespace WindowsTweaker
{
    class PasswdInRegsitry
    {
        static RegistryKey hkcu;
        static RegistryKey hkcuSoftware;
        static RegistryKey hkcuSuRe;
        static RegistryKey hkcuW7T;
        public static void SetPassword(string passwd)
        {
            try
            {
                hkcu = Registry.CurrentUser;
                hkcuSoftware = hkcu.OpenSubKey("Software", true);
                hkcuSuRe = hkcuSoftware.CreateSubKey("SuRe");
                hkcuW7T = hkcuSuRe.CreateSubKey("W7T");
                hkcuW7T.SetValue("UserConfig", passwd);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Oh My God!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        public static void RemovePasswd()
        {
            try
            {
                hkcu = Registry.CurrentUser;
                hkcuSoftware = hkcu.OpenSubKey("Software", true);
                hkcuSuRe = hkcuSoftware.OpenSubKey("SuRe", true);
                if (hkcuSuRe == null)
                    return;
                hkcuW7T = hkcuSuRe.OpenSubKey("W7T", true);
                if (hkcuW7T == null)
                    return;
                string[] valueshkcuW7T = hkcuW7T.GetValueNames();
                if (keySearch(valueshkcuW7T, "UserConfig"))
                    hkcuW7T.DeleteValue("UserConfig");
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Oh My God!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public static string ReadPasswd()
        {
            hkcu = Registry.CurrentUser;
            hkcuSoftware = hkcu.OpenSubKey("Software", true);
            hkcuSuRe = hkcuSoftware.OpenSubKey("SuRe", true);
            if (hkcuSuRe == null)
                return null;
            hkcuW7T = hkcuSuRe.OpenSubKey("W7T");
            if (hkcuW7T == null)
                return null;
            string[] valueshkcuW7T = hkcuW7T.GetValueNames();
            if (keySearch(valueshkcuW7T, "UserConfig"))
            {
                string rv = (string)hkcuW7T.GetValue("UserConfig");
                if (rv.Equals("") || rv == null)
                    return null;
                else
                    return rv;
            }
            return null;
        }

        private static bool keySearch(string[] keyArr, string itm)
        {
            for (int i = 0; i < keyArr.Length; i++)
            {
                string tmp = keyArr[i];
                if (tmp.Equals(itm))
                    return true;
            }
            return false;
        }

        public static string HashPasswd(string passwd)
        {
            string salt = "passwdForW7T";
            byte[] bytSource;
            byte[] bytHash;
            bytSource = ASCIIEncoding.ASCII.GetBytes(salt + passwd + salt);
            bytHash = new MD5CryptoServiceProvider().ComputeHash(bytSource);
            return ByteToString(bytHash);
        }

        private static string ByteToString(byte[] bytHash)
        {
            StringBuilder sb = new StringBuilder(bytHash.Length);
            for (int i = 0; i < bytHash.Length; i++)
            {
                sb.Append(bytHash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
