using AudioPlayer.Structure;
using AudioPlayer.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace AudioPlayer.Views
{
    /// <summary>
    /// Interaction logic for PlayListSideBar.xaml
    /// </summary>
    public partial class PlayListSideBar : UserControl
    {
      


        private Point startPoint;
        private TreeViewItem draggedItem;

        public PlayListSideBar()
        {
            InitializeComponent();
            DataContext = new TreeViewModel();
        }

        private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void TreeView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(null);
                if (Math.Abs(position.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    draggedItem = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);
                    if (draggedItem != null)
                    {
                        DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                    }
                }
            }
        }

        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop) && !e.Data.GetDataPresent(typeof(RootFolder)) && !e.Data.GetDataPresent(typeof(Files)))
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop) && !e.Data.GetDataPresent(typeof(RootFolder)) && !e.Data.GetDataPresent(typeof(Files)))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) // Dropping files from outside
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var targetItem = GetNearestContainer(e.OriginalSource as UIElement);
                var targetFolder = targetItem?.DataContext as RootFolder ?? (DataContext as TreeViewModel)?.Folders;

                if (targetFolder != null)
                {
                    foreach (string file in files)
                    {
                        string targetPath = Path.Combine(targetFolder.FolderPath, Path.GetFileName(file));

                        if (File.Exists(file))
                        {
                            // Move the file to the target folder on disk
                            File.Move(file, targetPath);

                            // Add the file to the folder in TreeView
                            targetFolder.SubFolder.Add(new Files { FileName = Path.GetFileName(file), FilePath = targetPath });
                        }
                        else if (Directory.Exists(file))
                        {
                            // Move the folder to the target folder on disk
                            Directory.Move(file, targetPath);

                            // Add the folder recursively to the TreeView
                            AddFolderRecursively(targetFolder, new DirectoryInfo(targetPath));
                        }
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(RootFolder)) || e.Data.GetDataPresent(typeof(Files))) // Internal drag and drop
            {
                Debug.WriteLine("internal");
                var droppedItem = e.Data.GetData(typeof(RootFolder)) as RootFolder ?? e.Data.GetData(typeof(Files)) as Files;
                var targetItem = GetNearestContainer(e.OriginalSource as UIElement);
                var targetFolder = targetItem?.DataContext as RootFolder ?? (DataContext as TreeViewModel)?.Folders;
                Debug.WriteLine($"Dropped Item Type: {droppedItem?.GetType().Name}");
                Debug.WriteLine($"Target Folder: {targetFolder?.FolderPath}");

                try {
                    if (targetFolder != null && droppedItem != null)
                    {
                        string sourcePath = droppedItem is Files fileItem ? fileItem.FilePath : (droppedItem as RootFolder).FolderPath;
                        string targetPath = Path.Combine(targetFolder.FolderPath, Path.GetFileName(sourcePath));


                        RemoveItemFromOriginalLocation(droppedItem);

                        try
                        {
                            if (droppedItem is Files file)
                            {

                                File.Move(file.FilePath, targetPath);

                                // Add the file to the target folder in TreeView
                                targetFolder.SubFolder.Add(new Files { FileName = file.FileName, FilePath = targetPath });
                            }
                            else if (droppedItem is RootFolder folder)
                            {

                                Directory.Move(folder.FolderPath, targetPath);


                                targetFolder.SubFolder.Add(folder);
                            }
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show($"Error moving file or folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                }
                catch(Exception err)
                {
                    Debug.WriteLine(err.Message);
                    return;
                }
            }
            else
            {
                Debug.WriteLine("Invalid data format for drop.");
            }
        }


        private TreeViewItem GetNearestContainer(UIElement element)
        {
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        private void AddFolderRecursively(RootFolder parentFolder, DirectoryInfo dirInfo)
        {
            var newFolder = new RootFolder { FolderName = dirInfo.Name };
            parentFolder.SubFolder.Add(newFolder);

            foreach (var file in dirInfo.GetFiles())
            {
                newFolder.SubFolder.Add(new Files { FileName = file.Name, FilePath = file.FullName });
            }

            foreach (var dir in dirInfo.GetDirectories())
            {
                AddFolderRecursively(newFolder, dir);
            }
        }

        private bool IsDescendantOf(object item, RootFolder folder)
        {
            Debug.WriteLine($"Checking if {item} is descendant of {folder.FolderName}");
            if (item == folder) return true;
            if (folder.SubFolder == null) return false;
            return folder.SubFolder.Any(subFolder => IsDescendantOf(item, subFolder));
        }

        private void RemoveItemFromOriginalLocation(object item)
        {
            var viewModel = DataContext as TreeViewModel;
            if (viewModel != null)
            {
                RemoveItemRecursively(viewModel.Folders, item);
            }
        }

        private bool RemoveItemRecursively(RootFolder folder, object item)
        {
            if (item is Files file)
            {
                if (folder.SubFolder.Remove(file)) return true;
            }
            else if (item is RootFolder subFolder)
            {
                if (folder.SubFolder.Remove(subFolder)) return true;
            }

            foreach (var childFolder in folder.SubFolder)
            {
                if (RemoveItemRecursively(childFolder, item)) return true;
            }

            return false;
        }

        private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
       
    }
}
