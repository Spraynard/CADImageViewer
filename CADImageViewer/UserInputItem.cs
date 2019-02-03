using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CADImageViewer
{
    public class UserInputItem : INotifyPropertyChanged
    {
        // Program Reference
        private static string program;

        // Truck Reference
        private static string truck;

        // DRE (Engineer Reference)
        private static string dre;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Program
        {
            get
            {
                return program;
            }
            set
            {
                program = value;
                OnPropertyChanged("Program");
            }
        }

        public string Truck
        {
            get
            {
                return truck;
            }
            set
            {
                truck = value;
                OnPropertyChanged("Truck");
            }
        }

        public string Engineer
        {
            get
            {
                return dre;
            }
            set
            {
                dre = value;
                OnPropertyChanged("Engineer");
            }
        }

        public UserInputItem()
        {
            program = null;
            truck = null;
            dre = null;
        }

        public void Print()
        {
            string printString = String.Format("Program: {0}\nTruck: {1}\nEngineer: {2}", program, truck, dre);
            System.Diagnostics.Debug.WriteLine("Printing User Input Item");
            System.Diagnostics.Debug.WriteLine(printString);
        }

        public bool AllPropertiesAvailable()
        {
            if ( 
                program != null &&
                truck != null && 
                dre != null
            )
            {
                return true;
            }
            return false;
        }

        protected void OnPropertyChanged( string name )
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
