using System;
using System.Globalization;
using MaterialDesignThemes.Wpf;
using Myphones.Buddies.Model.DataModel;

namespace PhoneBuddy.Wpf
{
    public class PresenceValueConverter : BaseValueConverter<PresenceValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case (int)State.Available:
                    return new PackIcon { Kind = PackIconKind.AccountTick };
                default:
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.ToObject(targetType, System.Convert.ToInt32(value));
        }
    }
}
