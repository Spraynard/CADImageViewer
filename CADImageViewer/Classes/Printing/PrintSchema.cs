using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace CADImageViewer.Classes.Printing
{
    public class PrintSchema
    {
        private DatabaseHandler DataBase { get; set; }
        private DocumentStore DocumentStore { get; set; }

        public PrintSchema()
        {
            DataBase = new DatabaseHandler();
            DocumentStore = new DocumentStore(DataBase);
        }

        public List<Block> RunFullSchema( UserInputItem userInput, string[] installations )
        {
            List<Block> elementList = new List<Block>();

            // Insert the page header
            Paragraph pageHeaderText = new Paragraph( new Run("REPORT FOR ENGINEER: " + userInput.Engineer));
            pageHeaderText.FontSize = 35;
            pageHeaderText.FontWeight = FontWeights.Bold;
            pageHeaderText.FontFamily = new System.Windows.Media.FontFamily("Verdana");

            elementList.Add(pageHeaderText);

            // Insert Installation Block 
            //  * Installation Title
            //  * Data Section
            //  * Notes Section

            foreach ( string installation in installations )
            {
                InstallationDataItem[] dataItems = DataBase.BuildInstallationData(installation, userInput.Engineer).ToArray();
                InstallationNote[] noteItems = DataBase.BuildInstallationNotes(installation).ToArray();

                InstallationPrintable installationPrintable = new InstallationPrintable(installation, dataItems, noteItems);

                elementList.Add(installationPrintable.GetSection());
            }

            // Add images to our elementList
            foreach (string installation in installations)
            {
                ObservableCollection<InstallationDataItem> imageDataItems = DataBase.BuildInstallationData(installation, userInput.Engineer);

                // List filed with FileInfo types
                ArrayList installationImages = DocumentStore.ObtainInstallationImages(installation, userInput.Program, userInput.Truck, imageDataItems);

                if ( installationImages.ToArray().Length > 0 )
                {
                    foreach (FileInfo fileInfo in installationImages)
                    {

                        // Rotating our image in place
                        RotateTransform transform = new RotateTransform(90);


                        // Create the image file

                        Image installationImage = new Image();

                        BitmapImage bitmap = new BitmapImage();
                        TransformedBitmap tbitmap = new TransformedBitmap();


                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(fileInfo.FullName);
                        bitmap.EndInit();

                        //tbitmap.BeginInit();
                        //tbitmap.Source = bitmap;
                        //tbitmap.Transform = transform;
                        //tbitmap.EndInit();

                        //installationImage.Source = tbitmap;

                        installationImage.Source = bitmap;

                        // Applying transforms
                        transform.CenterX = installationImage.ActualWidth / 2;
                        transform.CenterY = installationImage.ActualHeight / 2;

                        installationImage.RenderTransform = transform;

                        BlockUIContainer uiContainer = new BlockUIContainer();
                        uiContainer.BreakPageBefore = true;
                        uiContainer.Child = installationImage;

                        elementList.Add(uiContainer);

                    }
                }
            }

            return elementList;

        }


        public List<Block> RunSelectiveSchema()
        {
            throw new NotImplementedException("Run Selective Schema is not implemented");
        }

    }
}
