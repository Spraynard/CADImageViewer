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
using System.Configuration;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CADImageViewer
{
    /// <summary>
    /// Interaction logic for CADImageViewerHomePage.xaml
    /// </summary>
    public partial class CADImageViewerHomeWindow : Window, INotifyPropertyChanged
    {
        // Class that will contain the user's selections of categories:
        //  * Program
        //  * Truck
        //  * Engineer
        private static UserInputItem UserSelections = new UserInputItem();
        private static DatabaseHandler db = new DatabaseHandler();

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<string> _programs = new ObservableCollection<string>();
        private ObservableCollection<string> _trucks = new ObservableCollection<string>();
        private ObservableCollection<string> _engineers = new ObservableCollection<string>();

        private string _categorySelected = "";


        public CADImageViewerHomeWindow()
        {
            InitializeComponent();

            // On init, we want to obtain our available PROGRAMS
            string queryString = "SELECT DISTINCT Program FROM bom";

            ObservableCollection<string> programCollection;

            try
            {
                bool connectionAvailable = db.ConnectionAvailable();

                if ( connectionAvailable == false )
                {
                    throw new Exception("No Connection Available. Please contact administrator");
                }

                programCollection = db.HandleQuery_ObservableCollection(queryString);

                Programs = programCollection;
            }
            catch( Exception e )
            {
                string popupText = String.Format("{0}", e.ToString());
                ErrorPopupText.Text = popupText;
                ErrorPopup.IsOpen = true;
            }


            db.HandleQueryAndPrint(queryString);
        }

        private string GetTrucksQueryString( string program )
        {
            return String.Format("SELECT DISTINCT Truck FROM bom WHERE Program = '{0}'", program);
        }

        private string GetEngineersQueryString( string program, string truck )
        {
            return String.Format("SELECT DISTINCT DRE FROM bom WHERE Program = '{0}' AND Truck = '{1}'", program, truck);
        }

        public ObservableCollection<string> Programs
        {
            get { return _programs; }
            set { _programs = value; OnPropertyChanged("Programs"); }
        }

        public ObservableCollection<string> Trucks
        {
            get { return _trucks; }
            set { _trucks = value; OnPropertyChanged("Trucks"); }
        }

        public ObservableCollection<string> Engineers
        {
            get { return _engineers; }
            set { _engineers = value; OnPropertyChanged("Engineers"); }
        }

        public string SelectedCategory
        {
            get { return _categorySelected; }
            set { _categorySelected = value; }
        }

        // Handles clicking of the "Get Report" button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CADImageViewerReportWindow viewerReportPage = new CADImageViewerReportWindow(UserSelections)
            {
                WindowState = (WindowState)2
            };

            viewerReportPage.Show();
            //this.NavigationService.Navigate(viewerReportPage);
        }

        // Inserts User Selection from any of our home page <ListBox/>es into our user selection class.
        
        // We also update the category specific text boxes with the text of our selection.
        private void InsertUserSelection( string ListBoxName, string ListBoxItemContent )
        {
            switch ( ListBoxName )
            {
                case "Program":
                    UserSelections.Program = ListBoxItemContent;
                    SelectedName.Text = ListBoxItemContent;
                    break;
                case "Truck":
                    UserSelections.Truck = ListBoxItemContent;
                    SelectedTruck.Text = ListBoxItemContent;
                    break;
                case "DRE":
                    UserSelections.Engineer = ListBoxItemContent;
                    SelectedEngineer.Text = ListBoxItemContent;
                    break;
            }
        }

        // Obtains the user selection from specific user selections.
        private string ObtainUserSelection(string ListBoxName, int SelectedListBoxIndex)
        {
            string returnString = null;
            ObservableCollection<string> reference = null;

            switch( ListBoxName )
            {
                case "Program":
                    reference = Programs;
                    break;
                case "Truck":
                    reference = Trucks;
                    break;
                case "DRE":
                    reference = Engineers;
                    break;
                default:
                    break;
            }

            returnString = reference[SelectedListBoxIndex];

            return returnString;
        }

        private void UpdateNextListBoxWithData( string ListBoxName )
        {
            string queryString = null;

            switch ( ListBoxName )
            {
                case "Program":
                    queryString = GetTrucksQueryString(UserSelections.Program);
                    Trucks = db.HandleQuery_ObservableCollection(queryString);
                    break;
                case "Truck":
                    queryString = GetEngineersQueryString(UserSelections.Program, UserSelections.Truck);
                    Engineers = db.HandleQuery_ObservableCollection(queryString);
                    break;
                default:
                    break;
            }

            //Console.WriteLine("Query String");
            //Console.WriteLine(queryString);
            //db.HandleQueryAndPrint(queryString);
            //observableReference = db.HandleQuery_ObservableCollection(queryString);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox ListBoxContext = sender as ListBox;

            string ListBoxName = ListBoxContext.Name;

            System.Diagnostics.Debug.WriteLine(ListBoxName);

            // List Box Item Context.
            ListBoxItem SelectedListBoxItem = (ListBoxContext.SelectedItem as ListBoxItem);

            // Getting Index of list item selected
            int SelectedListBoxIndex = ListBoxContext.SelectedIndex;

            // Obtaining content from specific class item property
            string SelectedListBoxItemContent = ObtainUserSelection(ListBoxName, SelectedListBoxIndex);

            // Inserting the content into our "ItemsSelected" class, which is eventually passed over to the report window.
            InsertUserSelection(ListBoxName, SelectedListBoxItemContent);

            // For Debug Purposes
            //UserSelections.Print();

            UpdateNextListBoxWithData(ListBoxName);

            // Check to see if we have all items selected. If so, enable our button.
            if (UserSelections.AllPropertiesAvailable())
            {
                ReportButton.IsEnabled = true;
            }
        }

        private void Popup_Close(object sender, RoutedEventArgs e)
        {
            if ( ErrorPopup.IsOpen ) { ErrorPopup.IsOpen = false; }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Config_Button_Click(object sender, RoutedEventArgs e)
        {
            // Check to see what kind of permissions are available for the user.
            bool adminAccess = db.CheckUpdateAccess();

            Window configWindow = new ConfigWindow(db, adminAccess);

            configWindow.ShowDialog();
        }
    }
}