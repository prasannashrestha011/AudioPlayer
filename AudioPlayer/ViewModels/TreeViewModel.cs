using AudioPlayer.ModelBase;
using AudioPlayer.RelayBase;
using AudioPlayer.Structure;
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
        public ICommand AddNewDir => new RelayCommandBase(canExecute => true, execute => CreateDir());
        public ICommand DisplayFileName => new RelayCommandBase(canExecute => true, execute => OnFileSelected(execute));

        public ICommand SelectedObj => new RelayCommandBase(canExecute => true, execute => DisplayBranch(execute));
        public TreeViewModel()
        {
            string rootFolder = @"D:\testFolder";
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
            foreach (var file in rootDir.GetFiles())
            {
                Folder.SubFolder.Add(new Files
                {
                    FileName = file.Name,
                    FilePath = file.FullName,

                });
            }
            return Folder;
        }
        public void OnFileSelected(object parameter)
        {
            if (parameter is Files selectedFile)
            {
                MessageBox.Show($"{selectedFile.FileName}");
            }
            else
            {
                MessageBox.Show("file not found");
            }
        }
        public void CreateDir()
        {
            if (IsFolderSelected)
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
            MessageBox.Show(SelectedFolder.FolderName);
        }
    }
}
