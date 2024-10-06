using AudioPlayer.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AudioPlayer.Utilities
{
    public class LoadedFileList
    {
        private static RootFolder loadedAudioList = new RootFolder();
        public static RootFolder LoadedAudioList
        {
            get => loadedAudioList;
            set
            {
                loadedAudioList = value;
                OnLoadedAudioListChanged();
            }
        }
        public static event Action? LoadedAudioListChanged;
        public static void OnLoadedAudioListChanged()
        {
            LoadedAudioListChanged?.Invoke();

        }
    }
}
