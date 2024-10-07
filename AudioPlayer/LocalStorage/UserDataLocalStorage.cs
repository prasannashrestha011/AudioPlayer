using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AudioPlayer.LocalStorage
{
    public class UserDataLocalStorage
    {
        public static void SaveUserRootPath(string key, string value)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetMachineStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(key, FileMode.Create, isoStore))
                {
                    using (StreamWriter writer = new StreamWriter(isoStream))
                    {
                        writer.Write(value);
                        Debug.WriteLine("saved......");
                    }
                }
            }
           
        }
        public static string LoadUserRootPath(string key)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetMachineStoreForApplication())
            {
                if (isoStore.FileExists(key))
                {
                    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(key, FileMode.Open, isoStore))
                    {
                        using (StreamReader reader = new StreamReader(isoStream))
                        {
                            Debug.WriteLine("loading....");
                            return reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    return null;
                }

            }
        }
    }
}
