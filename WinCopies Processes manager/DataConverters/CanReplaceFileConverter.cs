using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCopiesProcessesManager
{
    public class CanReplaceFileConverter : WinCopies.Util.Data.ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (WinCopies.IO.FileProcesses.Exceptions)value == WinCopies.IO.FileProcesses.Exceptions.FileAlreadyExists ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
