using System;
using System.Globalization;
using System.Windows;
using WinCopies.IO.FileProcesses;
using static WinCopies.Util.Util;

using IOProcess = WinCopies.IO.FileProcesses.Process;

namespace WinCopiesProcessesManager
{

    // todo : implémenter dans un datatrigger : - isBusy pas assez précis : quand le processus est terminé, mais qu'il y a des exceptions à gérer.

    public class ProcessStatusConverter : WinCopies.Util.Data.ConverterBase
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
                            case ActionType.Move:
                            case ActionType.Recycling:
                            case ActionType.Deletion:

                                string result = $"{Application.Current.Resources[p.ActionType.ToString()]} {Application.Current.Resources["from"]} {p.FilesInfoLoader.SourcePath}";

                                if (If(ComparisonType.Or, ComparisonMode.Logical, Comparison.Equal, p.ActionType, ActionType.Copy, ActionType.Move))

                                    result += $" {Application.Current.Resources["to"]} { ((CopyProcessInfo)p).DestPath}";

                                return result;

                            //case ActionType.Search:
                            //return Application.Current.Resources["Search"];

                            default:

                                return null;

                        }

                    else if (value is ISevenZipProcess sevenZipProcess)

                    {

                        string action = null;

                        string sourcePath = null;

                        if (value is SevenZipCompressor c)

                        {

                            action = (string)Application.Current.Resources["Compression"];

                            sourcePath = c.SourcePath;

                        }

                        else if (value is SevenZipExtractor e)

                        {

                            action = (string)Application.Current.Resources["Extraction"];

                            sourcePath = e.FileInfo.HasValue ? e.FileInfo.Value.FileName : null;

                        }

                        else

                            throw new ArgumentException("'value' is not a supported type.");

                        return $"{action} {Application.Current.Resources["from"]} {sourcePath} {Application.Current.Resources["to"]} {sevenZipProcess.DestPath}";

                    }

                    else

                        throw new ArgumentException("'value' is not a supported type.");

                case "ProgressStatus":

                    return (bool)value ? WinCopies.GUI.Controls.ProcessStatus.Normal
: WinCopies.GUI.Controls.ProcessStatus.None;

                case "ProgressStatusDisplay":

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
