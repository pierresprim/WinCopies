using Microsoft.Shell;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using WinCopies.IO;
using WinCopies.IO.FileProcesses;
using WinCopies.Util;
using static WinCopies.Util.Util;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;

namespace WinCopiesProcessesManager
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged, ISingleInstanceApp
    {

#if DEBUG 
        private System.Collections.ObjectModel.ObservableCollection<string> _args = null;

        public System.Collections.ObjectModel.ObservableCollection<string> Args { get => _args; set { _args = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Args))); } }
#endif 

        private System.Collections.ObjectModel.ObservableCollection<object> _processes = null;

        public System.Collections.ObjectModel.ObservableCollection<object> Processes { get => _processes; set { _processes = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Processes))); } }

        static App application = null;

        public event PropertyChangedEventHandler PropertyChanged;

        [STAThread]
        public static void Main()

        {

            if (SingleInstance<App>.InitializeAsFirstInstance(((GuidAttribute)System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value))

            {

                application = new App();

                application.Startup += Application_Startup;
                application.InitializeComponent();

#if DEBUG 
                application.Args = new System.Collections.ObjectModel.ObservableCollection<string>();
                application.Args.CollectionChanged += Args_CollectionChanged;
#endif 
                application.

            Processes = new System.Collections.ObjectModel.ObservableCollection<object>();
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

        internal static void BeginSevenZipProcess(SevenZipCompressor sevenZipCompressorWrapper)

        {

            try
            {

                if ((Directory.Exists(sevenZipCompressorWrapper.DestPath) || File.Exists(sevenZipCompressorWrapper.DestPath)) && MessageBox.Show("File already exists. Do you want to overwrite it?", Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)

                    return;

                List<string> files = new List<string>();

                ShellObjectInfo path = null;

                bool error = false;

                for (int i = 0; i < sevenZipCompressorWrapper.SourcePaths.Length; i++)

                    try

                    {

                        if (!Directory.Exists(sevenZipCompressorWrapper.SourcePaths[i]))

                            using (path = new ShellObjectInfo(ShellObject.FromParsingName(sevenZipCompressorWrapper.SourcePaths[i]), sevenZipCompressorWrapper.SourcePaths[i]))

                                if (If(ComparisonType.Or, ComparisonMode.Logical, Comparison.Equals, path.FileType, FileType.Folder, FileType.Drive, FileType.SpecialFolder))

                                    sevenZipCompressorWrapper.BeginCompressDirectory(sevenZipCompressorWrapper.SourcePaths[i]);

                                else

                                    files.Add(path.Path);

                    }

                    catch (ShellException ex)

                    {

                        error = true;

                    }

                path = null;

                if (files.Count > 0)

                    sevenZipCompressorWrapper.BeginCompressFiles(files.ToArray());

                if (error)

                {

                    Thread t = new Thread(() =>

                    MessageBox.Show("One or more paths couldn't be added to the paths to compress."));

                    t.Start();

                }

            }

            catch (Exception ex) when (ex is IOException || ex is SevenZip.SevenZipException)

            {

                if (ex is SevenZip.SevenZipException)

                    sevenZipCompressorWrapper.ExceptionOccurred = true;

                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void OnNewArgsAdded(IList<string> new_args)

        {

            if (new_args.Count < 3) return;

            List<WinCopies.IO.FileProcesses.FileSystemInfo> pathsInfo = new List<WinCopies.IO.FileProcesses.FileSystemInfo>();

            string new_arg = "";

            void AddNewPath()

            {

                if (Directory.Exists(new_arg))

                    pathsInfo.Add(new WinCopies.IO.FileProcesses.FileSystemInfo(new_arg, FileType.Folder));

                else if (File.Exists(new_arg))

                    pathsInfo.Add(new WinCopies.IO.FileProcesses.FileSystemInfo(new_arg, FileType.File));

            }

            switch (new_args[0])

            {

                case "Copy":
                case "FileMoving":

                    string destPath = "";

                    bool isAFileMoving = false;

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

                    CopyProcessInfo cpi = new CopyProcessInfo(destPath, isAFileMoving)

                    {

                        FilesInfoLoader = new FilesInfoLoader(pathsInfo, ActionType.Copy)

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

                    SevenZip.CompressionLevel compressionLevel = SevenZip.CompressionLevel.None;

                    if (!Enum.TryParse(new_args[2], out compressionLevel)) { MessageBox.Show("Error in parsing arguments."); return; }

                    SevenZip.CompressionMethod compressionMethod = SevenZip.CompressionMethod.Copy;

                    if (!Enum.TryParse(new_args[3], out compressionMethod)) { MessageBox.Show("Error in parsing arguments."); return; }

                    SevenZip.CompressionMode compressionMode = SevenZip.CompressionMode.Create;

                    if (!Enum.TryParse(new_args[4], out compressionMode)) { MessageBox.Show("Error in parsing arguments."); return; }

                    bool directoryStructure = true;

                    if (!bool.TryParse(new_args[5], out directoryStructure)) { MessageBox.Show("Error in parsing arguments."); return; }

                    bool includeEmptyDirectories = true;

                    if (!bool.TryParse(new_args[6], out includeEmptyDirectories)) { MessageBox.Show("Error in parsing arguments."); return; }

                    string archiveName = new_args[7];

                    SevenZipCompressor sevenZipCompressorWrapper = null;

                    SevenZip.SevenZipCompressor sevenZipCompressor = new SevenZip.SevenZipCompressor
                    {
                        ArchiveFormat = archiveFormat,

                        CompressionLevel = compressionLevel,

                        CompressionMethod = compressionMethod,

                        CompressionMode = compressionMode,

                        DirectoryStructure = directoryStructure,

                        IncludeEmptyDirectories = includeEmptyDirectories,

                        // VolumeSize = 64*1024*1024
                    };

                    sevenZipCompressorWrapper = new SevenZipCompressor(sevenZipCompressor);

                    sevenZipCompressorWrapper.SourcePaths = new_args.ToArray(8, new_args.Count - 8);

                    sevenZipCompressorWrapper.DestPath = archiveName;

                    Processes.Add(sevenZipCompressorWrapper);

                    BeginSevenZipProcess(sevenZipCompressorWrapper);

                    break;

                case "Extraction":

                    string archiveFileName = new_args[1];

                    string _destPath = new_args[2];

                    try

                    {

                        SevenZip.SevenZipExtractor sevenZipExtractor = new SevenZip.SevenZipExtractor(archiveFileName);

                        SevenZipExtractor sevenZipExtractorWrapper = new SevenZipExtractor(sevenZipExtractor);

                        Processes.Add(sevenZipExtractorWrapper);

                        sevenZipExtractorWrapper.BeginExtractArchive(_destPath);

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

                if (fileSystemInfo.FileType == FileType.Folder || fileSystemInfo.FileType == FileType.Drive)

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
