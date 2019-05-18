using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCopies.IO.FileProcesses;

namespace WinCopiesProcessesManager
{
    public class ActionTypeToVisibilityConverter : WinCopies.Util.DataConverters.ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((ActionType)value) == ActionType.Deletion ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
