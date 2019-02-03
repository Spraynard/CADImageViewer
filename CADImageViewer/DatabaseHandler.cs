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
    // Takes in a query string and a table name in order to query the specific table.
    // Return: DataTable element filled with the results from our query.
    class DatabaseHandler
    {
        private string connectionString;

        // Initialization
        public DatabaseHandler()
        {
            int cStringObtained = ObtainConnectionString();

            if ( cStringObtained != 1 )
            {
                throw new Exception("Our connection string is not obtained");
            }
        }

        public bool ConnectionAvailable()
        {
            bool isConn = false;
            MySqlConnection c = null;

            try
            {
                using (c = new MySqlConnection(connectionString))
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

            MySqlConnection conn = new MySqlConnection(connectionString);

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

        private int ObtainConnectionString()
        {
            // I Should try and provide encryption to the configuration settings so I might have to do something here that performs unencryption operations.

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["CADImageViewer.Properties.Settings.CADImageViewerConnectionString.Admin"];

            if ( settings != null )
            {
                connectionString = settings.ConnectionString;
                return 1;
            }

            return 0;
        }
    }
}
