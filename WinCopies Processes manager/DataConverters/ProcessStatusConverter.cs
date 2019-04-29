using System;
using System.Globalization;
using System.Windows;
using WinCopies.IO.FileProcesses;

using IOProcess = WinCopies.IO.FileProcesses.Process;

namespace WinCopiesProcessesManager
{

    // todo : implémenter dans un datatrigger : - isBusy pas assez précis : quand le processus est terminé, mais qu'il y a des exceptions à gérer.

    public class ProcessStatusConverter : WinCopies.Util.DataConverters.ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            IOProcess p = null;

            switch ((string)parameter)
            {

                case "ActionType":

                    if ((p = value as IOProcess) != null)

                        switch (p.ActionType)

                        {

                            case ActionType.Unknown:

                                return null;

                            case ActionType.Copy:
                                return Application.Current.Resources["Copy"];
                            case ActionType.Move:
                                return Application.Current.Resources["Move"];
                            case ActionType.MoveToRecycleBin:
                                return Application.Current.Resources["MoveToRecycleBin"];
                            case ActionType.Deletion:
                                return Application.Current.Resources["Deletion"];
                            //case ActionType.Search:
                            //return Application.Current.Resources["Search"];

                            default:

                                return null;

                        }

                    else if (value is SevenZipCompressor)

                        return Application.Current.Resources["Compression"];

                    else if (value is SevenZipExtractor)

                        // todo

                        return null;

                    else

                        return null;

                case "From":

                    return (p = value as IOProcess) != null ? p.FilesInfoLoader.SourcePath : value is SevenZipCompressor c ? c.SourcePath : value is SevenZipExtractor e ? e.FileInfo.HasValue ? e.FileInfo.Value.FileName
: null
: null;

                case "To":

                    return value is CopyProcessInfo copyProcessInfo ? copyProcessInfo.DestPath : value is ISevenZipProcess sevenZipProcess ? sevenZipProcess.DestPath : null;

                case "ProgressStatus":

                    return (bool)value ? WinCopies.GUI.Controls.ProcessStatus.Normal
: WinCopies.GUI.Controls.ProcessStatus.None;

                case "DisplayProgressStatus":

                    return (bool)value ? Application.Current.Resources["ProcessIsRunning"] : Application.Current.Resources["ProcessCompletedSuccessfully"];

                case "ProgressStatusPercent":

                    return (byte)value;

                default:

                    return null;

            }

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
