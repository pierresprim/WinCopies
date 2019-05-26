using Microsoft.Shell;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

using WinCopies.GUI.Explorer;
using WinCopies.SettingsManagement;
using WinCopies.Util;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;
using static WinCopies.SettingsManagement.SettingsManagement;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace WinCopies.GUI
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged, ISingleInstanceApp
    {

        protected virtual void OnPropertyChanged(string propertyName, string fieldName, object newValue, Type declaringType)

        {

            (bool propertyChanged, object oldValue) = ((INotifyPropertyChanged)this).SetProperty(propertyName, fieldName, newValue, declaringType);

            if (propertyChanged) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // OnPropertyChanged(propertyName, result.oldValue, newValue);

        }

        // protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // todo:

        private const string Unique = "08248566-c8c4-4b19-96b7-489fe3b1049b";

        private FileSystemWatcher FileSystemWatcher { get; set; } = new FileSystemWatcher(Path.GetDirectoryName(SavePath)) { NotifyFilter = NotifyFilters.LastWrite, IncludeSubdirectories = false, Filter = "settings.settings", EnableRaisingEvents = true };

        public bool IsFirstInstance { get; private set; } = false;

#if DEBUG
        private System.Collections.ObjectModel.ObservableCollection<string> _args = null;

        public System.Collections.ObjectModel.ObservableCollection<string> Args { get => _args; set { _args = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Args))); } }
#endif

        public Common CommonProperties { get; } = new Common(true);

        public Ergonomics ErgonomicsProperties { get; } = new Ergonomics(true);

        public IOOperations IOOperationsProperties { get; } = new IOOperations(true);

        public bool _IsClosing { get; set; } = false;

        static App application = null;

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

        //static async System.Threading.Tasks.Task DefaultLaunchAsync()
        //{
        //    // Path to the file in the app package to launch
        //    string imageFile = "C:\\Users\\pierr\\Desktop\\DVD2-1218-640.png";

        //    var file = StorageFile.GetFileFromPathAsync(imageFile);

        //    file.Completed = (IAsyncOperation<StorageFile> asyncInfo, AsyncStatus asyncStatus) =>
        //    {
        //        var sto = asyncInfo.GetResults();
        //        if (sto != null)
        //        {
        //            // Set the option to show the picker
        //            var options = new LauncherOptions
        //            {

        //                DisplayApplicationPicker = false
        //            };

        //            //options.PreferredApplicationPackageFamilyName = "Capture d'écran et croquis";

        //            // Launch the retrieved file
        //            var result = Launcher.LaunchFileAsync(sto, options);
        //            result.Completed = (IAsyncOperation<bool> _asyncInfo, AsyncStatus _asyncStatus) =>
        //            {

        //                bool r = _asyncInfo.GetResults();

        //            };
        //            //if (success)
        //            //{
        //            //    // File launched
        //            //}
        //            //else
        //            //{
        //            //    // File launch failed
        //            //}
        //        }
        //        else
        //        {
        //            // Could not find file
        //        }
        //    };
        //}

        //private static Dictionary<string, IReadOnlyList<AppInfo>> _appInfos = new Dictionary<string, IReadOnlyList<AppInfo>>();

        //public static ReadOnlyDictionary<string, IReadOnlyList<AppInfo>> AppInfos { get; } = new ReadOnlyDictionary<string, IReadOnlyList<AppInfo>>(_appInfos);

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
                application.Args = new System.Collections.ObjectModel.ObservableCollection<string>();
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

        //public virtual void OnPropertyChanged(string propertyName)
        //{
        //    throw new NotImplementedException();
        //}

        //public virtual void OnPropertyChanged(string propertyName, string fieldName, object previousValue, object newValue)
        //{
        //   WinCopies.GUI. CommonHelper.OnPropertyChangedHelper(this, propertyName, fieldName, previousValue, newValue);

        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, previousValue, newValue));
        //}

        //public virtual void OnPropertyChangedReadOnly(string propertyName, object previousValue, object newValue)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnPropertyChanged(string propertyName, object previousValue, object newValue)
        //{
        //    throw new NotImplementedException();
        //}

        //private void SetProperty(string propertyName, string fieldName, object newValue)

        //{

        //    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
        //                 BindingFlags.Static | BindingFlags.Instance |
        //                 BindingFlags.DeclaredOnly;

        //    var field = this.GetType().GetField(fieldName, flags);

        //    MessageBox.Show((this == application).ToString());

        //    MessageBox.Show((this == Application.Current).ToString());

        //    object previousValue = field.GetValue(this);

        //    if (!newValue.Equals(previousValue))

        //    {

        //        field.SetValue(this, newValue);

        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //    }

        //}

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

        }
    }
}
