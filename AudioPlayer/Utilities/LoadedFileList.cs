using AudioPlayer.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer.Utilities
{
    public class LoadedFileList
    {
        public static ObservableCollection<Files> LoadedAudioList=new ObservableCollection<Files>();
        public static event Action? LoadedAudioListChanged;
        public static void OnLoadedAudioListChanged()
        {
            LoadedAudioListChanged?.Invoke();
        }
    }
}
