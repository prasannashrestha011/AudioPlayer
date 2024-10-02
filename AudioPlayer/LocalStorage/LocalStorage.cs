using AudioPlayer.Structure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer.LocalStorage
{
    public class LocalStorage
    {
        public static void SavePlayListToLocalStorage(string key,ObservableCollection<PlayListStruct> audioList)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication()) {
                if (isoStore.FileExists(key)) { 
                    var existingdata=new ObservableCollection<PlayListStruct>();
                    using(IsolatedStorageFileStream isoFileStream=new IsolatedStorageFileStream(key, FileMode.Open, isoStore))
                    {
                        using (StreamReader reader = new StreamReader(isoFileStream)) { 
                            var parsedData=reader.ReadToEnd();
                            existingdata=JsonConvert.DeserializeObject<ObservableCollection<PlayListStruct>>(parsedData);
                        }

                    }

                    //appending new data to existing list
                    if (existingdata != null)
                    {
                        foreach (var audio in audioList)
                        {
                            existingdata.Add(audio);
                        }
                    }
                    
                    using(IsolatedStorageFileStream isoFileStream=new IsolatedStorageFileStream(key, FileMode.Open, isoStore))
                    {
                        using(StreamWriter writer=new StreamWriter(isoFileStream))
                        {
                            var parsedData = JsonConvert.SerializeObject(existingdata);
                            writer.WriteLine(parsedData);
                            Debug.WriteLine("new data added to existing list");
                        }
                    }
                }
                else
                {
                    using(IsolatedStorageFileStream isoStream=new IsolatedStorageFileStream(key, FileMode.Open, isoStore))
                    {
                        using (StreamWriter writer = new StreamWriter(isoStream)) {
                            var parsedData = JsonConvert.SerializeObject(audioList);
                            writer.WriteLine(parsedData);
                            Debug.WriteLine("new audio list added...");
                        }
                    }
                }
            
            }
        }
    }
}
