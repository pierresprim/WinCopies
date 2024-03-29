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

#region Usings
#region System
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
#endregion System

#region WinCopies
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Extensions;
using WinCopies.GUI.IO;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IPCService.Extensions;
using WinCopies.Linq;
#endregion WinCopies

using static WinCopies.App;

using Action = System.Action;
using Enumerable = System.Linq.Enumerable;
#endregion Usings

namespace WinCopies
{
    public sealed class SingleInstanceApp : SingleInstanceApp2<string, System.Windows.Application>
    {
        public static new SingleInstanceApp App { get; } = new();

        protected override string FileName => App.FileName;

        public override App GetDefaultApp<TItems>(IPCService.Extensions.IQueue<TItems> queue)
        {
            var app = new App();

            ObservableLinkedCollection<Window> openWindows = GetOpenWindows(app);

            SetOpenWindows(app, new Collections.Generic.UIntCountableProvider<Window, IEnumeratorInfo2<Window>>(() => new EnumeratorInfo<Window>(openWindows), () => openWindows.Count));

            app.Resources = IPCService.Extensions.Application.GetResourceDictionary("ResourceDictionary.xaml");

            app.SetPluginParametersAction();

            if (app.PluginParameters != null)
            {
                var mainWindow = new MainWindow(queue == null || queue.Count < 1);
                app.MainWindow = mainWindow;

                if (queue != null)
                {
                    IExplorerControlViewModel? arg;
                    var paths = (System.Collections.ObjectModel.ObservableCollection<IExplorerControlViewModel>)((MainWindowViewModel)app.MainWindow.DataContext).Paths.Paths;

                    while (queue.Count != 0)
                    {
                        arg = GetExplorerControlViewModel(queue.DequeueAsString());

                        if (arg != null)

                            paths.Add(arg);
                    }

                    if (paths.Count > 0)

                        paths[0].IsSelected = true;

                    else

                        paths.Add(mainWindow.GetDefaultExplorerControlViewModel(true));
                }

                //else

                //    paths.Add(GUI.Shell.ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlBrowsableObjectInfoViewModel());

                // app.MainWindow = new MainWindow();

                // System.Windows.Application.LoadComponent(app.Resources, new Uri("/wincopies;component/ResourceDictionary.xaml", UriKind.Relative));
            }

            return app;
        }

        public override SingleInstanceAppInstance<string> GetDefaultSingleInstanceApp(IPCService.Extensions.IQueue<string> args) => new SingleInstanceApp_Path(args);

        protected override AppLoader<string, System.Windows.Application> GetLoaderOverride() => new Loader();

        internal new static unsafe System.Collections.Generic.IEnumerable<string> GetArray(ref ArrayBuilder<string> arrayBuilder, System.Collections.Generic.IEnumerable<string> keys, int* i, params string[] args) => IPCService.Extensions.SingleInstanceApp.GetArray(ref arrayBuilder, keys, i, args);
    }

    public abstract class SingleInstanceAppInstance<T> : IPCService.Extensions.Windows.SingleInstanceAppInstance<IPCService.Extensions.IQueue<T>>
    {
        public SingleInstanceAppInstance(in string pipeName, in IPCService.Extensions.IQueue<T> innerObject) : base(pipeName, innerObject) { /* Left empty. */ }

        public override string GetClientName() => IO.ObjectModel.BrowsableObjectInfo.DefaultClientVersion.ClientName;
    }

    public sealed class SingleInstanceApp_Process : SingleInstanceAppInstance<IProcessParameters>
    {
        public SingleInstanceApp_Process(in IPCService.Extensions.IQueue<IProcessParameters> processParameters) : base("65cae396-971a-4545-97e7-83d4ed042d92", processParameters)
        {
            // Left empty.
        }

        protected override App GetApp() => new()
        {
            Resources = IPCService.Extensions.Application.GetResourceDictionary("ResourceDictionary.xaml"),
            MainWindow = new ProcessWindow(),
            _processQueue = InnerObject
        };

        protected override Expression<Func<IUpdater, int>> GetExpressionOverride()
        {
            var processes = new ArrayBuilder<string>();

            while (InnerObject.Count != 0)

                foreach (string value in GetProcessParameters(InnerObject.Dequeue()))

                    _ = processes.AddLast(value);

            string[] _processes = processes.ToArray();

            return item => item.Run(_processes);
        }
    }

    public class SingleInstanceApp_Path : SingleInstanceAppInstance<string>
    {
        public SingleInstanceApp_Path(in IPCService.Extensions.IQueue<string> paths) : base("5e2e0072-edee-47d2-a0cd-a98d43b8b705", paths)
        {
            // Left empty.
        }

        protected override IPCService.Extensions.Application GetApp() => SingleInstanceApp.App.GetDefaultApp(InnerObject);

        protected override Expression<Func<IUpdater, int>> GetExpressionOverride()
        {
            if (InnerObject == null)

                return item => item.Run(null);

            string[] paths = new string[InnerObject.Count * 2];

            int i = -1;

            while (InnerObject.Count != 0)
            {
                paths[++i] = "Path";

                paths[++i] = InnerObject.Dequeue();
            }

            return item => item.Run(paths);
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class KeyAttribute : Attribute
    {

    }

    public sealed class Keys
    {
        [Key]
        public const string Path = nameof(Path);

        [Key]
        public const string Process = nameof(Process);

        public static System.Collections.Generic.IEnumerable<string> GetConsts() => typeof(Keys).GetFields(BindingFlags.Public | BindingFlags.Static).WhereSelect(f => f.GetCustomAttribute<KeyAttribute>(false) != null, f => (string)f.GetValue(null));
    }

    public class PathQueue : Queue
    {
        protected override Task Run() => SingleInstanceApp.App.MainDefault<PathCollectionUpdater>(this);
    }

    public class ProcessQueue : Collections.DotNetFix.Generic.Queue<IProcessParameters>, IPCService.Extensions.IQueue<IProcessParameters>
    {
        string IQueueBase.DequeueAsString() => throw new NotSupportedException();

        string IQueueBase.PeekAsString() => throw new NotSupportedException();

        Task IQueueBase.Run() => Main_Process(this);
    }

    public class Loader : AppLoader<string, System.Windows.Application>
    {
        private readonly IPCService.Extensions.IQueue<string> _pathQueue = new PathQueue();
        private readonly IPCService.Extensions.IQueue<IProcessParameters> _processQueue = new ProcessQueue();

        public override string DefaultKey => Keys.Path;

        public override IQueueBase DefaultQueue => _pathQueue;

        public unsafe static void LoadPathParameters(in Collections.DotNetFix.Generic.IQueue<string> queue, in int* i, params string[] args) => queue.Enqueue(args[*i]);

        public unsafe static void LoadProcessParameters(in Collections.DotNetFix.Generic.IQueue<IProcessParameters> queue, ref ArrayBuilder<string> arrayBuilder, in int* i, params string[] args) => queue.Enqueue(new ProcessParameters(args[*i], SingleInstanceApp.GetArray(ref arrayBuilder, Keys.GetConsts(), i, args)));

        public unsafe override System.Collections.Generic.IDictionary<string, IPCService.Extensions.Action> GetActions() => new Dictionary<string, IPCService.Extensions.Action>(2)
            {
                { Keys.Path, (string[] args, ref ArrayBuilder<string> arrayBuilder, in int* i) => LoadPathParameters(_pathQueue, i, args) },

                { Keys.Process, (string[] args, ref ArrayBuilder<string> arrayBuilder, in int* i) =>  LoadProcessParameters (_processQueue, ref  arrayBuilder, i, args) }
            };

        public override System.Collections.Generic.IEnumerable<IQueueBase> GetQueues() => Enumerable.Repeat(_processQueue, 1);
    }

    public class PluginInfo : GUI.IO.ObjectModel.PluginInfo
    {
        public PluginInfo(in IBrowsableObjectInfoPlugin plugin, in ClientVersion clientVersion) : base(plugin, clientVersion) { /* Left empty. */ }

        protected override GUI.IO.ObjectModel.BrowsableObjectInfoStartPage GetBrowsableObjectInfoStartPage() => new BrowsableObjectInfoStartPage(ClientVersion);

        public override IBrowsableObjectInfo Clone() => new PluginInfo(InnerObjectGeneric, ClientVersion);
    }

    public class BrowsableObjectInfoStartPage : GUI.IO.ObjectModel.BrowsableObjectInfoStartPage
    {
        public BrowsableObjectInfoStartPage(ClientVersion clientVersion) : base(Current.PluginParameters.Select(plugin => new PluginInfo(plugin, clientVersion)), clientVersion) { /* Left empty. */ }
        public BrowsableObjectInfoStartPage() : this(DefaultClientVersion) { /* Left empty. */ }

        protected override Icon GetIcon() => Properties.Resources.WinCopies;

        public override IBrowsableObjectInfo Clone() => new BrowsableObjectInfoStartPage(ClientVersion);
    }

    public partial class App : IPCService.Extensions.Application, IApplication
    {
        internal IPCService.Extensions.IQueue<IProcessParameters> _processQueue;

        public const string FileName = "WinCopies.exe";

        public static IProcessPathCollectionFactory DefaultProcessPathCollectionFactory { get; } = new ProcessPathCollectionFactory();

        public new bool IsClosing { get => base.IsClosing; internal set => base.IsClosing = value; }

        internal new ObservableLinkedCollection<Window> _OpenWindows => base._OpenWindows;

        internal System.Collections.Generic.IEnumerable<IBrowsableObjectInfoPlugin> PluginParameters { get; private set; }

        internal static void StartInstance<T>(in T processInfo, in FuncIn<T, IProcessParameters> processParametersFunc) where T : IProcessFactoryProcessInfo
        {
            if (processInfo.UserConfirmationRequired && MessageBox.Show(processInfo.GetUserConfirmationText(), "Process Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.Cancel)

                return;

            IProcessParameters processParameters = processParametersFunc(processInfo);

            if (processParameters != null)

                IPCService.Extensions.SingleInstanceApp.StartInstance(FileName, GetProcessParameters(processParameters));
        }

        internal static System.Collections.Generic.IEnumerable<string> GetProcessParameters(IProcessParameters processParameters)
        {
            yield return "Process";

            yield return processParameters.Guid.ToString();

            foreach (string parameter in processParameters.Parameters)

                yield return parameter;
        }

        public static void OnArgumentError(string? arg) => MessageBox.Show($"Module {arg ?? "<No module provided>"} unknown.", "Argument Error", MessageBoxButton.OK, MessageBoxImage.Error);

        #region Main
        public static async Task Main_Process(IPCService.Extensions.IQueue<IProcessParameters> processQueue) => await SingleInstanceApp.App.MainMutex<IProcessParameters, ProcessCollectionUpdater>(new SingleInstanceApp_Process(processQueue), false, null);

        [STAThread]
        public static async Task Main(string[] args) => await new SingleInstanceApp().Main<PathCollectionUpdater>(OnArgumentError, args).Await();
        #endregion

        private void SetPluginParameters(in Action<IBrowsableObjectInfoPlugin> action, out System.Collections.Generic.IEnumerable<IBrowsableObjectInfoPlugin> pluginParameters) => GUI.IO.Application.Initialize(this, action, Delegates.EmptyVoid, out pluginParameters);

        private void SetPluginParameters()
        {
            SetPluginParametersAction = Delegates.EmptyVoid;

            SetPluginParameters(pluginParameters =>
            {
                pluginParameters.RegisterBrowsabilityPaths();
                pluginParameters.RegisterBrowsableObjectInfoSelectors();
                pluginParameters.RegisterItemSelectors();
            },
            out System.Collections.Generic.IEnumerable<IBrowsableObjectInfoPlugin> pluginParameters);

            PluginParameters = pluginParameters;
        }

        internal Action SetPluginParametersAction { get; private set; }

        public App() => SetPluginParametersAction = SetPluginParameters;

        protected override void OnStartup2(StartupEventArgs e)
        {
            if (_processQueue == null)

                SetPluginParametersAction();

            else
            {
                SetPluginParameters(pluginParameters => pluginParameters.RegisterProcessSelectors(), out System.Collections.Generic.IEnumerable<IBrowsableObjectInfoPlugin> pluginParameters);

                if (pluginParameters == null)
                {
                    Shutdown(Environment.ExitCode);

                    Environment.Exit(Environment.ExitCode);
                }

                else

                    Run(_processQueue);
            }
        }

        public static void Run(IPCService.Extensions.IQueue<string> pathQueue)
        {
            IExplorerControlViewModel? arg;

            while (pathQueue.Count != 0)

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    arg = GetExplorerControlViewModel(pathQueue.Dequeue());

                    if (arg != null)

                        PathCollectionUpdater.Instance.Paths.Add(arg);
                });
        }

        public static void Run(IPCService.Extensions.IQueue<IProcessParameters> processQueue)
        {
            while (processQueue.Count != 0)

                System.Windows.Application.Current.Dispatcher.Invoke(() => ProcessCollectionUpdater.Instance.Processes.Add(new Process(IO.ObjectModel.BrowsableObjectInfo.DefaultProcessSelectorDictionary.Select(new ProcessFactorySelectorDictionaryParameters(processQueue.Dequeue(), DefaultProcessPathCollectionFactory)))));
        }

        public static new App Current => (App)System.Windows.Application.Current;

        public Extensions.Logger Logger { get; } = FileLogger.GetLogger();

        internal static IExplorerControlViewModel? GetExplorerControlViewModel(in string path)
        {
            IBrowsableObjectInfo? result = IO.ObjectModel.BrowsableObjectInfo.Create(path, true, false);

            return result == null ? null : new ExplorerControlViewModel(new BrowsableObjectInfoViewModel(result));
        }

        internal static new ObservableLinkedCollection<Window> GetOpenWindows(IPCService.Extensions.Application app) => IPCService.Extensions.Application.GetOpenWindows(app);

        internal static void SetOpenWindows(in IPCService.Extensions.Application app, in IUIntCountableEnumerable<Window> enumerable) => IPCService.Extensions.Application.SetOpenWindows(app, enumerable);

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
