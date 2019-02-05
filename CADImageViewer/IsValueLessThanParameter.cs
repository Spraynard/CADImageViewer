using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;


namespace CADImageViewer
{
    class IsValueLessThanParameter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isLessThan = false;

            int valueInt = System.Convert.ToInt16(value);
            int valueParameter = System.Convert.ToInt16(parameter);

            if ( valueInt < valueParameter )
            {
                isLessThan = true;
            }

            return isLessThan;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
