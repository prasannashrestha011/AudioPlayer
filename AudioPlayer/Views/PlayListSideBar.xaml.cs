using AudioPlayer.ViewModels;
using System.Windows.Controls;

namespace AudioPlayer.Views
{
    /// <summary>
    /// Interaction logic for PlayListSideBar.xaml
    /// </summary>
    public partial class PlayListSideBar : UserControl
    {
        public PlayListSideBar()
        {
            InitializeComponent();
            this.DataContext = new PlayListViewModel(); 
        }
    }
}
