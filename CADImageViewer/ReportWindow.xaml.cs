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
        private DatabaseHandler db = new DatabaseHandler();
        private DocumentStore ds = new DocumentStore();
        private string _selectedInstallation;
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<InstallationDataItem> _installationDataItems;
        private ObservableCollection<string> _selectedInstallations;
        private ObservableCollection<InstallationNote> _installationNotes;

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
        public ObservableCollection<string> SelectedInstallations
        {
            get
            {
                return _selectedInstallations;
            }
            set
            {
                _selectedInstallations = value;
                OnPropertyChanged("SelectedInstallations");
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
            InstallationNotes = new ObservableCollection<InstallationNote>();
            InitializeComponent();
           
            // On startup, obtain all installations for our specific program, truck, and engineer
            ObservableCollection<string> selectedInstallationsList = ObtainInstallationList(userInputItem.Program, userInputItem.Truck, userInputItem.Engineer);

            // Set the list of installations available to our current SelectedInstallations.
            SelectedInstallations = selectedInstallationsList;
        }

        // Public classbased getters 
        //private void ManipulateInstallationNoteView()

        private ObservableCollection<string> ObtainInstallationList( string program, string truck, string engineer )
        {
            string queryString = String.Format("SELECT DISTINCT Installation FROM bom WHERE Program = '{0}' AND Truck = '{1}' AND DRE = '{2}'", program, truck, engineer);

            Console.WriteLine("Printing Installation");

            return db.HandleQuery_ObservableCollection(queryString);
        }

        private DataTable ObtainInstallationData( string installation, string program, string truck, string engineer )
        {
            string queryString = String.Format("SELECT Item, Part, Description, Quantity, Status, Picture FROM bom WHERE Installation = '{0}' AND Program = '{1}' AND Truck = '{2}' AND DRE = '{3}'", installation, program, truck, engineer);

            DataTable returnTable = db.HandleQuery(queryString);

            return returnTable;
        }

        private DataTable ObtainInstallationNotes( string installation )
        {
            string queryString = String.Format("Select NoteID, Note from `installation notes` WHERE Installation = '{0}'", installation);

            return db.HandleQuery(queryString);
        }

        private ObservableCollection<InstallationDataItem> BuildInstallationData( string installation, string program, string truck, string engineer )
        {
            // Build "InstallationDataItem"s based on the selected installation
            ObservableCollection<InstallationDataItem> returnCollection = new ObservableCollection<InstallationDataItem>();

            DataTable installationDataTable = ObtainInstallationData(installation, program, truck, engineer);

            // Add our obtained items to the observable collections
            foreach (DataRow row in installationDataTable.Rows)
            {
                returnCollection.Add(new InstallationDataItem(Convert.ToString(row[0]), Convert.ToString(row[1]), Convert.ToString(row[2]), Convert.ToString(row[3]), Convert.ToString(row[4]), Convert.ToString(row[5])));
            }

            return returnCollection;
        }

        private ObservableCollection<InstallationNote> BuildInstallationNotes( string installation )
        {
            ObservableCollection<InstallationNote> returnCollection = new ObservableCollection<InstallationNote>();

            DataTable installationNotes = ObtainInstallationNotes(installation);

            foreach ( DataRow row in installationNotes.Rows )
            {
                returnCollection.Add(new InstallationNote(Convert.ToString(row[0]), Convert.ToString(row[1])));
            }

            return returnCollection;
        }

        // Any changes to the selectbox containing installations will result in this handling function being run.
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Getting reference of listbox that is sending the "Selection Changed" event.
            ListBox listBox = sender as ListBox;

            // Getting current selected index of the listbox selection.
            int listBoxSelectedIndex = listBox.SelectedIndex;

            // Obtaining the value of the selected Installation based on index.
            string selectedInstallation = SelectedInstallations[listBoxSelectedIndex];

            // Update the selected installation
            SelectedInstallation = selectedInstallation;

            // Setting the current InstallationDataItems class property to the built installation data
            InstallationDataItems = BuildInstallationData(SelectedInstallation, UserInputItem.Program, UserInputItem.Truck, UserInputItem.Engineer);

            // Obtaining and setting the InstallationNotes class property
            InstallationNotes = BuildInstallationNotes(SelectedInstallation);

            // Selectively Show / Hide Installation Notes Based on whether we have any.
            //ManipulateInstallationNoteView();

            // Obtaining Images Applicable our Installation.
            ArrayList installationImages = ds.ObtainInstallationImages(
                SelectedInstallation,
                UserInputItem.Program,
                UserInputItem.Truck,
                InstallationDataItems);

            ImageDisplayList.DataContext = installationImages;
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

            Console.WriteLine("What is our file's name?");
            Console.WriteLine(SelectedImage.Name);

            ImageWindow imageWindow = new ImageWindow(SelectedImage)
            {
                // Full Screen
                WindowState = (WindowState)2
            };

            imageWindow.Show();
        }
    }
}
