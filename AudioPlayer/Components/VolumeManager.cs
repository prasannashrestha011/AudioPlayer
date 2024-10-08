using AudioPlayer.ModelBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AudioPlayer.Components
{
    public class VolumeManager : ViewModelBase
    {
        private Double volumeLvl;
        public Double VolumeLvl
        {
            get => volumeLvl;
            set
            {
                volumeLvl = value;
                OnPropertyChanged();
                CheckVolumeRange();
            }
        }
        private string volumeRange = "mute";
        public string VolumeRange
        {
            get => volumeRange;
            set
            {
                volumeRange = value;
                OnPropertyChanged();

            }
        }
        private void CheckVolumeRange()
        {
            if (Math.Floor(VolumeLvl) == 0)
            {
                VolumeRange = "mute";
            }
            if (1 < Math.Floor(VolumeLvl) && Math.Floor(VolumeLvl) < 49)
            {
                VolumeRange = "small";
            }
            else if (50 < Math.Floor(VolumeLvl) && Math.Floor(VolumeLvl) < 79)
            {
                VolumeRange = "medium";
            }
            else if (80 < Math.Floor(VolumeLvl) && Math.Floor(VolumeLvl) < 100)
            {
                VolumeRange = "large";
            }
        }
    }
    }
