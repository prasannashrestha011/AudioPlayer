using AudioPlayer.NavigationStores;
using AudioPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AudioPlayer.Views
{
    /// <summary>
    /// Interaction logic for AudioPlayerView.xaml
    /// </summary>
    public partial class AudioPlayerView : UserControl
    {
        public AudioPlayerView()
        {
            InitializeComponent();
            MainNavigationStore mainNavStore=new MainNavigationStore();
            DataContext = new AudioPlayerViewModel(mainNavStore);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
