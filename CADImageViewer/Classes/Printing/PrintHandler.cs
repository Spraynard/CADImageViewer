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

// Printing stuff
using CADImageViewer.Classes.Printing

namespace CADImageViewer
{
    public class PrintHandler
    {
        public PrintDialog PrintDialog { get; set; }
        public UserInputItem UserInput { get; set; }
        public ArrayList Installations { get; set; }
        public DatabaseHandler DataHandler { get; set; }

        public PrintHandler( UserInputItem userInputItem, ArrayList installations )
        {
            UserInput = userInputItem;
            Installations = installations;
            DataHandler = new DatabaseHandler();
        }

        /**
         * Breaking apart each of the items that will be visible on one page into different sections
         */
        private Section DocumentTitle()
        {
            Section pageSection = new Section();
            return pageSection;
        }

        /**
         * Title of the Installation
         */
        private TextBlock InstallationTitle() { return new TextBlock(); }

        /**
         * Installation Data section contains 
         */
        private Section InstallationDataSection()
        {
            Section pageSection = new Section();
            return pageSection;

            Section a = new Section();
        }

        /**
         * Installation notes will be built with this function on a fixed page
         */
        private Section InstallationNotesSection()
        {
            Section pageSection = new Section();
            return pageSection;
        }

        private FixedPage BuildInformationPage()
        {
            FixedPage page = new FixedPage();

            return page;
        }

        /**
         * Schemas are built here.
         */
        public FixedDocument CreateReportDocument( PrintDialog pd )
        {
            FixedDocument document = new FixedDocument();
            SetDocumentDimensions(pd, document);
            return document;
        }

        public FlowDocument CreateFullInstallationReport( PrintDialog pd )
        {
            PrintSchema ps = new PrintSchema();
            FlowDocument document = new FlowDocument();
            
            // Setting our Flow Document columns to have the same width as the printable area
            document.ColumnWidth = pd.PrintableAreaWidth;

           List<UIElement> printableElements = ps.RunFullSchema();
            foreach( UIElement elem in printableElements)
            {
                PageContent page = new PageContent();

            }

            return document;
        }

        private bool ShowPrintPreview()
        {
            return true;
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
        public void PrintFullReport()
        {
            PrintSchema fullReportSchema = new PrintSchema();

            if ( PrintDialog.ShowDialog() == true )
            {
                if ( ShowPrintPreview() == true )
                {
                    // Now we print
                }
            }
        }
    }
