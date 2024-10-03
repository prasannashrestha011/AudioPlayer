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
using System.Windows;

namespace AudioPlayer.LocalStorage
{
    public class AudioLocalStorage
    {
        public static event Action? PlayListDataChanged;
        public static void SavePlayListToLocalStorage(string key,PlayListStruct audioObj)
        {
            Debug.WriteLine(key);
            Debug.WriteLine(audioObj.AudioName);
            ObservableCollection<PlayListStruct> existingData=new ObservableCollection<PlayListStruct>(); 
            if (audioObj == null && string.IsNullOrEmpty(audioObj.AudioName) && string.IsNullOrEmpty(audioObj.AudioPath) )
            {
                MessageBox.Show("element cannot be empty", "Error");
                return;
            }
       
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication()) {
                
                if (isoStore.FileExists(key)) {
                    Debug.WriteLine("creating new work space...");
                    existingData = new ObservableCollection<PlayListStruct>();
                    using(IsolatedStorageFileStream isoFileStream=new IsolatedStorageFileStream(key, FileMode.Open, isoStore))
                    {
                        using (StreamReader reader = new StreamReader(isoFileStream)) {
                           
                            var exisitingParsedData=reader.ReadToEnd();
                            if (string.IsNullOrEmpty(exisitingParsedData) || string.IsNullOrWhiteSpace(exisitingParsedData)) MessageBox.Show("probably null");
                           
                            existingData = JsonConvert.DeserializeObject<ObservableCollection<PlayListStruct>>(exisitingParsedData);
                         
                        }

                    }
                    //appending new data to existing list
                    if (existingData != null && !existingData.Any(item => item.AudioName.Equals(audioObj.AudioName)))
                    {

                        existingData.Add(audioObj);


                    }
                    else
                    {
                        MessageBox.Show("audio item already exists on your playlist");
                    }                   
                }

                var parsedData = JsonConvert.SerializeObject(existingData);
                using (IsolatedStorageFileStream isoStream=new IsolatedStorageFileStream(key, FileMode.Create, isoStore))
                    {
                        
                        using (StreamWriter writer = new StreamWriter(isoStream)) {

                            
                           
                           
                            writer.WriteLine(parsedData);
                            Debug.WriteLine("new audio list added...");
                        }
                    }
                
                OnAudioListChanged();
            }
        
        }
        public static ObservableCollection<PlayListStruct> GetPlayListLocally(string key)
        {
            using(IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(key))
                {
                    using(IsolatedStorageFileStream isoStream=new IsolatedStorageFileStream(key, FileMode.Open, isoStore))
                    {
                        using(StreamReader reader=new StreamReader(isoStream))
                        {
                            var parsedData = reader.ReadToEnd();
                           
                            try
                            {
                                var data = JsonConvert.DeserializeObject<ObservableCollection<PlayListStruct>>(parsedData);
                                
                                return data;
                            }
                            catch (Exception err)
                            {
                                Debug.WriteLine(err.Message);
                                return null;

                            }
                      
                        }
                    }
                }
                return null;
            }
        }

        public static void DeleteAudio(string key,string audioName)
        {
            ObservableCollection<PlayListStruct> existingList;
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(key)) {
                    using (IsolatedStorageFileStream isoFileStream = new IsolatedStorageFileStream(key, FileMode.Open, isoStore))
                    {
                        using (StreamReader reader = new StreamReader(isoFileStream))
                        {
                            var parsedData = reader.ReadToEnd();
                            existingList = JsonConvert.DeserializeObject<ObservableCollection<PlayListStruct>>(parsedData);

                        }

                    }
                    var deletingData = existingList.FirstOrDefault(item => item.AudioName.Equals(audioName));
                    if (deletingData != null)
                    {
                        existingList.Remove(deletingData);
                    }
                    if (existingList.Count == 0) {
                     isoStore.DeleteFile(key);
                        Debug.WriteLine("deleted the files");
                        OnAudioListChanged();
                        return;
                    }

                    Debug.WriteLine("item deleted");
                    
                    using (IsolatedStorageFileStream isoFileStream = new IsolatedStorageFileStream(key, FileMode.Create, isoStore))
                    {
                        using (StreamWriter writer = new StreamWriter(isoFileStream))
                        {
                            var parsedData = JsonConvert.SerializeObject(existingList);
                            writer.Write(parsedData);


                        }

                    }
                    OnAudioListChanged();

                }
            }
        }

        public static void OnAudioListChanged()
        {
            PlayListDataChanged?.Invoke();
        }
    }
}
