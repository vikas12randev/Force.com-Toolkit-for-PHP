using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhoneBuddy.Wpf.ValueConverters
{
    /// <summary>
    /// A converter that takes in a full name and returns initials
    /// </summary>
    public class FullNameToInitialsConverter : BaseValueConverter<FullNameToInitialsConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            else
                return GetInitials(value);
        }

        private object GetInitials(object value)
        {
            string name = value.ToString();
            // Extract the first character out of each block of non-whitespace
            // exept name suffixes, e.g. Jr., III.
            // The number of initials is not limited.
            Regex extractInitials = new Regex(@"(?i)(?:^|\s|-)+([^\s-])[^\s-]*(?:(?:\s+)(?:the\s+)?(?:jr|sr|II|2nd|III|3rd|IV|4th)\.?$)?");
            return extractInitials.Replace(name, "$1").ToUpper();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
