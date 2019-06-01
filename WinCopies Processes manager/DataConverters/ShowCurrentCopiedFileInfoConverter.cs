using System;
using System.Globalization;
using System.Windows;
using WinCopies.IO;

namespace WinCopiesProcessesManager
{
    public class ShowCurrentCopiedFileInfoConverter : WinCopies.Util.Data.MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG 
            foreach (object value in values)

                if (value == null || value == DependencyProperty.UnsetValue) return "";
#endif 

            return values[0] is WinCopies.IO.FileProcesses. FileSystemInfo currentCopiedFile
                ? currentCopiedFile.FileType == FileType.File
                    ? string.Format("{0} {1} %", currentCopiedFile.FileSystemInfoProperties.FullName, ((WinCopies.IO.Size)    values[1] / ((System.IO.FileInfo)currentCopiedFile.FileSystemInfoProperties).Length).GetValueInUnit(SizeUnit.Byte) * 100)
                    : currentCopiedFile.FileSystemInfoProperties.FullName
                : values[0];
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
