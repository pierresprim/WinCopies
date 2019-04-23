using System;
using System.Globalization;
using System.Windows;

namespace WinCopiesProcessesManager
{
    public class FileCheckFileContentConverter : WinCopies.Util.DataConverters.MultiConverterBase
    {

        private const string SameContent = "SameContent";

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => (bool)values[0]
                ? string.Format("{0} {1}", (string)Application.Current.Resources[SameContent], (string)Application.Current.Resources["Computing"])
                : ((bool?)values[1]).HasValue
                ? ((bool?)values[1]).Value ? string.Format("{0} {1}", (string)Application.Current.Resources[SameContent], (string)Application.Current.Resources["Yes"]) : string.Format("{0} {1}", (string)Application.Current.Resources[SameContent], (string)Application.Current.Resources["No"])
                : string.Format("{0} {1}", (string)Application.Current.Resources[SameContent], (string)Application.Current.Resources["NotComputedYet"]);

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
