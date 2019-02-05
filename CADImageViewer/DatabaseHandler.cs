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
    public class DatabaseHandler
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

        // Checks to see if password given associates with 
        public bool CheckUpdateAccess()
        {
            bool updateAccess = false;
            string sql = "UPDATE config SET `config_value` = 'yes' WHERE `key` = 'Permission'";

            try
            {
                using (MySqlConnection c = new MySqlConnection(connectionString))
                {
                    c.Open();
                    MySqlCommand command = new MySqlCommand(sql, c);
                    int rowsAffected = command.ExecuteNonQuery();
                    // Just trying to check if we can update this field without throwing an error.
                    // If so, then we can update configuration values.
                    HandleQuery("UPDATE config SET `config_value` = 'yes' WHERE `key` = 'Permission'");

                    if ( rowsAffected == 1 )
                    {
                        updateAccess = true;
                    }
                }
            }
            catch
            {
                updateAccess = false;
            }

            return updateAccess;
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

        // Updating 
        public bool Update_Image_Base_Config( string imageBasePath )
        {
            string sql = "UPDATE config SET config_value = @imageBasePath WHERE `key` = 'ImageBasePath'";
            try
            {
                using (MySqlConnection c = new MySqlConnection(connectionString))
                { 
                    c.Open();
                    MySqlCommand command = new MySqlCommand(sql, c);
                    command.Parameters.AddWithValue("@imageBasePath", imageBasePath);
                    int rowsAffected = command.ExecuteNonQuery();

                    if ( rowsAffected < 1 )
                    {
                        throw new Exception("No Rows were affected on this update query");
                    }
                }
            }
            catch (ArgumentException err)
            {
                return false;
            }

            return true;
        }

        public string ObtainBaseImageDirectory()
        {
            return HandleSelect_SingleString("config_value", "config", "key", "ImageBasePath");
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

        public string HandleSelect_SingleString( string selectColumn, string table, string whereColumn, string whereValue )
        {
            string singleValue = null;
            string sql = String.Format("SELECT {0} FROM {1} WHERE `{2}` = '{3}'", selectColumn, table, whereColumn, whereValue);
            Console.WriteLine("SQL String: " + sql);
            using (MySqlConnection c = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sql, c);
                try
                {
                    c.Open();
                    var executingValue = cmd.ExecuteScalar();

                    singleValue = (executingValue == DBNull.Value) ? String.Empty : (string)executingValue;
                    Console.WriteLine("Returned Value: " + singleValue);
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
