using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Linq;


namespace PhoneBuddy.Wpf
{

    class MultiMarginValueConverter : BaseValueConverter<MultiMarginValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            return new Thickness(System.Convert.ToDouble(value), 62, 0, 0);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
