using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace CADImageViewer.Classes.Printing
{
    public class PrintSchema
    {
        public DatabaseHandler Database { get; set; }

        public PrintSchema()
        {
            Database = new DatabaseHandler();
        }

        public List<Block> RunFullSchema( string engineer, string[] installations )
        {
            List<Block> elementList = new List<Block>();

            // Insert the page header

            // Insert Installation Block 
            //  * Installation Title
            //  * Data Section
            //  * Notes Section

            foreach ( string installation in installations )
            {
                InstallationDataItem[] dataItems = Database.BuildInstallationData(installation, engineer).ToArray();
                InstallationNote[] noteItems = Database.BuildInstallationNotes(installation).ToArray();

                InstallationPrintable installationPrintable = new InstallationPrintable(installation, dataItems, noteItems);

                elementList.Add(installationPrintable.GetSection());
            }


            return elementList;

        }


        public List<Block> RunSelectiveSchema()
        {
            throw new NotImplementedException("Run Selective Schema is not implemented");
        }

    }
}
