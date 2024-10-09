using AudioPlayer.LocalStorage;
using AudioPlayer.ModelBase;
using AudioPlayer.RelayBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace AudioPlayer.Components
{
    public class VolumeManager : ViewModelBase
    {
        private bool isVolumeManagerEnb;
        public bool IsVolumeManagerEnb
        {
            get => isVolumeManagerEnb;
            set
            {
                isVolumeManagerEnb = value;
                OnPropertyChanged();
            }
        }
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
        private string volumeRange;
        public string VolumeRange
        {
            get => volumeRange;
            set
            {
                volumeRange = value;
                OnPropertyChanged();

            }
        }
        private MediaPlayer audioPlayer;
        public MediaPlayer AudioPlayer
        {
            get => audioPlayer;
            set
            {
                audioPlayer = value;
                OnPropertyChanged(nameof(AudioPlayer));
            }
        }
        public ICommand EnableVolumeCmd => new RelayCommandBase(canExecute => true, execute => EnableVolumeSlider());
        public ICommand ToggleVolumeCmd => new RelayCommandBase(canExecute => true, execute => ChangeVolumeLvl());
        public ICommand DisableVolumeCmd => new RelayCommandBase(canExecute => true, execute => DisableVolumeSlider());
        public VolumeManager(MediaPlayer audioPlayer)
        {
            VolumeLvl= double.TryParse(UserDataLocalStorage.LoadAudioVolumeState("audioVolumeState.txt"), out var lvl) ? lvl : 50;
            CheckVolumeRange();
            AudioPlayer = audioPlayer;
        }
        public void EnableVolumeSlider()
        {
            IsVolumeManagerEnb = true;
        }
        public void ChangeVolumeLvl()
        {

            AudioPlayer.Volume = Math.Floor(VolumeLvl) / 100;
            UserDataLocalStorage.SaveAudioVolumeState("audioVolumeState.txt",VolumeLvl.ToString());
        }
        public void DisableVolumeSlider()
        {
            IsVolumeManagerEnb = false;
        }
        public void CheckVolumeRange()
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
