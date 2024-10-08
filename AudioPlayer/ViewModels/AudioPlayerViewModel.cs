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
using System;
using System.Threading;
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
        private bool isAutoPlayMode;
        public bool IsAutoPlayMode
        {
            get => isAutoPlayMode;
            set
            {
                isAutoPlayMode = value;
                OnPropertyChanged(nameof(IsAutoPlayMode));
                UserDataLocalStorage.SaveAutoPlayModeState("autoPlayModeState.txt", IsAutoPlayMode.ToString());
                Debug.WriteLine($"{IsAutoPlayMode.ToString()} is your current autoplay state");
            }
        }
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
        public ICommand PlayAudioCommand => new RelayCommandBase(canExecute => true, execute => PlaySelectedAudio());
        public ICommand PauseAudioCommand=>new RelayCommandBase(canExecute=> true, execute => PauseSelectedAudio());
        public ICommand OpenFileCmd => new RelayCommandBase(canExecute => true, execute => OpenFileHandler());

        public ICommand FastForwardCmd => new RelayCommandBase(canExecute => true, execute => FastForward());
        public ICommand BackForwardCmd => new RelayCommandBase(canExecute => true, execute => BackAudio());
        public ICommand PlayNextAudioCmd => new RelayCommandBase(canExecute => true, execute => PlayNextAudio());
        public ICommand PlayPrevAudioCmd => new RelayCommandBase(canExecute => true, execute => PlayPrevAudio());
        public ICommand EnableSliderCursor => new RelayCommandBase(canExecute => { return AudioSlider != null && AudioPlayer != null; }, execute => AudioSlider.EnableCursorMovement());
        public ICommand MoveSliderCursor => new RelayCommandBase(canExecute => { return AudioSlider != null && AudioPlayer!=null; }, execute => AudioSlider.MoveSliderCursor());
        public ICommand DisableSliderCursor => new RelayCommandBase(canExecute => { return AudioSlider != null && AudioPlayer != null; }, execute => AudioSlider.DisableCursorMovement());
      

        //volumeManager
        public ICommand EnableVolumeCmd => new RelayCommandBase(canExecute => true, execute => EnableVolumeSlider());
        public ICommand ToggleVolumeCmd => new RelayCommandBase(canExecute => true, execute => ChangeVolumeLvl());
        public ICommand DisableVolumeCmd => new RelayCommandBase(canExecute => true, execute => DisableVolumeSlider());

        private int currentIdx = TreeViewModel.SelectedFileIndex;
        public AudioPlayerViewModel(MainNavigationStore mainNavStore)
        {
           
            this.mainNavStore = mainNavStore;
            TreeViewModel.StaticPropertyChanged += OnFileSelected;
            LoadedFileList.LoadedAudioListChanged += LoadedFolderFiles;
            AudioSlider = new ProgressSlider(0, 1, "0:0", false, false, AudioPlayer);
            IsAutoPlayMode = UserDataLocalStorage.LoadAutoPlayModeState("autoPlayModeState.txt");
            Debug.WriteLine($"{IsAutoPlayMode.ToString()} is your current autoplay state");
        }
        // get the folder files from user's dir
        private async void LoadedFolderFiles()
        {

            AudioPlayer.MediaEnded -= Media_Ended;
            AudioPlayer.MediaEnded += Media_Ended;
            if (LoadedFileList.LoadedAudioList != null && LoadedFileList.LoadedAudioList.SubFolder.Count > 0 && LoadedFileList.LoadedAudioList.SubFolder[TreeViewModel.SelectedFileIndex] is Files audioFile)
            {

                SelectedFileName = audioFile.FileName;
                SelectedFilePath = audioFile.FilePath;
                AudioPlayer.Open(new Uri(SelectedFilePath));
                await WaitForMediaOpened();
            }
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
                   
                    AudioPlayer.Play();
                    AudioSlider.IsPlaying = true;
                    AudioSlider.audioDispatcherTime.Start();//for updating the time span string (1 sec at a time)
                    AudioSlider.EscalatePointer();// for moving the thumb in slider 
                }
                else
                {
                    Debug.WriteLine("not ready"); 
                }
            }
            catch(Exception ex)
            {
               Debug.WriteLine(ex.Message);
                
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
        private void PlayPrevAudio()
        {
            if (LoadedFileList.LoadedAudioList.SubFolder.Count > 0)
            {
                if (TreeViewModel.SelectedFileIndex > 0)
                {
                    TreeViewModel.SelectedFileIndex--;
                }
                else
                {
                    // If we're at the first track, wrap around to the last track
                    TreeViewModel.SelectedFileIndex = 0;
                }

                Debug.WriteLine($"index->{TreeViewModel.SelectedFileIndex} after");
                ChangeAndPlay(TreeViewModel.SelectedFileIndex);
            }
        }
        private void PlayNextAudio()
        {
            Debug.WriteLine($"index->{TreeViewModel.SelectedFileIndex} before");
            if (TreeViewModel.SelectedFileIndex <LoadedFileList.LoadedAudioList.SubFolder.Count)
            {
                TreeViewModel.SelectedFileIndex++;
                Debug.WriteLine($"index->{TreeViewModel.SelectedFileIndex} after");
                ChangeAndPlay(TreeViewModel.SelectedFileIndex);
            }
            else
            {
                ExitCurrentAudio();
                TreeViewModel.SelectedFileIndex = 0;
                AudioPlayer.MediaEnded -= Media_Ended;
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

     
        //triggered whenever user selects the file from treeView
        private async void OnFileSelected()
        {

            Debug.WriteLine($"current volume lvl: {VolumeLvl}");

            AudioSlider.IsPlaying = false;
            SelectedFileName = TreeViewModel.SelectedFile.FileName;//get the selected file from the view control
           SelectedFilePath = TreeViewModel.SelectedFile.FilePath;
            AudioPlayer.Open(new Uri(SelectedFilePath));
            await WaitForMediaOpened();
          
        }


        private Task WaitForMediaOpened()//use to delay the action till we get the audio details from GetDuration func
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
     
        //for playing next or previous audio
        private async void ChangeAndPlay(int targetIdx)
        {

       

          
            AudioPlayer.Pause();
           
          if(targetIdx >= 0&& targetIdx < LoadedFileList.LoadedAudioList.SubFolder.Count)
            {
                var audiofile = LoadedFileList.LoadedAudioList.SubFolder[TreeViewModel.SelectedFileIndex];
      
                if (targetIdx >= 0 && targetIdx < LoadedFileList.LoadedAudioList.SubFolder.Count)
                {
                    var audioFile = LoadedFileList.LoadedAudioList.SubFolder[targetIdx] as Files;
                    var initialFile= LoadedFileList.LoadedAudioList.SubFolder[0]as Files;
                    Debug.WriteLine("\n");
               
                    if (audioFile != null)
                    {
                       
                        SelectedFileName = audioFile.FileName;
                        SelectedFilePath = audioFile.FilePath;
                        PlayInQueue();
                    }
                }
            }
        }

     
        private void Media_Ended(object sender, EventArgs e)
        {
            if (IsAutoPlayMode)
            {
                TreeViewModel.SelectedFileIndex++;
                Debug.WriteLine($"Autio play: {TreeViewModel.SelectedFileIndex} ");
                if (TreeViewModel.SelectedFileIndex < LoadedFileList.LoadedAudioList.SubFolder.Count)
                {
                    PlayInQueue();
                }
                else
                {
                    
                    TreeViewModel.SelectedFileIndex = 0;
                    AudioPlayer.Close();
                }
            }
            else
            {
                Debug.WriteLine("auto play off");
            }
        }
        private async void PlayInQueue()
        {
           var currentAudio= LoadedFileList.LoadedAudioList.SubFolder[TreeViewModel.SelectedFileIndex] as Files;

           
            if (LoadedFileList.LoadedAudioList.SubFolder[TreeViewModel.SelectedFileIndex] !=null && LoadedFileList.LoadedAudioList.SubFolder[TreeViewModel.SelectedFileIndex] is Files audioFile)
            {

                SelectedFileName = audioFile.FileName;
                SelectedFilePath = audioFile.FilePath;
                AudioPlayer.Open(new Uri(SelectedFilePath));
                await WaitForMediaOpened();
                PlaySelectedAudio();
            }
            else
            {
                Debug.WriteLine("not working/////////////////");
            }
            
        }

        //volumeManager
        private void EnableVolumeSlider()
        {
            IsVolumeManagerEnb = true;
        }
        private void ChangeVolumeLvl()
        {
            AudioPlayer.Volume=Math.Floor(VolumeLvl) /100;
            Debug.WriteLine(Math.Floor(VolumeLvl));
        }
        private void DisableVolumeSlider()
        {
            IsVolumeManagerEnb = false;
        }
        private void CheckVolumeRange()
        {
            if (Math.Floor(VolumeLvl) == 0)
            {
                VolumeRange = "mute";
            }
            if (1<Math.Floor(VolumeLvl) && Math.Floor(VolumeLvl) < 49) {
                VolumeRange = "small"; 
            }
            else if (50< Math.Floor(VolumeLvl)&& Math.Floor(VolumeLvl)<79)
            {
                VolumeRange = "medium";
            }
            else if(80<Math.Floor(VolumeLvl) && Math.Floor(VolumeLvl)<100)
            {
                VolumeRange = "large";
            }
        }
    }
}
