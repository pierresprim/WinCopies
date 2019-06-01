using System;
using System.Globalization;
using System.Windows;

namespace WinCopiesGUIWizard
{
    public class PageHeaderConverter : WinCopies.Util.Data.ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null)

                return null; 

            if (((TreeViewItem)value).Parent != null && ((TreeViewItem)value).Header == (string)Application.Current.Resources["Common"])

                return ((TreeViewItem)value).Parent.Header;

            else

                return ((TreeViewItem)value).Header;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
