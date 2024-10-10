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
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public bool IsNormalState = true;
        public MainWindow()
        {
            InitializeComponent();
        }
        public bool isNavigated = false;
     
        public bool isPlayLisModelOpened = true;
        DoubleAnimation animation = new DoubleAnimation();
       
    private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.EasingFunction = new QuadraticEase
            {
                EasingMode = EasingMode.EaseOut
            };
            if (isNavigated)
            {
                TranslateBack();


            }
            else
            {

                animation.From = TranslateNav.X;
                animation.To = 0;

                isNavigated = true;
            }
            BeginAnimation();
        }
        private void BeginAnimation()
        {
            TranslateNav.BeginAnimation(TranslateTransform.XProperty, animation);
        }
        private void TranslateBack()
        {
            
                animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
                animation.EasingFunction = new QuadraticEase
                {
                    EasingMode = EasingMode.EaseOut
                }; 
            animation.From = TranslateNav.X;
            animation.To = TranslateNav.X + 250;

            isNavigated = false;
            
            BeginAnimation();

        }
   

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (isNavigated)
            {
                TranslateBack();
            }
        }

        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

    
    }
}