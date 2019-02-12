using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.ComponentModel;

namespace CADImageViewer
{
    public abstract class ConfigurationItem : INotifyPropertyChanged
    {
        public string Label { get; set; }

        private string _value;
        public string Value { get { return _value; } set { _value = value; OnPropertyChanged("Value"); } }

        public bool ReadOnly { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected abstract void getValue();
        public abstract void SetValue();

        public ConfigurationItem()
        {
            ReadOnly = false;
        }

        public void OnPropertyChanged( string propertyName )
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UserConfiguration : ConfigurationItem
    {
        public string UserConfigurationValue { get; set; }
        public string UserConfigurationKey { get; set; }

        public UserConfiguration( string confKey, string label ) : base()
        {
            UserConfigurationKey = confKey;
            UserConfigurationValue = Properties.Settings.Default[UserConfigurationKey].ToString();
            Label = label;
            getValue();
        }

        protected override void getValue()
        {
            if ( ! String.IsNullOrEmpty( UserConfigurationValue ) )
            {
                Value = UserConfigurationValue;
            }
        }

        public override void SetValue()
        {
            Properties.Settings.Default[UserConfigurationKey] = Value;
        }
    }

    class ConfigWindowViewModel
    {
        private DatabaseHandler DBRef { get; set; }

        public List<ConfigurationItem> ConfigData { get; set; }

        public Command UpdateConfigurationValues { get; set; }
        public Action CloseWindow { get; set; }

        public ConfigWindowViewModel( DatabaseHandler dbRef )
        {
            DBRef = dbRef;

            List<ConfigurationItem> InitialConfigData = new List<ConfigurationItem>();

            InitialConfigData.Add(new UserConfiguration("username", "Username"));
            InitialConfigData.Add(new UserConfiguration("password", "Password"));
            InitialConfigData.Add(new UserConfiguration("hostname", "Host IP"));
            InitialConfigData.Add(new UserConfiguration("baseImagePath", "Base Image Path"));

            ConfigData = InitialConfigData;

            UpdateConfigurationValues = new Command(SaveConfigurationsCommand);
        }

        private void SaveConfigurationsCommand()
        {
            foreach (ConfigurationItem item in ConfigData)
            {
                if ( item.ReadOnly == false )
                {
                    item.SetValue();
                }
            }

            Properties.Settings.Default.Save();

            CloseWindow();
        }
    }
}
