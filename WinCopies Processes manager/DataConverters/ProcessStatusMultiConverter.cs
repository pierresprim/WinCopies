using System;
using System.Globalization;
using System.Windows;
using WinCopies.Util.DataConverters;

namespace WinCopiesProcessesManager
{
    public class ProcessStatusMultiConverter : MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => (string)parameter == "ProgressStatus"
                ? (bool)values[0] || (bool)values[1]
                    ? !(bool)values[0] && (bool)values[1]
                        ? WinCopies.GUI.Controls.ProcessStatus.Error
                        : (object)WinCopies.GUI.Controls.ProcessStatus.Normal
                    : WinCopies.GUI.Controls.ProcessStatus.None
                : (string)parameter == "ProgressStatusPercent"
                ? (WinCopies.IO.Size)values[1] > 0 ? ((WinCopies.IO.Size)values[0] / (WinCopies.IO.Size)values[1] * 100).Value : 0d
                : (string)parameter == "DisplayProgressStatus"
                ? (bool)values[0]
                    ? !(bool)values[1]
                    ? Application.Current.Resources["ProcessIsRunning"]
                    : Application.Current.Resources["ProcessIsPaused"]
                    : !(bool)values[2]
                    ? Application.Current.Resources["ProcessCompletedSuccessfully"]
                    : Application.Current.Resources["ProcessCompletedWithExceptions"]
                : null;

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
