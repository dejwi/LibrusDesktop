using LibrusPlan.MVVM.ViewModel;
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

namespace LibrusPlan.CustomControls
{
    /// <summary>
    /// Interaction logic for TopButtons.xaml
    /// </summary>
    public partial class TopButtons : UserControl
    {
        public TopButtons()
        {
            InitializeComponent();
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.WindowState = WindowState.Minimized;
        }
        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window is MainWindow)
                Application.Current.Shutdown();
            else
                window.Hide();
        }
    }
}
