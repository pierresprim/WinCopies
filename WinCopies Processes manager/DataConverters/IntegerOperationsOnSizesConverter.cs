using System;
using System.Diagnostics;
using System.Globalization;

namespace WinCopiesProcessesManager
{
    public class IntegerOperationsOnSizesConverter : WinCopies.Util.Data.MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
#if DEBUG 
        {
            Debug.WriteLine(values[0].ToString() + " " + values[1].ToString());

            return (WinCopies.IO.Size)values[0] - (WinCopies.IO.Size)values[1]; 
        } 
#else 
        => (WinCopies.IO.Size)values[0] - (WinCopies.IO.Size)values[1];
#endif

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
