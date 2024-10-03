using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer.Structure
{
    public class RootFolder
    {
        public string FolderName { get; set; }
        public string FolderPath { get; set; }
        public ObservableCollection<RootFolder> SubFolder { get; set; } = new ObservableCollection<RootFolder>();
        public RootFolder()
        {

        }
    }
    public class Files : RootFolder
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
