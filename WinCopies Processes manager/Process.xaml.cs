﻿using System.Windows.Controls;

namespace WinCopiesProcessesManager
{
    /// <summary>
    /// Logique d'interaction pour Process.xaml
    /// </summary>
    public partial class Process : Control
    {

        public Process() => InitializeComponent();

        private void Pause_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) =>

            // We don't need to check if the current process is not paused here because we've already checked it in the XAML view.

            e.CanExecute = e.Parameter is WinCopies.IO.FileProcesses.Process p ? /* !p.IsPaused && */ p.IsBusy :

            /*e.Parameter is ISevenZipProcess szp ? szp.IsBusy :*/ false;

        private void Pause_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

            if (e.Parameter is WinCopies.IO.FileProcesses.Process p)

                p.Suspend();

            //else if (e.Parameter is ISevenZipProcess szp)

            //    szp.CancellationPending = true;

        }

        private void Cancel_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => e.CanExecute = e.Parameter is WinCopies.IO.FileProcesses.Process p ? !p.IsCancelled && p.IsBusy : false;

        private void Cancel_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is WinCopies.IO.FileProcesses.Process p)

                p.CancelAsync();

            //else if (e.Parameter is ISevenZipProcess szp)

            //    szp.CancellationPending = true;
        }

        private void Resume_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) =>

            // We don't need to check if the current process is paused here because we've already checked it in the XAML view.

            e.CanExecute = true;

        private void Resume_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is WinCopies.IO.FileProcesses.Process p)

                p.Resume();

            //else if (e.Parameter is SevenZipCompressor c)

            //{

            //    SevenZip.SevenZipCompressor _c = (SevenZip.SevenZipCompressor)((ISevenZipProcessInternal)c).InnerProcess;

            //    _c.CompressionMode = SevenZip.CompressionMode.Append;

            //    App.BeginSevenZipProcess(c);

            //}
        }
    }
}
