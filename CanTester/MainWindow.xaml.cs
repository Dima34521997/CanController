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
using CAN_Test.ApiCanController;


namespace CanTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApiCanController ACC = new ApiCanController();
        public MainWindow()
        {
            InitializeComponent();
        }


        private void StartCan(object sender, RoutedEventArgs e)
        {
            ACC.ActivateCanOpen();
        }

        private void StopCan(object sender, RoutedEventArgs e)
        {
            ACC.DisactivateCanOpen();
        }
    }
}
