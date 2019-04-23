using System;
using System.Globalization;
using System.Windows;
using WinCopies.IO.FileProcesses;

namespace WinCopiesProcessesManager
{

    // todo : implémenter dans un datatrigger : - isBusy pas assez précis : quand le processus est terminé, mais qu'il y a des exceptions à gérer.

    public class ProcessStatusConverter : WinCopies.Util.DataConverters.ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is WinCopies.IO.FileProcesses.Process p) 

                if ((string)parameter == "ActionType")

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

                        case ActionType.Search:

                            return Application.Current.Resources["Search"];

                        default:

                            return null;

                    }

                else if ((string)parameter == "From")

                    return p.FilesInfoLoader.SourcePath;

                else if ((string)parameter == "To")

                    return ((CopyProcessInfo) p).DestPath; 

                else return null;

            else if (value is SevenZipCompressor c)

                if ((string)parameter == "ActionType")

                    return Application.Current.Resources["Compression"];

                else if ((string)parameter == "From")

                    return c.SourcePath;

                else if ((string)parameter == "To")

                    return c.DestPath;

            else if ((string)parameter == "ProgressStatus")

                return (bool)value ? WinCopies.GUI.Controls.ProcessStatus.Normal : WinCopies.GUI.Controls.ProcessStatus.None;

            else if ((string)parameter == "ProgressStatusPercent")

                return (byte)value;

                else if ((string)parameter == "DisplayProgressStatus")

                    return (bool)value ? Application.Current.Resources["ProcessIsRunning"] : Application.Current.Resources["ProcessCompletedSuccessfully"];

                else return null;

            else return null; 

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
