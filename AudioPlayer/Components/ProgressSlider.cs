using AudioPlayer.ModelBase;
using AudioPlayer.Utilities;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace AudioPlayer.Components
{
    public class ProgressSlider:ViewModelBase
    {
        public System.Timers.Timer timer;
        public DispatcherTimer audioDispatcherTime;

        private double currentPosition;
        public double CurrentPosition
        {
            get => currentPosition;
            set
            {
                currentPosition = value;
               OnPropertyChanged(nameof(CurrentPosition));
            }
        }
        private string audioTimeSpanStr;
        public string AudioTimeSpanStr
        {
            get => audioTimeSpanStr;
            set
            {
                audioTimeSpanStr = value;
                OnPropertyChanged(nameof (AudioTimeSpanStr));
            }
        }
        private double audioDuration;
        public double AudioDuration
        {
            get => audioDuration;
            set
            {
                audioDuration = value;
                OnPropertyChanged(nameof (AudioDuration));
            }
        }

        private bool isPlaying;
        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
                Debug.WriteLine($"{IsPlaying} has changed from progress slider");
            }
        }
        private bool isMoving;
        public bool IsMoving
        {
            get
            {
                return isMoving;
            }
            set
            {
                isMoving = value;
                OnPropertyChanged(nameof(IsMoving));
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
        private int counter = 0;
        public ProgressSlider(double currentPosition, double audioDuration,string audioTimeSpanStr, bool isMoving, bool isPlaying,MediaPlayer audioPlayer)
        {
            CurrentPosition = currentPosition;

            AudioDuration = audioDuration;
           

            AudioTimeSpanStr = audioTimeSpanStr;
            IsMoving = isMoving;
             IsPlaying = isPlaying;
            AudioPlayer = audioPlayer;
            timer = new System.Timers.Timer(1000);
        }
        
        public void EscalatePointer()
        {

      
            
            timer.Start();
        }
        public void StopPointer()
        {
            timer.Stop();
        }
        public void EnableCursorMovement()
        {
            IsMoving = true;
        
        }
        public void UpdateTimeSpanLeft()
        {
             audioDispatcherTime = new DispatcherTimer();
            audioDispatcherTime.Interval = TimeSpan.FromSeconds(1);
            Debug.WriteLine(IsMoving);
            if (!IsMoving)
            {



                audioDispatcherTime.Tick += UpdateAudioTimeSpanStr;
               
            }


        }
        public async void MoveSliderCursor()
        {
            
            if (IsMoving)
            {
                AudioPlayer.Position = TimeSpan.FromSeconds(CurrentPosition);
                
                var currentTimeSpan = AudioDuration - CurrentPosition;
                AudioTimeSpanStr = Utils.FormatTime(currentTimeSpan);
                
            }
            else
            {
                return;
            }
        }
        public void DisableCursorMovement()
        {
            IsMoving = false;
        }
        public void MoveThumb(object sender, EventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (CurrentPosition < AudioDuration)
                    {
                        CurrentPosition += 1;
                    
                     
                    }
                    else
                    {
                        IsPlaying = false;
                        CurrentPosition = 0;
                   
                        audioDispatcherTime.Stop();
                        timer.Stop();
               
                       
                    }

                });
            }
            catch (Exception err)
            {

                AudioPlayer.Close();
            }
        }
        public void UpdateAudioTimeSpanStr(object sender, EventArgs e)
        {
            var TimeSpan = AudioDuration;
            if (TimeSpan>0 )
            {
                TimeSpan -= CurrentPosition;
                AudioTimeSpanStr=Utils.FormatTime(TimeSpan);
            }
        }
    }
}
