using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CADImageViewer
{
    public class Command : ICommand
    {

        public Action Action { get; set; }

        public void Execute(object parameter)
        {
            Action?.Invoke();
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;

                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler CanExecuteChanged;

        public Command(Action action)
        {
            Action = action;
        }
    }

    public class Command<T> : ICommand
    {
        public Action<T> Action { get; set; }

        public void Execute(object parameter)
        {
            if (Action != null && parameter is T)
            {
                Action((T)parameter);
            }

        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        private bool _isEnabled = true;
        
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                IsEnabled = value;
                CanExecuteChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;

        public Command(Action<T> action)
        {
            Action = action;
        }
    }
}
