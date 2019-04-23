using System;
using System.Globalization;
using System.Windows;
using WinCopies.IO;

namespace WinCopiesProcessesManager
{
    public class ShowCurrentCopiedFileInfoConverter : WinCopies.Util.DataConverters.MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG 
            foreach (object value in values)

                if (value == null || value == DependencyProperty.UnsetValue) return "";
#endif 

            if (values[0] is WinCopies.IO.FileProcesses. FileSystemInfo currentCopiedFile)

            {
                if (currentCopiedFile.FileType == WinCopies.IO.FileTypes.File)

                    return string.Format("{0} {1} %", currentCopiedFile.FileSystemInfoProperties.FullName, ((WinCopies.IO.Size)    values[1] / ((System.IO.FileInfo)currentCopiedFile.FileSystemInfoProperties).Length).GetValueInUnit(SizeUnit.Byte) * 100);

                else return currentCopiedFile.FileSystemInfoProperties.FullName;

            }

            else return values[0]; 
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
