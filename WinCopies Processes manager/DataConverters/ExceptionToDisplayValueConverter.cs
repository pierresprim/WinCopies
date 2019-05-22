using System;
using System.Globalization;
using System.Windows;

namespace WinCopiesProcessesManager
{
    public class ExceptionToDisplayValueConverter : WinCopies.Util.DataConverters.ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ex = (WinCopies.IO.FileProcesses.Exceptions)value;

            switch (ex)

            {

                case WinCopies.IO.FileProcesses.Exceptions.Unknown:

                    return Application.Current.Resources["UnknownExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.FileNotFound:

                    return Application.Current.Resources["FileNotFoundExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.PathNotFound:

                    return Application.Current.Resources["PathNotFoundExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.AccessDenied:

                    return Application.Current.Resources["AccessDeniedExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.DirectoryCannotBeRemoved:

                    return Application.Current.Resources["DirectoryCannotBeRemovedExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.WriteProtected:

                    return Application.Current.Resources["WriteProtectedExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.ExceptionOnDeviceUnit:

                    return Application.Current.Resources["ExceptionOnDeviceUnitExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.DiskNotReady:

                    return Application.Current.Resources["DiskNotReadyExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.FileAlreadyExists:

                    return Application.Current.Resources["FileAlreadyExistsExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.FileNameTooLong:

                    return Application.Current.Resources["FileNameTooLongExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.DiskFull:

                    return Application.Current.Resources["DiskFullExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.InvalidName:

                    return Application.Current.Resources["InvalidNameExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.FileCheckedOut:

                    return Application.Current.Resources["FileCheckedOutExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.FileTooLarge:

                    return Application.Current.Resources["FileTooLargeExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.DiskTooFragmented:

                    return Application.Current.Resources["DiskTooFragmentedExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.DeletePending:

                    return Application.Current.Resources["DeletePendingExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.NotAllowedOnSystemFile:

                    return Application.Current.Resources["NotAllowedOnSystemFileExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.DestPathIsASubdirectory:

                    return Application.Current.Resources["DestPathIsASubdirectoryExceptionMessage"];

                case WinCopies.IO.FileProcesses.Exceptions.NotEnoughSpaceOnDisk:

                    return Application.Current.Resources["NotEnoughSpaceOnDiskExceptionMessage"];

                default:

                    return Application.Current.Resources["UnknownExceptionMessage"];

            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
