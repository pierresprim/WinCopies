﻿/* Copyright © Pierre Sprimont, 2020
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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IPCService.Client;
using WinCopies.IPCService.Hosting;

using static WinCopies.ThrowHelper;

namespace WinCopies
{
    public partial class App : Application/*, INotifyPropertyChanged, ISingleInstanceApp*/
    {
        public static IProcessPathCollectionFactory DefaultProcessPathCollectionFactory { get; } = new ProcessPathCollectionFactory();

        private unsafe delegate void Action(int* i);

        public static string GetAssemblyDirectory() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        internal static void StartInstance(in System.Collections.Generic.IEnumerable<string> parameters) => System.Diagnostics.Process.Start(GetAssemblyDirectory() + "\\WinCopies.exe", parameters);

        internal static System.Collections.Generic.IEnumerable<string> GetProcessParameters(IProcessParameters processParameters)
        {
            yield return "Process";

            yield return processParameters.Guid.ToString();

            foreach (string parameter in processParameters.Parameters)

                yield return parameter;
        }

        static unsafe void AddPath(in string[] args, in IQueue<string> paths, int* i) => paths.Enqueue(args[(*i)++]);

        static unsafe System.Collections.Generic.IEnumerable<string> getArray(string[] args, ref ArrayBuilder<string> arrayBuilder, int* i)
        {
            if (arrayBuilder == null)

                arrayBuilder = new ArrayBuilder<string>();

            else

                arrayBuilder.Clear();

            foreach (string value in new Enumerable<string>(() => new ArrayEnumerator(args, i)).TakeWhile(arg => arg is not ("Process" or "Path")))

                _ = arrayBuilder.AddLast(value);

            (*i)++;

            return arrayBuilder.ToArray();
        }

        private static unsafe void AddProcess(in string[] args, in IQueue<IProcessParameters> processes, ref ArrayBuilder<string> arrayBuilder, int* i) => processes.Enqueue(new ProcessParameters(args[*i], getArray(args, ref arrayBuilder, i)));

        private static unsafe void RunAction(in Action action, int* i)
        {
            (*i)++;

            action(i);
        }

        public static unsafe void GetQueues(string[] args, IQueue<string> paths, IQueue<IProcessParameters> processes)
        {
            ArrayBuilder<string> arrayBuilder = null;

            for (int i = 0; i < args.Length;)

                if (args[i] == "Process")

                    RunAction(i => AddProcess(args, processes, ref arrayBuilder, i), &i);

                else if (args[i] == "Path")

                    RunAction(i => AddPath(args, paths, i), &i);

                else

                    return;

            arrayBuilder?.Clear();
        }

        [STAThread]
        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Run(queue: null);

                return;
            }

            var pathQueue = new Collections.DotNetFix.Generic.Queue<string>();
            var processQueue = new Collections.DotNetFix.Generic.Queue<IProcessParameters>();

            GetQueues(args, pathQueue, processQueue);

            if (processQueue.Count == 0)
            {
                Run(pathQueue);

                return;
            }

            if (pathQueue.Count != 0)
            {
                System.Collections.Generic.IEnumerable<string> pathsToEnumerable()
                {
                    while (pathQueue.Count != 0)
                    {
                        yield return "Path";

                        yield return pathQueue.Dequeue();
                    }
                }

                StartInstance(pathsToEnumerable());
            }

            using var mutex = new Mutex(false, "65cae396-971a-4545-97e7-83d4ed042d92");

            if (mutex.WaitOne(0))
            {
                var t = new Thread(() =>
                {
                    var app = new App
                    {
                        MainWindow = new ProcessWindow()
                    };

                    app._processQueue = processQueue;

                    _ = app.Run();
                });

                t.SetApartmentState(ApartmentState.STA);

                t.Start();

                Host.CreateDefaultBuilder()
                   .ConfigureServices(services => services.AddScoped<IProcessCollectionUpdater, ProcessCollectionUpdater>())
                   .ConfigureIPCHost(builder => builder.AddNamedPipeEndpoint<IProcessCollectionUpdater>("65cae396-971a-4545-97e7-83d4ed042d92"))
                   .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Debug)).Build().Run();
            }

            else
            {
                ServiceProvider serviceProvider = new ServiceCollection()
                    .AddNamedPipeClient<IProcessCollectionUpdater>(ClientVersion.ClientName, pipeName: "65cae396-971a-4545-97e7-83d4ed042d92")
                    .BuildServiceProvider();

                IClientFactory<IProcessCollectionUpdater> clientFactory = serviceProvider.GetRequiredService<IClientFactory<IProcessCollectionUpdater>>();

                IClient<IProcessCollectionUpdater> client = clientFactory.CreateClient(ClientVersion.ClientName);

                var processes = new ArrayBuilder<string>();

                while (processQueue.Count != 0)

                    foreach (string value in GetProcessParameters(processQueue.Dequeue()))

                        _ = processes.AddLast(value);

                string[] _processes = processes.ToArray();

                await client.InvokeAsync(item => item.Run(_processes));
            }
        }

        private IQueue<IProcessParameters> _processQueue;

        private static void Run(IQueue<string> queue)
        {
            var t = new Thread(() =>
            {
                var app = new App();

                app.OpenWindows = new UIntCountableProvider<Window, IEnumeratorInfo2<Window>>(() => new EnumeratorInfo<Window>(app._OpenWindows), () => app._OpenWindows.Count);

                app.MainWindow = new MainWindow();

                System.Collections.ObjectModel.ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> paths = ((MainWindowViewModel)(app.MainWindow).DataContext).Paths;

                if (queue != null)
                {
                    while (queue.Count != 0)

                        paths.Add(ExplorerControlBrowsableObjectInfoViewModel.From(new BrowsableObjectInfoViewModel(ShellObjectInfo.From(queue.Dequeue(), ClientVersion))));
                }

                else

                    paths.Add(GetDefaultExplorerControlBrowsableObjectInfoViewModel());

                // app.MainWindow = new MainWindow();

                // System.Windows.Application.LoadComponent(app.Resources, new Uri("/wincopies;component/ResourceDictionary.xaml", UriKind.Relative));

                _ = app.Run();
            });

            t.SetApartmentState(ApartmentState.STA);

            t.Start();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _OpenWindows.CollectionChanged += _OpenWindows_CollectionChanged;

            Resources = new ResourceDictionary() { Source = new Uri("ResourceDictionary.xaml", UriKind.Relative) };

            //MainWindow = new MainWindow();

            //MainWindow.Closed += MainWindow_Closed;

            GUI.IO.ObjectModel.BrowsableObjectInfo.RegisterDefaultSelectors();

            if (_processQueue != null)

                Run(_processQueue);

            MainWindow.Show();
        }

        public static void Run(IQueue<IProcessParameters> processQueue)
        {
            while (processQueue.Count != 0)

                Current.Dispatcher.Invoke(() => ProcessCollectionUpdater.Instance.Processes.Add(new Process(IO.ObjectModel.BrowsableObjectInfo.DefaultProcessSelectorDictionary.Select(new ProcessFactorySelectorDictionaryParameters(processQueue.Dequeue(), App.DefaultProcessPathCollectionFactory)))));
        }

        private void _OpenWindows_CollectionChanged(object sender, LinkedCollectionChangedEventArgs<Window> e) => Environment.Exit(0);

        // TODO:
        public class CustomEnumeratorProvider<TItems, TEnumerator> : System.Collections.Generic.IEnumerable<TItems> where TEnumerator : System.Collections.Generic.IEnumerator<TItems>
        {
            protected Func<TEnumerator> Func { get; }

            public CustomEnumeratorProvider(in Func<TEnumerator> func) => Func = func;

            public TEnumerator GetEnumerator() => Func();

            System.Collections.Generic.IEnumerator<TItems> System.Collections.Generic.IEnumerable<TItems>.GetEnumerator() => Func();

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Func();
        }

        public class UIntCountableProvider<TItems, TEnumerator> : CustomEnumeratorProvider<TItems, TEnumerator>, IUIntCountableEnumerable<TItems> where TEnumerator : IEnumeratorInfo2<TItems>
        {
            private Func<uint> CountFunc { get; }

            uint IUIntCountable.Count => CountFunc();

            public UIntCountableProvider(in Func<TEnumerator> func, in Func<uint> countFunc) : base(func) => CountFunc = countFunc;

            IUIntCountableEnumerator<TItems> IUIntCountableEnumerable<TItems, IUIntCountableEnumerator<TItems>>.GetEnumerator() => new UIntCountableEnumeratorInfo<TItems>(GetEnumerator(), CountFunc);
        }

        public static ClientVersion ClientVersion { get; } = new ClientVersion(Assembly.GetExecutingAssembly().GetName());

        public bool IsClosing { get; internal set; }

        internal ObservableLinkedCollection<Window> _OpenWindows { get; } = new ObservableLinkedCollection<Window>();

        public IUIntCountableEnumerable<Window> OpenWindows { get; internal set; }

        public static new App Current => (App)Application.Current;

        public static IExplorerControlBrowsableObjectInfoViewModel GetDefaultExplorerControlBrowsableObjectInfoViewModel(in IBrowsableObjectInfo browsableObjectInfo)
        {
            IExplorerControlBrowsableObjectInfoViewModel viewModel = ExplorerControlBrowsableObjectInfoViewModel.From(new BrowsableObjectInfoViewModel(browsableObjectInfo));

            return viewModel;
        }

        public static IExplorerControlBrowsableObjectInfoViewModel GetDefaultExplorerControlBrowsableObjectInfoViewModel() => GetDefaultExplorerControlBrowsableObjectInfoViewModel(new ShellObjectInfo(KnownFolders.Desktop, ClientVersion));

#if !WinCopies4
        public class ArrayEnumerator<T> : Enumerator<T>, ICountableDisposableEnumeratorInfo<T>
        {
            private System.Collections.Generic.IReadOnlyList<T> _array;
            private readonly unsafe int* _currentIndex;
            private readonly int _startIndex;
            private Func<bool> _condition;
            private System.Action _moveNext;

            protected System.Collections.Generic.IReadOnlyList<T> Array => IsDisposed ? throw GetExceptionForDispose(false) : _array;

            public int Count => IsDisposed ? throw GetExceptionForDispose(false) : _array.Count;

            protected unsafe int CurrentIndex => IsDisposed ? throw GetExceptionForDispose(false) : *_currentIndex;

            public unsafe ArrayEnumerator(in System.Collections.Generic.IReadOnlyList<T> array, in bool reverse = false, int* startIndex = null)
            {
                _array = array ?? throw GetArgumentNullException(nameof(array));

                if (startIndex != null && (*startIndex < 0 || *startIndex >= array.Count))

                    throw new ArgumentOutOfRangeException(nameof(startIndex), *startIndex, $"The given index is less than zero or greater than or equal to {nameof(array.Count)}.");

                _currentIndex = startIndex;

                if (reverse)
                {
                    _startIndex = startIndex == null ? _array.Count - 1 : *startIndex;
                    _condition = () => *_currentIndex > 0;
                    _moveNext = () => (*_currentIndex)--;
                }

                else
                {
                    _startIndex = startIndex == null ? 0 : *startIndex;
                    _condition = () => *_currentIndex < _array.Count - 1;
                    _moveNext = () => (*_currentIndex)++;
                }
            }

            protected override unsafe T CurrentOverride => _array[*_currentIndex];

            public override bool? IsResetSupported => true;

            protected override bool MoveNextOverride()
            {
                if (_condition())
                {
                    _moveNext();

                    return true;
                }

                return false;
            }

            protected override unsafe void ResetCurrent() => *_currentIndex = _startIndex;

            protected override void DisposeManaged()
            {
                _array = null;
                _condition = null;
                _moveNext = null;

                Reset();
            }
        }
#endif

        public class ArrayEnumerator : ArrayEnumerator<string>
        {
            public unsafe ArrayEnumerator(in System.Collections.Generic.IReadOnlyList<string> array, int* startIndex = null) : base(array, false, startIndex) { }

            protected override void ResetCurrent() { }
        }

        /*protected virtual void OnPropertyChanged(string propertyName, string fieldName, object newValue, Type declaringType)

        {

            (bool propertyChanged, object oldValue) = ((INotifyPropertyChanged)this).SetProperty(propertyName, fieldName, newValue, declaringType);

            if (propertyChanged) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // OnPropertyChanged(propertyName, result.oldValue, newValue);

        }

         protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

         todo:

        private const string Unique = "08248566-c8c4-4b19-96b7-489fe3b1049b";

                private FileSystemWatcher FileSystemWatcher { get; set; } = new FileSystemWatcher(Path.GetDirectoryName(SavePath)) { NotifyFilter = NotifyFilters.LastWrite, IncludeSubdirectories = false, Filter = "settings.settings", EnableRaisingEvents = true };

                public bool IsFirstInstance { get; private set; } = false;

        #if DEBUG
                private ObservableCollection<string> _args = null;

                public ObservableCollection<string> Args { get => _args; set { _args = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Args))); } }
        #endif

        public Common CommonProperties { get; } = new Common(true);

        public Ergonomics ErgonomicsProperties { get; } = new Ergonomics(true);

        public IOOperations IOOperationsProperties { get; } = new IOOperations(true);

        public event PropertyChangedEventHandler PropertyChanged;

        public App()

        {

            foreach (CheckableObject item in CommonProperties.KnownExtensionsToOpenDirectly)

                item.PropertyChanged += Item_PropertyChanged;

        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            // todo

        }

        static async System.Threading.Tasks.Task DefaultLaunchAsync()
        {
            // Path to the file in the app package to launch
            string imageFile = "C:\\Users\\pierr\\Desktop\\DVD2-1218-640.png";

            var file = StorageFile.GetFileFromPathAsync(imageFile);

            file.Completed = (IAsyncOperation<StorageFile> asyncInfo, AsyncStatus asyncStatus) =>
            {
                var sto = asyncInfo.GetResults();
                if (sto != null)
                {
                    // Set the option to show the picker
                    var options = new LauncherOptions
                    {

                        DisplayApplicationPicker = false
                    };

                    //options.PreferredApplicationPackageFamilyName = "Capture d'écran et croquis";

                    // Launch the retrieved file
                    var result = Launcher.LaunchFileAsync(sto, options);
                    result.Completed = (IAsyncOperation<bool> _asyncInfo, AsyncStatus _asyncStatus) =>
                    {

                        bool r = _asyncInfo.GetResults();

                    };
                    //if (success)
                    //{
                    //    // File launched
                    //}
                    //else
                    //{
                    //    // File launch failed
                    //}
                }
                else
                {
                    // Could not find file
                }
            };
        }

        private static Dictionary<string, IReadOnlyList<AppInfo>> _appInfos = new Dictionary<string, IReadOnlyList<AppInfo>>();

        public static ReadOnlyDictionary<string, IReadOnlyList<AppInfo>> AppInfos { get; } = new ReadOnlyDictionary<string, IReadOnlyList<AppInfo>>(_appInfos);

        [STAThread]
        public static void Main()

        {

            Debug.WriteLine("System.Security.Principal.WindowsIdentity.GetCurrent().User.AccountDomainSid: " + System.Security.Principal.WindowsIdentity.GetCurrent().User.Value);

            // Process.Start("ms-windows-store://navigatetopage/?Id=Games");

            // Process.Start( new ProcessStartInfo( "ms-photos:viewer?filename=\"c:\\users\\pierr\\desktop\\dvd2-1218-640.png\""));

            // DefaultLaunchAsync();

            // var activation = (IApplicationActivationManager)new ApplicationActivationManager();

                    if (SingleInstance<App>.InitializeAsFirstInstance(Unique))

                    {

                        SplashScreen splashScreen = new SplashScreen("/SplashScreen.png");

                        splashScreen.Show(false);

                        application = new App();

                        application.IsFirstInstance = true;

        #if DEBUG 
                        application.Args = new ObservableCollection<string>();
                        application.Args.CollectionChanged += Args_CollectionChanged;
        #endif

                        application.Startup += Application_Startup;
                        application.InitializeComponent();

                        application.MainWindow = new MainWindow();

                        splashScreen.Close(new TimeSpan(30000000));
                        application.Run();

                        SingleInstance<App>.Cleanup();

                    }

                    else
                    {

                        SplashScreen splashScreen = new SplashScreen("/SplashScreen.png");

                        splashScreen.Show(false);

                        App app = new App();

                        app.Startup += Application_Startup;

                        app.InitializeComponent();

                        MainWindow mainWindow = new MainWindow();
                        app.MainWindow = mainWindow;
                        // mainWindow.Show();

                        splashScreen.Close(new TimeSpan(30000000));

                        app.Run();
                    }

                }

        #if DEBUG
                private static void Args_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => Console.WriteLine(e.NewItems[0].ToString());
        #endif

                private void AddNewArgs(IList<string> new_args)

                {

        #if DEBUG 
                    foreach (string new_arg in new_args)

                        Args.Add(new_arg);
        #endif

                    OnNewArgsAdded(new_args);

                }

        private void OnNewArgsAdded(IList<string> new_args)

        {

            if (new_args[0] == "/select")

            {

                if (new_args.Count > 1)

                {

                    for (int i = 1; i < new_args.Count; i++)

                        new_args[i] = new_args[i].Replace("/", "\\");

                    string path = new_args[1];

                    MainWindow mainWindow = (MainWindow)application.MainWindow;

                    bool alreadyPathOpen = false;

                    foreach (IBrowsableObjectInfo browsableObject in mainWindow.Items)

                        if (browsableObject.Path == path)

                        {

                            alreadyPathOpen = true;

                            browsableObject.IsSelected = true;

                            break;

                        }

                    if (!alreadyPathOpen)

                        mainWindow.New_Tab(new ValueObject<IBrowsableObjectInfo>(new ShellObjectInfo(ShellObject.FromParsingName(path), path)));

                    foreach (ValueObject<IBrowsableObjectInfo> shellObject in

                        mainWindow.SelectedItem.Value.Items)

                        for (int i = 2; i <= new_args.Count - 1; i++)

                            if (new_args[i] == Path.GetFileName(shellObject.Value.Path))

                                shellObject.Value.IsSelected = true;

                }

            }

            else

            {

                MainWindow mainWindow = (MainWindow)application.MainWindow;

                foreach (string arg in new_args)

                {

                    string _arg = arg.Replace("/", "\\");

                    try
                    {

                        mainWindow.New_Tab(new ValueObject<IBrowsableObjectInfo>(new ShellObjectInfo(ShellObject.FromParsingName(_arg), _arg)));

                    }
                    catch (ShellException)
                    {

                        MessageBox.Show("This file does not exists.");

                    }

                }

                if (mainWindow.Items.Count == 0)

                    mainWindow.New_Tab();

            }

        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            throw new NotImplementedException();
        }

        public virtual void OnPropertyChanged(string propertyName, string fieldName, object previousValue, object newValue)
        {
           WinCopies.GUI. CommonHelper.OnPropertyChangedHelper(this, propertyName, fieldName, previousValue, newValue);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, previousValue, newValue));
        }

        public virtual void OnPropertyChangedReadOnly(string propertyName, object previousValue, object newValue)
        {
            throw new NotImplementedException();
        }

        public void OnPropertyChanged(string propertyName, object previousValue, object newValue)
        {
            throw new NotImplementedException();
        }

        private void SetProperty(string propertyName, string fieldName, object newValue)

        {

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                         BindingFlags.Static | BindingFlags.Instance |
                         BindingFlags.DeclaredOnly;

            var field = this.GetType().GetField(fieldName, flags);

            MessageBox.Show((this == application).ToString());

            MessageBox.Show((this == Application.Current).ToString());

            object previousValue = field.GetValue(this);

            if (!newValue.Equals(previousValue))

            {

                field.SetValue(this, newValue);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            }

        }

        private static void Application_Startup(object sender, StartupEventArgs e)
        {

            // SettingsManagement.SettingsManagement.LoadSettings(sender);    

            ((App)sender).FileSystemWatcher.Changed += ((App)sender).FileSystemWatcher_Changed;

            if (e.Args.Length > 0)

                application.AddNewArgs(e.Args);

            // else if (!((App)sender).IsFirstInstance)

            else

                ((MainWindow)((App)sender).MainWindow).New_Tab();

            // else

            // ((MainWindow)application.MainWindow).Show();

        }

        DateTime OptionFileLastWriteTime { get; set; } = File.GetLastWriteTime(SavePath);

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)

        {

            DateTime d = File.GetLastWriteTime(SavePath);

            if (d == OptionFileLastWriteTime)

                return;

            OptionFileLastWriteTime = d;

            // if (MessageBox.Show((string)Current.Resources["SettingsFileChanged"], "WinCopies", MessageBoxButton.YesNo) == MessageBoxResult.Yes)

            // {

            void reloadSettings()
            {

                LoadSettings(CommonProperties);

                LoadSettings(ErgonomicsProperties);

                LoadSettings(IOOperationsProperties);

            }

            Reload();

            if (!Current.Dispatcher.CheckAccess())

                Current.Dispatcher.InvokeAsync(() => reloadSettings());

            else

                reloadSettings();

            // }

        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {

            if (args.Count > 1)

            {

                args.RemoveAt(0);

                AddNewArgs(args);

            }

            return true;

        }*/
    }
}
