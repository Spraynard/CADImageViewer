using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;

namespace CADImageViewer
{
    class DocumentStore
    {
        DatabaseHandler _db = new DatabaseHandler();
        private bool _imgDirectoryExists = false;
        private string _imgDirectory = null;

        public DocumentStore()
        {
            InitializeDocumentStore();
        }

        public bool ImageDirectoryExists
        {
            get { return _imgDirectoryExists; }
        }

        private void InitializeDocumentStore()
        {

            // Obtain applicable directory names ( currently only the image directory )
            _imgDirectory = _db.ObtainBaseImageDirectory();

            // Check if our image directory even exists (i.e., we have access to it)
            if (DirectoryExists(_imgDirectory) != false)
            {
                _imgDirectoryExists = true;
            }
        }

        private bool DirectoryExists( string path )
        {
            bool exists = false;
            DirectoryInfo di = new DirectoryInfo(path);

            try
            {
                if (di.Exists)
                {
                    exists = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

            return exists;
        }

        private String ObtainFolderString(string truck, string installation, string picture)
        {
            string queryString = String.Format("SELECT Folder FROM imagefilepath WHERE Truck = '{0}' AND Installation = '{1}' AND `Drawing Number` = '{2}'", truck, installation, picture);
            ObservableCollection<string> returnCollection = _db.HandleQuery_ObservableCollection(queryString);

            return returnCollection[0];
        }

        // Obtains specific installation images based on the given image base path in our database.
        public ArrayList ObtainInstallationImages(
            string installation,
            string program,
            string truck,
            ObservableCollection<InstallationDataItem> items
            )
        {
            ArrayList imageFiles = new ArrayList();

            // Exit out of our function if the directory doesn't exist.
            if ( _imgDirectoryExists == false )
            {
                return imageFiles;
            }

            /* 
             * Directory used to find images related to the program that holds our installation
             */
            string programSpecificDirectory = String.Format("{0}_Installations_OCR_Scan", program);

            // Combine program specific directory with the base directory and check if it exists.
            string fullProgramPath = System.IO.Path.Combine(new string[] { _imgDirectory, programSpecificDirectory });

            if ( DirectoryExists( fullProgramPath ) == false)
            {
                return imageFiles;
            }

            foreach( InstallationDataItem item in items )
            {
                // Item picture code of "999" means that there is no image available.
                if ( item.Picture == "999" )
                {
                    continue;
                }

                string specificItems = ObtainFolderString(truck, installation, item.Picture);

                // No folder available to look for.
                if ( specificItems.Length == 0 )
                {
                    continue;
                }
                
                // The folder for our specific installation in our program specific directory.
                string installationSpecificImageFolder = String.Format("{0}_JPG_Images_Item_{1}", installation, specificItems);

                string fullItemImagePath = System.IO.Path.Combine(new string[] { fullProgramPath, installationSpecificImageFolder });

                if (DirectoryExists(fullItemImagePath) != true)
                {
                    continue;
                }

                string imageFileLookupString = String.Format("{0}_{1}_*_results.jpg", installation, item.Picture);

                var obtainedImageFiles = Directory.GetFiles(fullItemImagePath, imageFileLookupString);

                foreach ( var image in obtainedImageFiles )
                {
                    var info = new FileInfo(image);
                    imageFiles.Add(info);
                }
            }
            return imageFiles;
        }
    }
}
