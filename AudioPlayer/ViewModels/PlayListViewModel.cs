using AudioPlayer.LocalStorage;
using AudioPlayer.ModelBase;
using AudioPlayer.RelayBase;
using AudioPlayer.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AudioPlayer.ViewModels
{
    public class PlayListViewModel : ViewModelBase
    {
        private PlaylistItem playList;
        public PlaylistItem PlayList
        {
            get => playList;
            set
            {
                playList = value;
                OnPropertyChanged(nameof(PlayList));
            }
        }

        public PlayListViewModel()
        {
            string root = @"D:\testfolder";
            var dirInfo = new DirectoryInfo(root);
            PlayList = CreateFileTree(dirInfo);
        }
        public PlaylistItem CreateFileTree(DirectoryInfo directiory)
        {
            var directioryItem = new PlaylistItem
            {
                FolderName = directiory.Name,
                FolderPath = directiory.FullName

            };
            foreach (var dir in directiory.GetDirectories())
            {
                directioryItem.Children.Add(CreateFileTree(dir));
            }
            foreach (var file in directiory.GetFiles())
            {
                directioryItem.Children.Add(new PlayListFileItem
                {
                    FileName = file.Name,
                    FolderPath = file.FullName
                });
            }
            return directioryItem;
        }


    }
}
