using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public bool isNavigated = false;
        public bool isPlayLisModelOpened = true;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation=new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.EasingFunction=new QuadraticEase { EasingMode = EasingMode.EaseOut };
            if (isNavigated)
            {
                animation.From = TranslateNav.X;
                animation.To = TranslateNav.X + 250;

                isNavigated = false;
            }
            else
            {
                animation.From = TranslateNav.X;
                animation.To = 0;
              
                isNavigated = true;
            }
            TranslateNav.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.EasingFunction=new QuadraticEase { EasingMode=EasingMode.EaseOut };
            if (isPlayLisModelOpened)
            {
                animation.From = PlayListModel.Y;
                animation.To = PlayListModel.Y - 400;
                isPlayLisModelOpened = false;
               
                Debug.WriteLine(isPlayLisModelOpened.ToString());
            }
            else
            {
                animation.From = PlayListModel.Y;
                animation.To = 400;
                isPlayLisModelOpened = true;
                Debug.WriteLine(isPlayLisModelOpened.ToString());
            }
            PlayListModel.BeginAnimation(TranslateTransform.YProperty, animation);
        }
    }
}