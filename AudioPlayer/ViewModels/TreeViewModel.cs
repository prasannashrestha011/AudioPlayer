using AudioPlayer.ModelBase;
using AudioPlayer.RelayBase;
using AudioPlayer.Structure;
using AudioPlayer.Utilities;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Input;

namespace AudioPlayer.ViewModels
{
    public class TreeViewModel : ViewModelBase
    {
        private RootFolder folders;
        public RootFolder Folders
        {
            get => folders;
            set
            {
                folders = value;
                OnPropertyChanged(nameof(Folders));
                CollectLoadedFiles();
            }
        }
        private bool isfolderSelected;
        public bool IsFolderSelected
        {
            get => isfolderSelected;
            set
            {
                isfolderSelected = value;

                OnPropertyChanged(nameof(IsFolderSelected));
            }
        }
        private RootFolder selectedFolder;
        public RootFolder SelectedFolder
        {
            get => selectedFolder;
            set
            {
                if (selectedFolder != value)
                {
                    selectedFolder = value;
                    OnPropertyChanged(nameof(SelectedFolder));
                }
            }
        }
        //to play the selected audiofile from treeview model to audioplayerview model
        private static  Files selectedFile;
        public static Files SelectedFile
        {
            get => selectedFile;
            set
            {
                selectedFile = value;
                OnStaticPropertyChanged();
            
            }
        }
        //assist the audioplayerviewmodel for getting selected file idx from the parent folder(for autoplaying mode)
        private static int selectedFileIndex;
        public static int SelectedFileIndex
        {
            get => selectedFileIndex;
            set
            {
                selectedFileIndex = value;
                OnStaticPropertyChanged();
              
            }
        }
       
        public ICommand AddNewDir => new RelayCommandBase(canExecute => true, execute => CreateDir());
        public ICommand DisplayFileName => new RelayCommandBase(canExecute => true, execute => OnFileSelected(execute));

        public ICommand SelectedFileCmd=>new RelayCommandBase(canExecute=>true, execute => OnFileSelected(execute));
        public ICommand SelectedObj => new RelayCommandBase(canExecute => true, execute => DisplayBranch(execute));
        public TreeViewModel()
        {
            string rootFolder = @"D:\Music";
            var rootDir = new DirectoryInfo(rootFolder);
            Folders = CreateTree(rootDir);

            foreach (var folder in Folders.SubFolder)
            {
                Debug.WriteLine(folder.FolderName, " is your folders");
            }
        }
        public RootFolder CreateTree(DirectoryInfo rootDir)
        {
            var Folder = new RootFolder
            {
                FolderName = rootDir.Name,
                FolderPath = rootDir.FullName,
            };
            foreach (var subFolder in rootDir.GetDirectories())
            {
                Folder.SubFolder.Add(CreateTree(subFolder));
            }
            foreach (var file in rootDir.GetFiles("*.mp3"))
            {
                Folder.SubFolder.Add(new Files
                {
                    FileName = file.Name,
                    FilePath = file.FullName,

                });
            }
            return Folder;
        }
        //this function gets triggered whenever user select the audio file from treeview
        public void OnFileSelected(object parameter)
        {
            var fileInfo = parameter as Files;
            var ResultedFile = TriverseToFindSelectedFile(Folders, fileInfo);
            
            (SelectedFile, var SelectedFolder)= ResultedFile;
            //assigning the the dir that contains the selectedfile
            LoadedFileList.LoadedAudioList = SelectedFolder;
            //getting the file idx from parent folder for autoplaying mode
            SelectedFileIndex = SelectedFolder.SubFolder.IndexOf(SelectedFile);
        }
        //triverse and compare to obtain selected file from dir
        private (Files file,RootFolder folder) TriverseToFindSelectedFile(RootFolder currentDir,Files selectedFileName)
        {
     
            if(currentDir.SubFolder!= null)
            {
                for(int i = 0; i < currentDir.SubFolder.Count; i++)
                {
                    //iterate over each subfolder
                    var subFolder = currentDir.SubFolder[i];
                   
                    if(subFolder is Files targetFile && string.Equals(targetFile.FileName, selectedFileName.FileName, StringComparison.OrdinalIgnoreCase))
                    {
                        //obtain the target file and its dir for autoplaying mode
                        return (targetFile,currentDir);
                    }
                    else
                    { //triverse on children folder to compare the files
                        var result = TriverseToFindSelectedFile(subFolder, selectedFileName);
                        if (result.file != null)
                        {
                            MessageBox.Show($"{result.folder.FolderName}");
                            return result;
                        }
                    }
                }
            }
  
            return (null, null);
        }
        public void CreateDir()
        {
            if (IsFolderSelected)//for creating sub folder 
            {

                var folderInfo = SelectedFolder;
                string newfolderName = "New folder";
                string newfolderPath = Path.Combine(folderInfo.FolderPath, newfolderName);
                if (folderInfo != null)
                {
                    var newFolder = new RootFolder
                    {
                        FolderName = newfolderName,
                        FolderPath = newfolderPath,
                    };
                    folderInfo.SubFolder.Add(newFolder);
                    Directory.CreateDirectory(newfolderPath);
                }
                else
                {
                    Debug.WriteLine("null");
                }

            }
        }

        public void DisplayBranch(object parameter)
        {
            IsFolderSelected = true;
            SelectedFolder = parameter as RootFolder;
        
        }
        public void CollectLoadedFiles()//initial treeview
        {
            foreach(var file in Folders.SubFolder)
            {
                if (file != null && file is Files audioFile)
                {
                    LoadedFileList.LoadedAudioList.SubFolder.Add(audioFile);
                    LoadedFileList.OnLoadedAudioListChanged();
                }
            }
        }
    }
}
