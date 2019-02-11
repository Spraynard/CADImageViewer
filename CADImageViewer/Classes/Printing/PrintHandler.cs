using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Printing;

// Printing stuff
using CADImageViewer.Classes.Printing;
using System.IO.Packaging;

namespace CADImageViewer
{
    public class PrintHandler
    {
        public PrintDialog PrintDialog { get; set; }
        public string Engineer { get; set; }
        public string[] Installations { get; set; }
        public string TemporaryPreviewPath { get; set; }

        public PrintHandler(string engineer, string[] installations)
        {
            PrintDialog = new PrintDialog();
            Engineer = engineer;
            Installations = installations;
            TemporaryPreviewPath = returnPrintableTempName();
        }

        private string returnPrintableTempName()
        {
            return Path.GetTempPath() + "cadimg-" + Guid.NewGuid().ToString() + ".xaml";
        }

        /**
        * Using Schema: Full Report
        * Description of steps when user makes dialog selection:
        *   1. Run through the installations that are being supplied. For each installation, output, in order:
        *       * Installation Data
        *       * Installation Title
        *       * Installation Notes
        *       
        *   2. Again go through our installations and obtain the images specific to that installation.
        *       Each image should have a title to reference the installation data with each image.
        */
        public FlowDocument CreateFullInstallationReport(PrintDialog pd)
        {
            PrintSchema ps = new PrintSchema();
            FlowDocument document = new FlowDocument();

            // Setting our Flow Document columns to have the same width as the printable area
            document.ColumnWidth = pd.PrintableAreaWidth;
            document.PageHeight = pd.PrintableAreaHeight;
            document.PageWidth = pd.PrintableAreaWidth;

            List<Block> printableElements = ps.RunFullSchema( Engineer, Installations );

            foreach (Block elem in printableElements)
            {
                // Add these blocks to our flow document
                document.Blocks.Add(elem);
            }

            return document;
        }

        private bool ShowPrintPreview( FlowDocument fd, string fileName )
        {
            DocumentPaginator paginator = ((IDocumentPaginatorSource)fd).DocumentPaginator;

            XpsDocument xps = new XpsDocument(fileName, FileAccess.ReadWrite);
            XpsDocumentWriter xpw = XpsDocument.CreateXpsDocumentWriter(xps);

            xpw.Write(paginator);

            PrintPreview previewWindow = new PrintPreview(xps);

            previewWindow.ShowDialog();

            xps.Close();

            return true;
        }


        public void PrintFullReport()
        {
            Nullable<bool> DialogInput = PrintDialog.ShowDialog();

            if ( DialogInput == true || DialogInput == false )
            {
                FlowDocument PrintableFlowDocument = CreateFullInstallationReport(PrintDialog);

                if (ShowPrintPreview( PrintableFlowDocument, TemporaryPreviewPath ) == true)
                {
                    // Now we print
                }
            }

            // Delete the temporary file that was made in order to show the print preview
            if (File.Exists(TemporaryPreviewPath))
            {
                File.Delete(TemporaryPreviewPath);
            }
        }
    }
}
