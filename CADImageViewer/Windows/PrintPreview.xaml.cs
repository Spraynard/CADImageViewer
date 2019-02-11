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
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace CADImageViewer
{
    /// <summary>
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : Window
    {
        // The document sequence displayed to our document viewer's "Document" parameter
        IDocumentPaginatorSource DisplayedDocument { get; set; }

        public PrintPreview( XpsDocument xps )
        {
            InitializeComponent();
            DataContext = this;
            DisplayedDocument = xps.GetFixedDocumentSequence();
        }
    }
}
