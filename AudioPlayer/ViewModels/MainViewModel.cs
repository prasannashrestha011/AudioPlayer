using AudioPlayer.ModelBase;
using AudioPlayer.NavigationStores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer.ViewModels
{
    public class MainViewModel:ViewModelBase
    {
        public readonly MainNavigationStore mainNavStore;
        public ViewModelBase CurrentViewModel => mainNavStore.CurrentViewModel;
        public MainViewModel(MainNavigationStore mainNavStore)
        {
            this.mainNavStore = mainNavStore;
            mainNavStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }
        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
