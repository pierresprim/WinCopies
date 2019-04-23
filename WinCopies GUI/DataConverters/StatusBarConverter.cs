using System;
using System.Globalization;

namespace WinCopies.GUI
{
    public class StatusBarConverter : Util.DataConverters.MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values[0] == null || values[1] == null
                ? string.Empty
                : string.Format("{0} éléments ; {1} éléments sélectionnés", values[0].ToString(), values[1].ToString());

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
