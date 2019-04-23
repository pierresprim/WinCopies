using Microsoft.Shell;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WinCopies.IO;

namespace WinCopiesProcessesManager
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged, ISingleInstanceApp
    {

        private const string Unique = "b35e3300-b2fb-48e3-ac2e-e18719757994";

#if DEBUG 
        private ObservableCollection<string> _args = null;

        public ObservableCollection<string> Args { get => _args; set { _args = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Args")); } }
#endif 

        private ObservableCollection<object> _processes = null;

        public ObservableCollection<object> Processes { get => _processes; set { _processes = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Processes")); } }

        static App application = null;

        public event PropertyChangedEventHandler PropertyChanged;

        [STAThread]
        public static void Main()

        {

            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))

            {

                application = new App();

                application.Startup += Application_Startup;
                application.InitializeComponent();

#if DEBUG 
                application.Args = new ObservableCollection<string>();
                application.Args.CollectionChanged += Args_CollectionChanged;
#endif 
                application.

            Processes = new ObservableCollection<object>();
                application.Run();

                SingleInstance<App>.Cleanup();

            }

        }

#if DEBUG
        private static void Args_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => Debug.WriteLine(e.NewItems[0].ToString());
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

            if (new_args.Count < 3) return;

            var pathsInfo = new List<WinCopies.IO.FileProcesses.FileSystemInfo>();

            var new_arg = "";

            void AddNewPath()

            {

                if (Directory.Exists(new_arg))

                    pathsInfo.Add(new WinCopies.IO.FileProcesses.FileSystemInfo(new_arg, WinCopies.IO.FileTypes.Folder));

                else if (File.Exists(new_arg))

                    pathsInfo.Add(new WinCopies.IO.FileProcesses.FileSystemInfo(new_arg, WinCopies.IO.FileTypes.File));

            }

            switch (new_args[0])

            {

                case "Copy":
                case "FileMoving":

                    var destPath = "";

                    var isAFileMoving = false;

                    for (int i = 1; i < new_args.Count - 1; i++)

                    {

                        new_arg = new_args[i];

                        AddNewPath();

#if DEBUG 
                        Debug.WriteLine(new_arg);
#endif 

                    }

                    destPath = new_args[new_args.Count - 1];

#if DEBUG
                    Debug.WriteLine(destPath);
#endif

                    isAFileMoving = new_args[0] == "FileMoving" ? true : false;

                    var cpi = new WinCopies.IO.FileProcesses.CopyProcessInfo(destPath, isAFileMoving)

                    {

                        FilesInfoLoader = new WinCopies.IO.FileProcesses.LoadFilesInfo(pathsInfo, WinCopies.IO.FileProcesses.ActionType.Copy)

                    };

                    cpi.FilesInfoLoader.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => Process_FilesInfoLoaded(cpi);

                    Processes.Add(cpi);

                    cpi.FilesInfoLoader.LoadAsync();

#if DEBUG 
                    Debug.WriteLine("Process started.");
#endif 

                    break;

                case "Compression":

                    if (new_args.Count < 9)

                    { MessageBox.Show("Error in parsing arguments."); return; }

                    SevenZip.OutArchiveFormat archiveFormat;

                    if (!Enum.TryParse(new_args[1], out archiveFormat)) { MessageBox.Show("Error in parsing arguments."); return; }

                    var compressionLevel = SevenZip.CompressionLevel.None;

                    if (!Enum.TryParse(new_args[2], out compressionLevel)) { MessageBox.Show("Error in parsing arguments."); return; }

                    var compressionMethod = SevenZip.CompressionMethod.Copy;

                    if (!Enum.TryParse(new_args[3], out compressionMethod)) { MessageBox.Show("Error in parsing arguments."); return; }

                    var compressionMode = SevenZip.CompressionMode.Create;

                    if (!Enum.TryParse(new_args[4], out compressionMode)) { MessageBox.Show("Error in parsing arguments."); return; }

                    var directoryStructure = true;

                    if (!bool.TryParse(new_args[5], out directoryStructure)) { MessageBox.Show("Error in parsing arguments."); return; }

                    var includeEmptyDirectories = true;

                    if (!bool.TryParse(new_args[6], out includeEmptyDirectories)) { MessageBox.Show("Error in parsing arguments."); return; }

                    var archiveName = new_args[7];

                    SevenZip.SevenZipCompressor sevenZipCompressor = new SevenZip.SevenZipCompressor
                    {
                        ArchiveFormat = archiveFormat,

                        CompressionLevel = compressionLevel,

                        CompressionMethod = compressionMethod,

                        CompressionMode = compressionMode,

                        DirectoryStructure = directoryStructure,

                        IncludeEmptyDirectories = includeEmptyDirectories
                    };

                    SevenZipCompressor sevenZipCompressorWrapper = new SevenZipCompressor(sevenZipCompressor);

                    Processes.Add(sevenZipCompressorWrapper);

                    if ((Directory.Exists(archiveName) || File.Exists(archiveName)) && MessageBox.Show("File already exists. Do you want to overwrite it?", System.Reflection.Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)

                        break;

                    try
                    {

                        List<string> files = new List<string>();

                        ShellObjectInfo path = null;

                        for (int i = 8; i < new_args.Count; i++)

                            using (path = new ShellObjectInfo(ShellObject.FromParsingName(new_args[i]), new_args[i]))

                                if (WinCopies.Util.Util.If(WinCopies.Util.Util.ComparisonType.Or, WinCopies.Util.Util.Comparison.Equals, path.FileType, FileTypes.Folder, FileTypes.Drive, FileTypes.SpecialFolder))

                                    sevenZipCompressorWrapper.BeginCompressDirectory(path.Path, archiveName);

                                else

                                    files.Add(path.Path);

                        path = null;

                        sevenZipCompressorWrapper.BeginCompressFiles(files.ToArray(), archiveName);

                    }

                    catch (Exception ex) when (ex is IOException || ex is SevenZip.SevenZipException) { MessageBox.Show("Error: " + ex.Message); }

                    break;

                case "Deletion":

                    for (int i = 1; i < new_args.Count; i++)

                    {

                        new_arg = new_args[i];

                        AddNewPath();

                    }

                    // Processes.Add(new WinCopies.IO.FilesProcesses.);

                    break;

            }

        }

        private void Process_FilesInfoLoaded(WinCopies.IO.FileProcesses.Process p)
        {

#if DEBUG 
            // var p = (WinCopies.Util.BackgroundWorker)sender;    

            if (p is CopyProcessInfo)

                ((CopyProcessInfo)p).StartCopy();

            foreach (WinCopies.IO.FileProcesses.FileSystemInfo fileSystemInfo in p.FilesInfoLoader.PathsLoaded)

                if (fileSystemInfo.FileType == WinCopies.IO.FileTypes.Folder || fileSystemInfo.FileType == WinCopies.IO.FileTypes.Drive)

                    Debug.WriteLine(fileSystemInfo.FileSystemInfoProperties.FullName + " " + fileSystemInfo.FileType.ToString());
#else 
            ((WinCopies.IO.FilesProcesses.CopyProcessInfo)sender).startCopy();
#endif 



        }

        private static void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
                application.AddNewArgs(e.Args);
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (args.Count > 0)
                AddNewArgs(args);

            return true;
        }
    }
}
