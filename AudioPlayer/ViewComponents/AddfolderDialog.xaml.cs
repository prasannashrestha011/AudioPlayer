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
using System.Windows.Shapes;

namespace AudioPlayer.ViewComponents
{
 
    public partial class AddfolderDialog : Window
    {
        public static readonly DependencyProperty FolderNameProperty = DependencyProperty.Register(
          "foldername",
          typeof(string),
          typeof(AddfolderDialog)

          );
        public string FolderName
        {
            get
            {
                return (string)GetValue(FolderNameProperty);
            }
            set
            {
                SetValue(FolderNameProperty, value);
            }
        }
        public AddfolderDialog()
        {
            InitializeComponent();
        }
      
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           Close();
        }


       
    }
}
