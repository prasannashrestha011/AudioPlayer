using AudioPlayer.ModelBase;
using AudioPlayer.RelayBase;
using AudioPlayer.Structure;
using AudioPlayer.Utilities;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using AudioPlayer.LocalStorage;
using AudioPlayer.Components;
using AudioPlayer.ViewComponents;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AudioPlayer.ViewModels
{
    public class TreeViewModel : ViewModelBase
    {
        private string defaultrootFolder;
        public string DefaultRootFolder
        {
            get => defaultrootFolder;
            set
            {
                defaultrootFolder = value;
                OnPropertyChanged();
            }
        }
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
        private bool isFolderRenaming;
        public bool IsFolderRenaming
        {
            get => isFolderRenaming;
            set
            {
                isFolderRenaming = value;
                OnPropertyChanged();
            }
        }
        private bool isPopUpOpen;
        public bool IsPopUpOpen
        {
            get => isPopUpOpen;
            set
            {
                isPopUpOpen = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddNewDir => new RelayCommandBase(canExecute => true, execute => CreateDir());
        public ICommand DisplayFileName => new RelayCommandBase(canExecute => true, execute => OnFileSelected(execute));
        public ICommand ChangeRootDirCmd => new RelayCommandBase(canExecute => true, execute => ChangeRootFolder());
        public ICommand SelectedFileCmd=>new RelayCommandBase(canExecute=>true, execute => OnFileSelected(execute));
        public ICommand SelectedObj => new RelayCommandBase(canExecute => true, execute => DisplayBranch(execute));
        public ICommand UnFocusValueCmd => new RelayCommandBase(canExecute => true, execute=> UnFocusValue());
        public ICommand RenameDirCmd => new RelayCommandBase(canExecute => true, execute => RenameDir());
        public ICommand DeleteDirCmd => new RelayCommandBase(canExecute => true, execute => DeleteDir());
        public ICommand EditFolderCmd => new RelayCommandBase(canExecute => true, execute => EditDir());
        public TreeViewModel()
        {
            
            DefaultRootFolder = UserDataLocalStorage.LoadUserRootPath("rootPath.txt") ?? $@"C:\Users\{Environment.UserName}\Music";
            LoadTreeView();
           
        }
        private void ChangeRootFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    
                    DefaultRootFolder = Path.GetFullPath(dialog.FileName);
                    UserDataLocalStorage.SaveUserRootPath("rootPath.txt", DefaultRootFolder);
                    LoadTreeView();
                }
            }


        }
        private void LoadTreeView()
        {
            var rootDir = new DirectoryInfo(DefaultRootFolder);
            Folders = CreateTree(rootDir);
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
                            
                            return result;
                        }
                    }
                }
            }
  
            return (null, null);
        }
        public void CreateDir()
        {
            DisablePopUp();
             AddfolderDialog inputDialog= new AddfolderDialog();
             inputDialog.ShowDialog();
            string newfolderName = inputDialog.FolderName;
            if (newfolderName == null) return;
            if (IsFolderSelected)//for creating sub folder 
            {

                var folderInfo = SelectedFolder;
               
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
                 
                    return;
                }
                else
                {
                    Debug.WriteLine("null");
                }
              
            }
   
            string newFolderPath = Path.Combine(defaultrootFolder, newfolderName);
            Folders.SubFolder.Add(new RootFolder
            {
                FolderName = newfolderName,
                FolderPath = newFolderPath,
                
            });
            Directory.CreateDirectory(newFolderPath);
            IsFolderRenaming = false;
        }
        private void RenameDir()
        {
            DisablePopUp();
            if (IsFolderSelected)
            {
                AddfolderDialog dialog = new AddfolderDialog();
                dialog.ShowDialog();
                var currentDirPath = SelectedFolder.FolderPath;
                if(currentDirPath == null)
                {
                    MessageBox.Show("no folder selected");
                    return;
                }
                try
                {
                    var newFolderName = dialog.FolderName;
                    var newFolderPath = Path.Combine(Path.GetDirectoryName(currentDirPath), newFolderName);

                    Directory.Move(currentDirPath, newFolderPath);




                    LoadTreeView();
                }catch(Exception err)
                {
                    Debug.WriteLine(err.Message);
                    return;
                }
                
            }
            else
            {
               
            }
        }
        public void DeleteDir()
        {
            if (IsFolderSelected)
            {
                Directory.Delete(SelectedFolder.FolderPath,true);
                LoadTreeView();
                DisablePopUp();
            }
        }
         private void DisablePopUp()
        {
            IsPopUpOpen = false;
        }
        public void EditDir()
        {
            if (IsPopUpOpen)
            {
                IsPopUpOpen = false;
                IsPopUpOpen = true;
            }
            IsPopUpOpen = true;
        }
        public void DisplayBranch(object parameter)
        {
            IsFolderSelected = true;
            SelectedFolder = parameter as RootFolder;
            DisablePopUp();
        }
        private void UnFocusValue()
        {
            IsFolderSelected = false;
            DisablePopUp();
            SelectedFolder = null;
         
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
        private Image RenderIcons(string iconPath, bool isPackUri = true)
        {
            Image imageIcon = new Image();
            imageIcon.Width = 32;
            imageIcon.Height = 32;
            imageIcon.Source = new BitmapImage(new Uri(iconPath, UriKind.Relative));
            return imageIcon;
        }
    }
}
