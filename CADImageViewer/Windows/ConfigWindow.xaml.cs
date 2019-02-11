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
using System.Data;

namespace CADImageViewer
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public DatabaseHandler DBRef { get; set; }

        public ConfigWindow( DatabaseHandler dbRef )
        {
            DBRef = dbRef;

            InitializeComponent();
            ConfigWindowViewModel vm = new ConfigWindowViewModel( DBRef );

            DataContext = vm;
            Console.WriteLine("VM Close WIndow: " + vm.CloseWindow);
            if (vm.CloseWindow == null)
            {
                Console.WriteLine("It was null");
                vm.CloseWindow = new Action(() => this.Close());
            }
        }
    }
}
