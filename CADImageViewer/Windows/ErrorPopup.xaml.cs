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
using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace CADImageViewer
{
    /// <summary>
    /// Interaction logic for ErrorPopup.xaml
    /// </summary>
    public partial class ErrorPopup : Popup, INotifyPropertyChanged
    {
        private string _errorText;
        private Window _currentWindow;

        public event PropertyChangedEventHandler PropertyChanged;

        public ErrorPopup()
        {
            DataContext = this;
            InitializeComponent();
        }

        public Window CurrentWindowReference
        {
            get { return _currentWindow; }
            set { _currentWindow = value; OnPropertyChanged("CurrentWindowReference"); }
        }

        public string ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; OnPropertyChanged("ErrorText"); }
        }


        // Closes our popup on click
        private void PopupCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
