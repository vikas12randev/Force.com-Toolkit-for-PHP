using Myphones.Buddies.Model.DataModel;
using PhoneBuddy.Wpf.View;
using PhoneBuddy.Wpf.View.UserControls;
using Prism.Events;
using System;
using System.Globalization;

namespace PhoneBuddy.Wpf
{
    // <summary>
    // Converts the<see cref="ApplicationPage"/> to an actual view
    // </summary>
    public class PageValueConverter : BaseValueConverter<PageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case (int)ApplicationPage.Login:
                    return new LoginView();
                case (int)ApplicationPage.PBGeneral:
                    return new PBMainView();
                default:
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
