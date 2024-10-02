using AudioPlayer.ModelBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer.NavigationStores
{
    public class MainNavigationStore
    {
        public event Action? CurrentViewModelChanged;
        public MainNavigationStore mainNavStore;
        private ViewModelBase currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }
       
        public void OnCurrentViewModelChanged() {
            CurrentViewModelChanged?.Invoke();
        }

    }
}
