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
using System.IO;


namespace CADImageViewer
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {

        private FileInfo _currentImage;

        public ImageWindow( FileInfo image )
        {
            // Setting our image before component initialization
            CurrentImage = image;

            // Now we initializeour component.
            InitializeComponent();

            // Changing our window's title dynamically based on the file name.
            this.Title = String.Format("{0} - {1}", this.Title, CurrentImage.Name);
        }

        public FileInfo CurrentImage
        {
            get { return _currentImage; }
            set { _currentImage = value; }
        }
    }
}
