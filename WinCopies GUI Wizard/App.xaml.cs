using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace WinCopiesGUIWizard
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged, ISingleInstanceApp
    {

        private const string Unique = "f7917f28-5b7f-43d3-b673-dcef9c8f4839";

        private FileSystemWatcher FileSystemWatcher { get; set; } = new FileSystemWatcher(Path.GetDirectoryName(WinCopies.SettingsManagement.SettingsManagement.SavePath)) { NotifyFilter = NotifyFilters.LastWrite, IncludeSubdirectories = false, Filter = "settings.settings", EnableRaisingEvents = true };

        // todo: to replace this property by a file stream to the options file. By using this way, the user will have to close the app to manually change the content of the options file. By the way, this pattern is not requiered for the other apps of the solution because these apps don't need to write on to the file, but only read it. So, these apps must authorize changes to the options file when they are in use and, when a change has occured, automatically reload the current apps and selected views regarding options.

        public bool IsApplyingChanges { get; set; } = false;

        private bool _isSaved = true;

        public bool IsSaved { get => _isSaved; internal set { _isSaved = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSaved))); } }

        // public XmlDocument Settings { get; private set; } = null;

        static App application = null;

        public event PropertyChangedEventHandler PropertyChanged;

        [STAThread]
        public static void Main()

        {

            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))

            {

                application = new App();

#if DEBUG 

                Debug.WriteLine("App static guid: " + Unique);

#endif    

                PropertyChangedCallback += (object sender, PropertyChangedEventArgs e) => ((App)Current).IsSaved = false;

                // application.Startup += Application_Startup;
                application.InitializeComponent();
                application.Run();

                SingleInstance<App>.Cleanup();

            }

        }

        private void Application_Startup(object sender, StartupEventArgs e) =>

            // string savingPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WinCopies\\settings.xml";

            // XmlDocument xmlDoc = new XmlDocument();

            // xmlDoc.LoadXml(WinCopies.SettingsManagement.SettingsManagement.DefaultXmlSettings);

            // if (!File.Exists(savingPath))

            // SaveSettingFile(xmlDoc);

            FileSystemWatcher.Changed += FileSystemWatcher_Changed;

        // FileSystemWatcher.EnableRaisingEvents = true;// Settings = xmlDoc;

        public static PropertyChangedEventHandler PropertyChangedCallback { get; set; } = null;

        DateTime OptionFileLastWriteTime { get; set; } = File.GetLastWriteTime(WinCopies.SettingsManagement.SettingsManagement.SavePath);

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)

        {

            DateTime d = File.GetLastWriteTime(WinCopies.SettingsManagement.SettingsManagement.SavePath);

            if (d == OptionFileLastWriteTime)

                return;

            OptionFileLastWriteTime = d;

            if (IsApplyingChanges)

                ((App)Current).IsApplyingChanges = false;

            else

                if (MessageBox.Show((string)Current.Resources["SettingsFileChanged"], "WinCopies", MessageBoxButton.YesNo) == MessageBoxResult.Yes)

            {

                void reloadSettings()

                {

                    WinCopies.SettingsManagement.SettingsManagement.LoadSettings(((MainWindow)MainWindow).Frame.Content);

                    IsSaved = true;

                }

                WinCopies.SettingsManagement.SettingsManagement.Reload();

                if (!Current.Dispatcher.CheckAccess())

                    Current.Dispatcher.InvokeAsync(() => reloadSettings());

                else

                    // ... your code goes here without need to use invoke

                    Current.Dispatcher.InvokeAsync(() => reloadSettings());

            }

            else

                IsSaved = false;

        }

        public static void OnUnableToSaveSettingFile()

        {

            MessageBox.Show((string)Current.Resources["UnableToCreateASettingFile"], Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Information);

            Current.Shutdown();

        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {

            application.MainWindow.Activate();

            return true;

        }
    }
}
