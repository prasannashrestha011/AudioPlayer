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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation=new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.EasingFunction=new QuadraticEase { EasingMode = EasingMode.EaseOut };
            if (isNavigated)
            {
                animation.From = TranslateNav.X;
                animation.To = TranslateNav.X + 210;

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
    }
}