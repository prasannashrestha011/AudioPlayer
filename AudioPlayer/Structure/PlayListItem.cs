using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer.Structure
{
    public class PlaylistItem
    {
        public string FolderName { get; set; } 
        public string FolderPath { get; set; } 
        public ObservableCollection<PlaylistItem> Children { get; set; } = new ObservableCollection<PlaylistItem>(); // Children for subfolders/files
       
    }

    public class PlayListFileItem:PlaylistItem
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

}
