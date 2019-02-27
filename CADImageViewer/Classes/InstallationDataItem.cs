using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel; // For INotifyPropertyChanged


namespace CADImageViewer
{
    public class InstallationDataItem
    {
        public string Item { get; set; }
        public string Part { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string Status { get; set; }
        public string Picture { get; set; }
    }
}
