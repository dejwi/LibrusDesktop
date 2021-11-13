using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LibrusPlan.Converters
{
    public class deletepolish : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                return null;
            string txt = value.ToString();
            
            txt = txt.Replace("ś", "s");
            txt = txt.Replace("ą", "a");
            txt = txt.Replace("ó", "o");
            txt = txt.Replace("ę", "e");
            txt = txt.Replace("ż", "z");
            txt = txt.Replace("ź", "z");
            txt = txt.Replace("ć", "c");
            txt = txt.Replace("ń", "n");
            txt = txt.Replace("ł", "l");
            txt = txt.Replace("ś", "s");
            return txt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
