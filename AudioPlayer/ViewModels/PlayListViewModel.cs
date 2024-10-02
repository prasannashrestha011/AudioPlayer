using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer.ViewModels
{
    public class PlayListViewModel
    {
        public ObservableCollection<string> List { get; set; }
        public PlayListViewModel()
        {
            List = new ObservableCollection<string> { "hi", "hello", "nigga", "ashesh ko chak" };
        }
    }
}
