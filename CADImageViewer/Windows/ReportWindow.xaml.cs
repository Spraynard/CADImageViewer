using System;
using System.Collections;
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
using System.Data;
using System.Collections.ObjectModel; // For ObservableCollection
using System.ComponentModel; // For INotifyPropertyChanged
using System.IO;

namespace CADImageViewer
{
    /// <summary>
    /// Interaction logic for CADImageViewerReportPage.xaml
    /// </summary>
    public partial class CADImageViewerReportWindow : Window, INotifyPropertyChanged
    {
        private UserInputItem userInputItem;
        private DatabaseHandler DataBase { get; set; }
        private DocumentStore DocumentStore { get; set; }
        private string _selectedInstallation;
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<InstallationDataItem> _installationDataItems;
        private ObservableCollection<string> _availableInstallations;
        private ObservableCollection<InstallationNote> _installationNotes = new ObservableCollection<InstallationNote>();

        private ErrorPopup _errorPopup = new ErrorPopup();

        public Boolean ShowNotes
        {
            get { return (InstallationNotes.Count > 0) ? true : false; }
        }

        // Item user inputs in the home page.
        public UserInputItem UserInputItem
        {
            get
            {
                return userInputItem;
            }
            set
            {
                userInputItem = value;
            }
        }

        // List of installations obtained by user input in Home Page.
        public ObservableCollection<string> AvailableInstallations
        {
            get
            {
                return _availableInstallations;
            }
            set
            {
                _availableInstallations = value;
                OnPropertyChanged("AvailableInstallations");
            }
        }

        // Data obtained for each installation.
        public ObservableCollection<InstallationDataItem> InstallationDataItems
        {
            get { return _installationDataItems; }
            set { _installationDataItems = value; OnPropertyChanged("InstallationDataItems"); }
        }

        // Installation Notes for each installation
        public ObservableCollection<InstallationNote> InstallationNotes
        {
            get { return _installationNotes; }
            set { _installationNotes = value; OnPropertyChanged("InstallationNotes"); OnPropertyChanged("ShowNotes"); }
        }

        // Installation Currently Selected by user to display
        public string SelectedInstallation
        {
            get
            {
                return _selectedInstallation;
            }
            set
            {
                _selectedInstallation = value;
                OnPropertyChanged("SelectedInstallation");
            }
        }

        // Initialization Of Report Page.
        // List all of the available installations for the Program, Truck, and Engineer
        // Based on Installation selection, display specific installation data.
        public CADImageViewerReportWindow(UserInputItem Item)
        {
            userInputItem = Item;
            DataBase = new DatabaseHandler();
            DocumentStore = new DocumentStore(DataBase);
            InitializeComponent();
        }

        private ObservableCollection<string> ObtainInstallationList( string engineer )
        {
            //string queryString = String.Format("SELECT DISTINCT Installation FROM bom WHERE Program = '{0}' AND Truck = '{1}' AND DRE = '{2}'", program, truck, engineer);
            string queryString = String.Format("SELECT DISTINCT Installation FROM bom WHERE DRE = '{0}'", engineer);

            return DataBase.HandleQuery_ObservableCollection(queryString);
        }

        // Any changes to the selectbox containing installations will result in this handling function being run.
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Getting reference of listbox that is sending the "Selection Changed" event.
            ListBox listBox = sender as ListBox;

            // Getting current selected index of the listbox selection.
            int listBoxSelectedIndex = listBox.SelectedIndex;

            // Obtaining the value of the selected Installation based on index.
            string selectedInstallation = AvailableInstallations[listBoxSelectedIndex];

            // Update the selected installation
            SelectedInstallation = selectedInstallation;

            // Setting the current InstallationDataItems class property to the built installation data
            InstallationDataItems = DataBase.BuildInstallationData(SelectedInstallation, UserInputItem.Engineer);

            // Obtaining and setting the InstallationNotes class property
            InstallationNotes = DataBase.BuildInstallationNotes(SelectedInstallation);

            // Selectively Show / Hide Installation Notes Based on whether we have any.
            //ManipulateInstallationNoteView();

            // Obtaining Images Applicable our Installation.
            ArrayList installationImages = new ArrayList();

            try
            {
                installationImages = DocumentStore.ObtainInstallationImages(
                    SelectedInstallation,
                    UserInputItem.Program,
                    UserInputItem.Truck,
                    InstallationDataItems);
            }
            catch ( Exception ex )
            {
                // Something wrong with our path or whatever. We'll display it in our error window.
                _errorPopup.ErrorText = ex.Message;
                _errorPopup.CurrentWindowReference = this;

                if ( _errorPopup.IsOpen == false )
                {
                    _errorPopup.IsOpen = true;
                }
            }
            finally
            {
                if ( installationImages.Count > 0 )
                {
                    ImageDisplayList.DataContext = installationImages;
                    InstallationImageEmptyIndication.Visibility = Visibility.Hidden;
                }
                else
                {
                    InstallationImageEmptyIndication.Visibility = Visibility.Visible;
                    ImageDisplayList.Visibility = Visibility.Hidden;
                }
            }
                
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // When a user double clicks on an image it will show the full image in a new window.
        private void ImageDisplayList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            FileInfo SelectedImage = listBox.SelectedItem as FileInfo;

            ImageWindow imageWindow = new ImageWindow(SelectedImage)
            {
                // Full Screen
                WindowState = (WindowState)2
            };

            imageWindow.Show();
        }

        // Operations to execute when we load our report window.
        private void ReportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InstallationNotes = new ObservableCollection<InstallationNote>();

            // On startup, obtain all installations for our specific program, truck, and engineer
            ObservableCollection<string> installationsList = ObtainInstallationList(userInputItem.Engineer);

            // Set the list of installations available to our current SelectedInstallations.
            AvailableInstallations = installationsList;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintHandler printHandler = new PrintHandler( userInputItem, AvailableInstallations.ToArray() );

            printHandler.PrintFullReport();
        }
    }
}
