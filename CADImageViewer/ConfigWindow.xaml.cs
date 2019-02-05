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
        private bool _textChanged = false;
        private bool _adminAccess = false;
        private DatabaseHandler _dbRef = null;

        public ConfigWindow( DatabaseHandler dbRef, bool adminAccess )
        {
            _dbRef = dbRef;
            _adminAccess = adminAccess;
            InitializeComponent();
        }

        public bool TextChanged
        {
            get { return _textChanged; }
            set { _textChanged = value; }
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            TextBox imageFilePath = baseImageFilepathTextBox;
            string fp_text = imageFilePath.Text;

            //g:/Freelance/Projects/CADIMageViewer/images/installation_images/
            bool updateSuccess  = _dbRef.Update_Image_Base_Config(fp_text);

            if ( updateSuccess != true )
            {
                // Popup something like update failed or whatever.
                FlashTextBox.Text = "Update of values not successful. Error occured.";
            }
            else
            {
                FlashTextBox.Text = "Update of config values successful";
            }

            TextChanged = false;
        }

        private void ImageFilePathText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ( SaveButton.IsEnabled == false && _adminAccess )
            {
                SaveButton.IsEnabled = true;
            }

            if ( FlashTextBox.Text.Count<char>() > 0 )
            {
                FlashTextBox.Text = "";
            }
        }

        // When the window is loaded insert text into our textboxes.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the text of the image file path textbox.
            TextBox imageFilePath = baseImageFilepathTextBox;
            string imageFilePathText = _dbRef.ObtainBaseImageDirectory();
            imageFilePath.Text = imageFilePathText;

            // Update Active/Disabled on save button
            if ( _adminAccess != false )
            {
                imageFilePath.IsEnabled = true;
            }
        }
    }
}
