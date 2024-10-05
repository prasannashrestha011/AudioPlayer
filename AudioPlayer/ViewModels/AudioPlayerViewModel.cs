using AudioPlayer.ModelBase;
using AudioPlayer.NavigationStores;
using AudioPlayer.RelayBase;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using AudioPlayer.Utilities;
using AudioPlayer.Components;
using System.Diagnostics;
using System.Windows.Threading;
using AudioPlayer.Structure;
using AudioPlayer.LocalStorage;
namespace AudioPlayer.ViewModels
{
    public class AudioPlayerViewModel : ViewModelBase
    {
        public MainNavigationStore mainNavStore { get; set; }
        private string selectedFilePath;
        public string SelectedFilePath
        {
            get { return selectedFilePath; }
            set
            {
                selectedFilePath = value;
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }
        private string selectedFileName;
        public string SelectedFileName
        {
            get { return selectedFileName; }
            set
            {
                selectedFileName = value;
                OnPropertyChanged(nameof(SelectedFileName));
            }
        }
        private MediaPlayer audioPlayer=new MediaPlayer();
        public MediaPlayer AudioPlayer
        {
            get { return audioPlayer; }
            set
            {
                audioPlayer = value;
                OnPropertyChanged(nameof(AudioPlayer));
            }
        }
        private bool isPlaying;
        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
                Debug.WriteLine($"{IsPlaying} has changed");
            }
        }
     
        private ProgressSlider audioSlider;
        public ProgressSlider AudioSlider
        {
            get => audioSlider;
            set
            {
                audioSlider = value;
                OnPropertyChanged(nameof (AudioSlider));
            }
        }
        
        public ICommand PlayAudioCommand => new RelayCommandBase(canExecute => true, execute => PlaySelectedAudio());
        public ICommand PauseAudioCommand=>new RelayCommandBase(canExecute=> true, execute => PauseSelectedAudio());
        public ICommand OpenFileCmd => new RelayCommandBase(canExecute => true, execute => OpenFileHandler());

        public ICommand FastForwardCmd => new RelayCommandBase(canExecute => true, execute => FastForward());
        public ICommand BackForwardCmd => new RelayCommandBase(canExecute => true, execute => BackAudio());

        public ICommand EnableSliderCursor => new RelayCommandBase(canExecute => { return AudioSlider != null && AudioPlayer != null; }, execute => AudioSlider.EnableCursorMovement());
        public ICommand MoveSliderCursor => new RelayCommandBase(canExecute => { return AudioSlider != null && AudioPlayer!=null; }, execute => AudioSlider.MoveSliderCursor());
        public ICommand DisableSliderCursor => new RelayCommandBase(canExecute => { return AudioSlider != null && AudioPlayer != null; }, execute => AudioSlider.DisableCursorMovement());
        
        public AudioPlayerViewModel(MainNavigationStore mainNavStore)
        {
           
            this.mainNavStore = mainNavStore;
            TreeViewModel.StaticPropertyChanged += OnFileSelected;
            LoadedFileList.LoadedAudioListChanged += LoadedFolderFiles;
        }
        
        public void OpenFileHandler()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.ShowDialog();

                var selectedFilePath = openFileDialog.FileName;
                SelectedFilePath = selectedFilePath;
               
                SelectedFileName = Path.GetFileName(selectedFilePath);
                PlayListStruct playListStruct = new PlayListStruct(SelectedFileName, SelectedFilePath);
                
                AudioPlayer.MediaOpened += GetAudioDuration;

                AudioPlayer.Open(new Uri(SelectedFilePath));
                IsPlaying = false;
            }
            catch (Exception err)
            {
                return;
            }
        }
        public void PlaySelectedAudio()
        {
        
            try
            {
                if (!AudioSlider.IsPlaying&&AudioPlayer!=null && AudioPlayer.HasAudio)
                {
                    Debug.WriteLine("working....");
                    AudioPlayer.Play();
                    AudioSlider.IsPlaying = true;
                    AudioSlider.audioDispatcherTime.Start();
                    AudioSlider.EscalatePointer();
                }
                else
                {
                    Debug.WriteLine("not ready"); 
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(SelectedFilePath, "Please select the audio file to play");
                
            }
        }
       
        public void PauseSelectedAudio()
        {
            if (AudioSlider.IsPlaying)
            {
                AudioSlider.IsPlaying = false;
                AudioPlayer.Pause();
                AudioSlider.audioDispatcherTime.Stop();
               AudioSlider.StopPointer();
            }
            
        }
        public void BackAudio()
        {
            AudioPlayer.Position -= TimeSpan.FromSeconds(10);
            AudioSlider.CurrentPosition -= 10;
        }
        public void FastForward()
        {
            AudioPlayer.Position += TimeSpan.FromSeconds(10);
            AudioSlider.CurrentPosition += 10;
        }




        public  void GetAudioDuration(object sender, EventArgs e)
        {
            
            if (AudioPlayer != null && AudioPlayer.NaturalDuration.HasTimeSpan)
            {
                var audioDuration = AudioPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                var timeSpan = Utils.FormatTime(audioDuration);
              
                AudioSlider = new ProgressSlider(0, audioDuration, timeSpan  , false, false, AudioPlayer);
           
                AudioSlider.UpdateTimeSpanLeft();

                AudioSlider.timer.Elapsed += AudioSlider.MoveThumb;
                   
                

            }
            else
            {
                MessageBox.Show("no time duration provided");
            }
        }

        public void ExitCurrentAudio()
        {
            if (AudioPlayer.Position.Equals(TimeSpan.FromSeconds(AudioSlider.AudioDuration)))
            {
                AudioPlayer.Close();
                AudioSlider.IsPlaying = false;
            }
        }

        private async void OnFileSelected()
        {
            AudioSlider.IsPlaying = false;
           SelectedFileName=TreeViewModel.SelectedFile.FileName;
           SelectedFilePath=TreeViewModel.SelectedFile.FilePath;
            AudioPlayer.Open(new Uri(SelectedFilePath));
            await WaitForMediaOpened();
           PlaySelectedAudio();

        }


        private Task WaitForMediaOpened()
        {
            var tcs = new TaskCompletionSource<bool>();

   
            void OnMediaOpened(object sender, EventArgs e)
            {
                AudioPlayer.MediaOpened -= OnMediaOpened;
                tcs.SetResult(true);
                GetAudioDuration(sender, e);
            }
            AudioPlayer.MediaOpened += OnMediaOpened;
            return tcs.Task;
        }
        private int currentIdx = 0;
        private async void LoadedFolderFiles()
        {
            
            AudioPlayer.MediaEnded -= Media_Ended;
            AudioPlayer.MediaEnded += Media_Ended;
            if (LoadedFileList.LoadedAudioList != null && LoadedFileList.LoadedAudioList.Count>0)
            {
                SelectedFileName = LoadedFileList.LoadedAudioList[currentIdx].FileName;
                SelectedFilePath = LoadedFileList.LoadedAudioList[currentIdx].FilePath;
                AudioPlayer.Open(new Uri(SelectedFilePath));
                await WaitForMediaOpened();
            }
        }
        private void Media_Ended(object sender, EventArgs e)
        {
            currentIdx++;
            if (currentIdx < LoadedFileList.LoadedAudioList.Count)
            {
                PlayInQueue();
            }
        }
        private async void PlayInQueue()
        {
            SelectedFileName = LoadedFileList.LoadedAudioList[currentIdx].FileName;
            SelectedFilePath = LoadedFileList.LoadedAudioList[currentIdx].FilePath;
            AudioPlayer.Open(new Uri(SelectedFilePath));
            await WaitForMediaOpened();
            PlaySelectedAudio();
        }
    }
}
