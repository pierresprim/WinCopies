using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WinCopies.GUI
{
    public class TabItemIsSelectedConverter : Util.DataConverters.MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)

        {

            return values[0] == DependencyProperty.UnsetValue ? Binding.DoNothing : values[1];    

        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)

        {

            return new object[] { Binding.DoNothing, value } ;    

        } 
    }
}
