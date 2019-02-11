using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel; // For INotifyPropertyChanged


namespace CADImageViewer
{
    public class InstallationDataItem : INotifyPropertyChanged
    {
        private string _item;
        private string _part;
        private string _description;
        private string _quantity;
        private string _status;
        private string _picture;

        public event PropertyChangedEventHandler PropertyChanged;

        public InstallationDataItem( string item, string part, string description, string quantity, string status, string picture )
        {
            Item = item;
            Part = part;
            Description = description;
            Quantity = quantity;
            Status = status;
            Picture = picture;
        }

        public string Item
        {
            get { return _item; }
            set { _item = value; OnPropertyChanged("Item"); }
        }

        public string Part
        {
            get { return _part; }
            set { _part = value; OnPropertyChanged("Part"); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged("Description"); }
        }

        public string Quantity
        {
            get { return _quantity; }
            set { _quantity = value; OnPropertyChanged("Quantity"); }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("Status"); }
        }

        public string Picture
        {
            get { return _picture; }
            set { _picture = value; OnPropertyChanged("Picture"); }
        }

        public void PrintProps()
        {
            Console.WriteLine("Printing out Installation Data Item");
            Console.WriteLine(String.Format("Item: {0}\nPart: {1}\nDescription: {2}\nQuantity: {3}\nStatus: {4}\n", Item, Part, Description, Quantity, Status));
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
