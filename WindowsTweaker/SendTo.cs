using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace WindowsTweaker
{
    static class SendTo
    {
        public static ObservableCollection<ListItem> LoadSendToItems(string sendToPath)
        {
            ObservableCollection<ListItem> listSendTo = null;
            System.Windows.Media.ImageSource imgSource = null;
            string fileName = "";
            int index = 0;
            try
            {
                DirectoryInfo sendToDir = new DirectoryInfo(sendToPath);
                if (!sendToDir.Exists)
                    throw new FileNotFoundException();
                listSendTo = new ObservableCollection<ListItem>();
                foreach (FileInfo file in sendToDir.GetFiles())
                {
                    string ext = file.Extension;
                    if (ext.Equals(".ini"))
                        continue;
                    System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(file.FullName);
                    index = file.Name.LastIndexOf(ext);
                    fileName = file.Name.Remove(index);
                    imgSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, new System.Windows.Int32Rect(0, 0, icon.Width, icon.Height), BitmapSizeOptions.FromEmptyOptions());
                    listSendTo.Add(new ListItem(fileName, imgSource));
                }
                return listSendTo;
            }
            catch (NullReferenceException nes)
            {
                System.Windows.MessageBox.Show("Unable to read Values:\n" + nes.Message, "Oh my God!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return null;
            }
            catch (System.IO.FileNotFoundException fab)
            {
                System.Windows.MessageBox.Show(fab.Message + "\n\nDisabling Send To Customizations!", "Oh my God!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return null;
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message, "Oh my God!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return null;
            }
        }
    }
}
