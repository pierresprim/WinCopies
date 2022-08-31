/* Copyright © Pierre Sprimont, 2020
 *
 * This file is part of the WinCopies Framework.
 *
 * The WinCopies Framework is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The WinCopies Framework is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

using WinCopies.GUI.Windows;
using WinCopies.Util.Data;
using static System.Resources.ResXFileRef;
using static WinCopies.About;

namespace WinCopies
{
    public struct FrameworkVersions
    {
        public string WinCopiesUtilities { get; }

        public string WinCopies { get; }

        public string WindowsAPICodePack { get; }

        public FrameworkVersions()
        {
#if DEBUG
            WinCopiesUtilities = null;
            WinCopies = null;
            WindowsAPICodePack = null;
#else
            string assemblyDirectory = IPCService.Extensions.SingleInstanceApp.GetAssemblyDirectory();

            string getVersion(in string assemblyName) => Assembly.LoadFile($"{assemblyDirectory}\\{assemblyName}.dll").GetName().Version.ToString();

            WinCopiesUtilities = getVersion($"{nameof(WinCopies)}.Util");

            WinCopies = getVersion($"{nameof(WinCopies)}.IO");

            WindowsAPICodePack = getVersion($"{nameof(WinCopies)}.{nameof(WindowsAPICodePack)}");
#endif
        }
    }

    public partial class About : DialogWindow
    {
        public interface ICommand : System.Windows.Input.ICommand
        {
            bool IsEnabled { get; }

            string Text { get; }
        }

        public abstract class UpdateViewModelBase : ViewModelBase
        {
            private ICommand _command;

            public ICommand Command { get => _command; internal set => UpdateValue(ref _command, value, nameof(Command)); }

            public abstract Version Version { get; }

            public UpdateViewModelBase() => _command = GetCommand();

            protected abstract ICommand GetCommand();
        }

        public abstract class UpdateCommandBase<T> : ICommand where T : UpdateViewModelBase
        {
            private class CancelUpdateCommand : ViewModelBase, ICommand, System.IDisposable
            {
                private readonly CancellationTokenSource _cancellationTokenSource = new();
                private string _text = "Cancel";

                public string Text { get => _text; private set => UpdateValue(ref _text, value, nameof(Text)); }
                public bool IsEnabled => CanExecute(null);

                public event EventHandler? CanExecuteChanged;

                public CancelUpdateCommand(UpdateCommandBase<T> command, in ConverterIn<CancellationToken, Task> taskProvider)
                {
                    Task task = taskProvider(_cancellationTokenSource.Token);

                    _ = task.ContinueWith(task => command.ViewModel.Command = command);

                    task.Start();

                    command.ViewModel.Command = this;
                }

                public bool CanExecute(object? parameter) => !_cancellationTokenSource.Token.IsCancellationRequested;
                public void Execute(object? parameter)
                {
                    _cancellationTokenSource.Cancel();

                    OnPropertyChanged(nameof(IsEnabled));

                    CanExecuteChanged?.Invoke(this, System.EventArgs.Empty);

                    Text = "Cancelling...";
                }

                public void Dispose() => _cancellationTokenSource.Dispose();
            }

            protected T ViewModel { get; }

            protected abstract char Command { get; }

            public abstract string Text { get; }
            public bool IsEnabled => CanExecute(null);

            public event EventHandler? CanExecuteChanged = null;

            public UpdateCommandBase(in T viewModel) => ViewModel = viewModel;

            public bool CanExecute(object? parameter) => true;

            protected abstract void DoWork(Process process, in CancellationToken token);

            public void Execute(object? parameter)
            {
                static Task getTask(ActionIn<CancellationToken> action, CancellationToken token) => new(() => action(token));

                ViewModel.Command = new CancelUpdateCommand(this, (in CancellationToken token) => getTask((in CancellationToken token) =>
                {
                    if (token.IsCancellationRequested)

                        return;

                    try
                    {
                        string? updaterPath = Environment.GetEnvironmentVariable("WinCopiesUpdaterPath");

                        if (string.IsNullOrEmpty(updaterPath))

                            return;

                        var startInfo = new ProcessStartInfo(updaterPath, Command.ToString())
                        {
                            UseShellExecute = false,
                            RedirectStandardOutput = true
                        };

                        using var process = new Process();

                        DoWork(process, token);
                    }

                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }, token));
            }
        }

        public class UpdateViewModel : UpdateViewModelBase
        {
            private class UpdateCommand : UpdateCommandBase<UpdateViewModelBase>
            {
                protected override char Command => 'u';

                public override string Text => "Update";

                public UpdateCommand(in UpdateViewModelBase viewModel) : base(viewModel) { /* Left empty. */ }

                protected override void DoWork(Process process, in CancellationToken token) => process.Start();
            }

            private Version _version;

            public override Version Version => _version;

            protected override ICommand GetCommand() => new UpdateCommand(this);

            internal void SetVersion(in Version version) => UpdateValue(ref _version, version, nameof(Version));
        }

        public class UpdateCheckViewModel : UpdateViewModelBase
        {
            private class UpdateCheckCommand : UpdateCommandBase<UpdateCheckViewModel>
            {
                protected override char Command => 'c';

                public override string Text => "Check for updates";

                public UpdateCheckCommand(in UpdateCheckViewModel viewModel) : base(viewModel) { /* Left empty. */ }

                protected override void DoWork(Process process, in CancellationToken token)
                {
                    void onDataReceived(object sender, DataReceivedEventArgs e)
                    {
                        string[] data = e.Data.Split();
                        var values = new uint[4];
                        byte i = 0;

                        for (; i < 4; i++)

                            values[i] = uint.Parse(data[i]);

                        i = 0;

                        uint getValue() => values[i++];

                        ViewModel.UpdateViewModel.SetVersion(new Version(getValue(), getValue(), getValue(), getValue()));

                        process.CancelOutputRead();
                        process.OutputDataReceived -= onDataReceived;
                    }

                    process.OutputDataReceived += onDataReceived;

                    process.BeginOutputReadLine();
                    _ = process.Start();

                    Task task = process.WaitForExitAsync(token);
                }
            }

            internal UpdateViewModel UpdateViewModel { get; }

            public override Version Version => (Version)Assembly.GetExecutingAssembly().GetName().Version;

            public UpdateCheckViewModel(in UpdateViewModel updateViewModel) => UpdateViewModel = updateViewModel;

            protected override ICommand GetCommand() => new UpdateCheckCommand(this);
        }

        private static DependencyPropertyKey RegisterReadOnly<T>(in string propertyName, in T value) => Util.Desktop.UtilHelpers.RegisterReadOnly<T, About>(propertyName, new PropertyMetadata(value));

        private static readonly DependencyPropertyKey FrameworkVersionsPropertyKey = RegisterReadOnly(nameof(FrameworkVersions), new FrameworkVersions());

        public static readonly DependencyProperty FrameworkVersionsProperty = FrameworkVersionsPropertyKey.DependencyProperty;

        public FrameworkVersions FrameworkVersions => (FrameworkVersions)GetValue(FrameworkVersionsProperty);

        public UpdateCheckViewModel UpdateCheckCommand { get; }

        public UpdateViewModel UpdateCommand { get; } = new UpdateViewModel();

        public static FlowDocument Document
        {
            get
            {
                var document = new FlowDocument();

                var s = new MemoryStream();
                var w = new StreamWriter(s);

                w.Write(Properties.Resources.gpl_3_0);
                w.Flush();

                new TextRange(document.ContentStart, document.ContentEnd).Load(s, DataFormats.Rtf);

                return document;
            }
        }

        public About()
        {
            UpdateCheckCommand = new UpdateCheckViewModel(UpdateCommand);

            DataContext = this;

            InitializeComponent();
        }
    }
}
