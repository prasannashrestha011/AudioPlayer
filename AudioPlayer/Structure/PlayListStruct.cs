using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer.Structure
{
    public class PlayListStruct
    {
        public string AudioName { get; set; }
        public string AudioPath { get; set; }
        public PlayListStruct(string audioName, string audioPath)
        {
            AudioName = audioName;
            AudioPath = audioPath;
        }
    }
}
