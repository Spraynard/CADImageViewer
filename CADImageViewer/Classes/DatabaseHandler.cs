using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Collections.ObjectModel;

namespace CADImageViewer
{
    public class DatabaseHandler
    {
        public string ConnectionString { get; set; }

        // Initialization
        public DatabaseHandler()
        {
            ConnectionString = ObtainConnectionString();
        }


        private DataTable ObtainInstallationData(string installation, string engineer)
        {
            string queryString = String.Format("SELECT Item, Part, Description, Quantity, Status, Picture FROM bom WHERE Installation = '{0}' AND DRE = '{1}'", installation, engineer);

            DataTable returnTable = HandleQuery(queryString);

            return returnTable;
        }

        private DataTable ObtainInstallationNotes(string installation)
        {
            string queryString = String.Format("Select NoteID, Note from `installation notes` WHERE Installation = '{0}'", installation);

            return HandleQuery(queryString);
        }

        public ObservableCollection<InstallationDataItem> BuildInstallationData(string installation, string engineer)
        {
            // Build "InstallationDataItem"s based on the selected installation
            ObservableCollection<InstallationDataItem> returnCollection = new ObservableCollection<InstallationDataItem>();

            DataTable installationDataTable = ObtainInstallationData(installation, engineer);

            // Add our obtained items to the observable collections
            foreach (DataRow row in installationDataTable.Rows)
            {
                returnCollection.Add(new InstallationDataItem(Convert.ToString(row[0]), Convert.ToString(row[1]), Convert.ToString(row[2]), Convert.ToString(row[3]), Convert.ToString(row[4]), Convert.ToString(row[5])));
            }

            return returnCollection;
        }

        public ObservableCollection<InstallationNote> BuildInstallationNotes(string installation)
        {
            ObservableCollection<InstallationNote> returnCollection = new ObservableCollection<InstallationNote>();

            DataTable installationNotes = ObtainInstallationNotes(installation);

            foreach (DataRow row in installationNotes.Rows)
            {
                returnCollection.Add(new InstallationNote(Convert.ToString(row[0]), Convert.ToString(row[1])));
            }

            return returnCollection;
        }

        public bool ConnectionAvailable()
        {
            bool isConn = false;
            MySqlConnection c = null;

            try
            {
                using (c = new MySqlConnection(ConnectionString))
                {
                    c.Open();
                    isConn = true;
                }
            }
            catch( ArgumentException err )
            {
                isConn = false;
            }
            catch( MySqlException err )
            {
                isConn = false;

                switch( err.Number )
                {
                    case 1042: // Unable to connect to any specified hosts
                        break;
                    case 0: // Access Denied
                        break;
                    default:
                        break;
                }
            }

            return isConn;
        }

        // Public method of class that handles queries and outputs a dataTable filled with the query return.
        public DataTable HandleQuery( string queryString )
        {
            DataTable dt = new DataTable();

            MySqlConnection conn = new MySqlConnection(ConnectionString);

            try
            {
                conn.Open();

                MySqlDataAdapter adapter = new MySqlDataAdapter(queryString, conn);

                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();

            return dt;
        }

        public string HandleSelect_SingleString( string selectColumn, string table, string whereColumn, string whereValue )
        {
            string singleValue = null;
            string sql = String.Format("SELECT {0} FROM {1} WHERE `{2}` = '{3}'", selectColumn, table, whereColumn, whereValue);
            using (MySqlConnection c = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sql, c);
                try
                {
                    c.Open();
                    var executingValue = cmd.ExecuteScalar();

                    singleValue = (executingValue == DBNull.Value) ? String.Empty : (string)executingValue;
                }
                catch ( Exception ex )
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return singleValue;
        }

        // Used to return a string type observable collection from our query.
        // This innately means that we should only be querying singular columns
        // EX: Select id from [table];
        public ObservableCollection<string> HandleQuery_ObservableCollection( string queryString )
        {
            ObservableCollection<string> vs = new ObservableCollection<string>();
            DataTable dataTable = HandleQuery(queryString);

            foreach (DataRow row in dataTable.Rows)
            {
                // Only work with items that are length of 1 in order to provide stability for this function
                if ( row.ItemArray.Length > 1 )
                {
                    continue;
                }

                foreach ( string item in row.ItemArray )
                {
                    vs.Add(item);
                }
            }

            return vs;
        }

        // Debug Purpose Printer Of Returned Data
        public void HandleQueryAndPrint( string queryString )
        {
            DataTable returnedQueryData = HandleQuery(queryString);

            foreach (DataRow row in returnedQueryData.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    string itemFormatted = String.Format("{0}, ", item);
                    Console.Write(itemFormatted);
                }
                Console.WriteLine("\n\n");
            }
        }

        private string ObtainConnectionString()
        {
            string server = Properties.Settings.Default.hostname;
            string userId = Properties.Settings.Default.username;
            string password = Properties.Settings.Default.password;

            // We're definitely not going to have a 
            if (
                String.IsNullOrEmpty(server) ||
                String.IsNullOrEmpty(userId) ||
                String.IsNullOrEmpty(password)
            )
            {
                return null;
            }

            // Getting partial connection string
            string settings = Properties.Settings.Default.connection_string;

            if (settings == null)
            {
                return null;
            }

            MySqlConnectionStringBuilder builder =
                new MySqlConnectionStringBuilder(settings);

            // Supply additional values
            builder.Server = server;
            builder.UserID = userId;
            builder.Password = password;

            return builder.ConnectionString;
        }
    }
}
