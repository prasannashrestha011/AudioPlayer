using AudioPlayer.NavigationStores;
using AudioPlayer.ViewModels;
using System.Configuration;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace AudioPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
       
            MainNavigationStore mainNavStore = new MainNavigationStore();
            AudioPlayerViewModel audioPlayerViewModel = new AudioPlayerViewModel(mainNavStore);
            mainNavStore.CurrentViewModel= audioPlayerViewModel;

            MainWindow mainWindow = new MainWindow();
            MainViewModel mainViewModel = new MainViewModel(mainNavStore);
            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();
        }
    }

}
