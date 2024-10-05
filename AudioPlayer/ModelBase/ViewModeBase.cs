using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AudioPlayer.ModelBase
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public static event Action? StaticPropertyChanged;
       
        protected void OnPropertyChanged([CallerMemberName]string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        protected static void OnStaticPropertyChanged()
        {
            StaticPropertyChanged?.Invoke();
        }
    }
}
