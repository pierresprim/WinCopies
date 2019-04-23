using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WinCopiesProcessesManager
{
    /// <summary>
    /// Logique d'interaction pour CopyProcess.xaml
    /// </summary>
    public partial class CopyProcess : Control
    {
        private DispatcherTimer dispatcherTimer = null;

        private WinCopies.IO.Size PreviousCopiedSize = (WinCopies.IO.Size)0;

        private static readonly DependencyPropertyKey SpeedPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Speed), typeof(WinCopies.IO.Size), typeof(CopyProcess), new PropertyMetadata(new WinCopies.IO.Size(0, WinCopies.IO.SizeUnit.Byte)));

        public static readonly DependencyProperty SpeedProperty = SpeedPropertyKey.DependencyProperty;

        public WinCopies.IO.Size Speed => (WinCopies.IO.Size)GetValue(SpeedProperty);

        private static readonly DependencyPropertyKey EstimatedTimeRemainingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(EstimatedTimeRemaining), typeof(string), typeof(CopyProcess), new PropertyMetadata(null));

        public static readonly DependencyProperty EstimatedTimeRemainingProperty = EstimatedTimeRemainingPropertyKey.DependencyProperty;

        public string EstimatedTimeRemaining => (string)GetValue(EstimatedTimeRemainingProperty);

        public static readonly DependencyProperty ActionOnExceptionProperty = DependencyProperty.Register(nameof(ActionOnException), typeof(HowToRetry), typeof(CopyProcess), new PropertyMetadata(HowToRetry.None));

        public HowToRetry ActionOnException { get => (HowToRetry)GetValue(ActionOnExceptionProperty); set => SetValue(ActionOnExceptionProperty, value); }

        public static readonly DependencyProperty DoActionForAllExceptionsProperty = DependencyProperty.Register(nameof(DoActionForAllExceptions), typeof(bool), typeof(CopyProcess), new PropertyMetadata(false));

        public bool DoActionForAllExceptions { get => (bool)GetValue(DoActionForAllExceptionsProperty); set => SetValue(DoActionForAllExceptionsProperty, value); }

        // public static readonly DependencyProperty CopiedSizeProperty = DependencyProperty.Register("CopiedSize", typeof(WinCopies.IO.FilesProcesses.Size), typeof(Process));

        // public WinCopies.IO.FilesProcesses.Size CopiedSize { get => (WinCopies.IO.FilesProcesses.Size)GetValue(CopiedSizeProperty); set => SetValue(CopiedSizeProperty, value); }

        public CopyProcess() => InitializeComponent();

        private void Process_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            // var p = (WinCopies.IO.FileProcesses.CopyProcessInfo)DataContext;

            //if (p.ExceptionsOccurred)

            //{

            //    Type type = typeof(WinCopies.IO.FileProcesses.Exceptions);

            //    foreach (string _s in type.GetEnumNames())

            //    {

            //        Enum enumValue = (Enum)Enum.Parse(type, _s);



            //        var items = p.Exceptions.Where((WinCopies.IO.FileProcesses.FileSystemInfo f) => f.Exception == (WinCopies.IO.FileProcesses.Exceptions)enumValue);







            //        // s += item.FileSystemInfoProperties.FullName + " " + item.FileType.ToString() + " " + item.Exception.ToString() + "\n";



            //        //if (flagsEnum.HasFlag(enumValue))

            //        //{

            //        //    if (!alreadyFoundAFlag) alreadyFoundAFlag = true;

            //        //    else return true;

            //        //}

            //    }

            //}

            // p.ExceptionsToRetry = WinCopies.IO.FilesProcesses.Exceptions.FileAlreadyExists;

            // p.startCopy();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            var copiedSize = ((WinCopies.IO.FileProcesses.CopyProcessInfo)DataContext).CurrentCopiedSize;

            var result = copiedSize - PreviousCopiedSize;

            SetValue(SpeedPropertyKey, result);

            var a = (((WinCopies.IO.FileProcesses.CopyProcessInfo)DataContext).FilesInfoLoader.TotalSize / result).GetValueInUnit(WinCopies.IO.SizeUnit.Byte);

            SetValue(EstimatedTimeRemainingPropertyKey,

                result > 0 ?

                TimeSpan.FromSeconds(((((WinCopies.IO.FileProcesses.CopyProcessInfo)DataContext).FilesInfoLoader.TotalSize - copiedSize) / result).GetValueInUnit(WinCopies.IO.SizeUnit.Byte)).ToString()

                :

                "NaN");

            PreviousCopiedSize = copiedSize;
        }

        private void Control_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            var p = (WinCopies.IO.FileProcesses.CopyProcessInfo)DataContext;

            // p.DoWork += Process_DoWork;

            p.RunWorkerCompleted += Process_RunWorkerCompleted;

            dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            dispatcherTimer.Tick += DispatcherTimer_Tick;

            dispatcherTimer.Start();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var cpi = (WinCopies.IO.FileProcesses.CopyProcessInfo)DataContext;

            var actionOnException = ActionOnException;

            if (actionOnException == HowToRetry.CheckFiles)

            {

                FileCheck fileCheckWindow = new FileCheck(cpi);    

                // cpi.Exceptions

                fileCheckWindow.ShowDialog();

                return;

            }

            if (actionOnException == HowToRetry.Cancel || DoActionForAllExceptions)

            {

                cpi.ExceptionsToRetry = cpi.Exceptions[0].Exception;

                cpi.HowToRetryWhenExceptionOccured = (WinCopies.IO.FileProcesses.HowToRetry)actionOnException;

                cpi.StartCopy();

            }

            else

            {

                cpi.ExceptionsToRetry = cpi.Exceptions[0].Exception;

                cpi.Exceptions[0].HowToRetryToProcess = (WinCopies.IO.FileProcesses.HowToRetry)actionOnException;

                cpi.StartCopy(true);

            }

            ActionOnException = HowToRetry.None;

        }

        // private void Process_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        // {
        // if (!Application.Current.Dispatcher.CheckAccess())
        // Application.Current.Dispatcher.InvokeAsync(() => Process_DoWork(sender, e)); // sorry for InvokeAsync :)
        // else

        // ... your code goes here without need to use invoke

        // SetValue(IsBusyPropertyKey, true);
        // }
    }
}
