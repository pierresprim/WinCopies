using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinCopies.GUI.Explorer;
using WinCopies.GUI.Windows.Dialogs;
using WinCopies.Util;

namespace WinCopies.GUI
{

    // todo : setting icon sizes from the bigger (size one) to the lower (size four) for better "compatibility" for the user with the Windows explorer.

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string WinCopiesProcessesManager = "WinCopiesProcessesManager.exe";

        public readonly string CloseTabButtonUserToolTip = string.Format("{0} ({1})", (string)Application.Current.Resources["CloseTab"], "Ctrl + W");

        // todo: should be public also for the set accessor:

        private static readonly DependencyPropertyKey ItemsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Items), typeof(ObservableCollection<ValueObject<IBrowsableObjectInfo>>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        public ObservableCollection<ValueObject<IBrowsableObjectInfo>> Items => (ObservableCollection<ValueObject<IBrowsableObjectInfo>>)GetValue(ItemsProperty);

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(ValueObject<IBrowsableObjectInfo>), typeof(MainWindow));

        public ValueObject<IBrowsableObjectInfo> SelectedItem => (ValueObject<IBrowsableObjectInfo>)GetValue(SelectedItemProperty);

        public static readonly DependencyProperty HistoryProperty = DependencyProperty.Register(nameof(History), typeof(System.Collections.ObjectModel.ObservableCollection<IHistoryItemData>), typeof(MainWindow), new PropertyMetadata(new System.Collections.ObjectModel.ObservableCollection<IHistoryItemData>()));

        public System.Collections.ObjectModel.ObservableCollection<IHistoryItemData> History { get => (System.Collections.ObjectModel.ObservableCollection<IHistoryItemData>)GetValue(HistoryProperty); set => SetValue(HistoryProperty, value); }

        private static readonly DependencyPropertyKey SelectedItemVisibleItemsCountPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedItemVisibleItemsCount), typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public static readonly DependencyProperty SelectedItemVisibleItemsCountProperty = SelectedItemVisibleItemsCountPropertyKey.DependencyProperty;

        public int SelectedItemVisibleItemsCount { get => (int)GetValue(SelectedItemVisibleItemsCountProperty); }

        public MainWindow()
        {
            InitializeComponent();

            SetValue(ItemsPropertyKey, new ObservableCollection<ValueObject<IBrowsableObjectInfo>>());
            // DataContext = this;
            // ShellInterop.Shell.ShellFile
        }

        public TabControl TabControl => (TabControl)Template.FindName("TabControl", this);

        private TabItem Add_New_Tab(ValueObject<IBrowsableObjectInfo> shellObject)

        {

            if (!IsLoaded)

                Show();

            Items.Add(shellObject);

            shellObject.Value.IsSelected = true;

            return (TabItem)TabControl.ItemContainerGenerator.ContainerFromItem(shellObject);

        }

        // WinCopies.GUI.Explorer.ExplorerControl.fsw.SynchronizingObject = (ISynchronizeInvoke)this;

        public void New_Tab() => New_Tab(new ValueObject<IBrowsableObjectInfo>(new ShellObjectInfo(ShellObject.FromParsingName(KnownFolders.Desktop.ParsingName), KnownFolders.Desktop.ParsingName, IO.FileType.SpecialFolder, IO.SpecialFolders.Desktop)));

        public void New_Tab(ValueObject<IBrowsableObjectInfo> shellObject) => Add_New_Tab(shellObject).PART_ExplorerControl.Navigate(shellObject.Value, true);

        public void Restore_Tab(ValueObject<IBrowsableObjectInfo> shellObject) => Add_New_Tab(shellObject).PART_ExplorerControl.Navigate(shellObject.Value, true);

        //private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{

        //    if (e.OriginalSource.GetType() == typeof(TabControl))

        //        if (e.AddedItems.Count > 0)

        //            SetValue(SelectedItemPropertyKey, (IBrowsableObjectInfo)e.AddedItems[0]);

        //        else

        //            SetValue(SelectedItemPropertyKey, null);

        //    // GetVisualTabItem( (IBrowsableObjectInfo) e.AddedItems[0]).Navigate(WinCopies.IO.Util.GetNormalizedOSPath(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)), true);

        //}

        private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void NewTab_Executed(object sender, ExecutedRoutedEventArgs e) => New_Tab();

        private void NewWindow_Executed(object sender, ExecutedRoutedEventArgs e) => new MainWindow().New_Tab();

        private void NewWindowInNewInstance_Executed(object sender, ExecutedRoutedEventArgs e)
        {
#if DEBUG 
            Debug.WriteLine("Assembly path: " + System.Reflection.Assembly.GetExecutingAssembly().Location);
#endif 
            Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        //private bool GetFileSystemOperation_CanExecute(string parameter)
        //{

        //    bool openDirectoryIsShellFolder = SelectedItem.Value is ShellObjectInfo shellObjectInfo && shellObjectInfo.ShellObject is ShellFolder;

        //    return true; //  parameter == openDirectoryIsShellFolder && : parameter == "FileSystemOpenDirectoryCommand" ? openDirectoryIsShellFolder : false;

        //}

        private void FileSystemOperation_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // var explorerControl = SelectedItem.PART_ExplorerControl;

            //if (GetFileSystemOperation_CanExecute((string)e.Parameter)) e.CanExecute = true;

            // e.CanExecute = e.Command.CanExecute(e.Parameter, GetVisualTabItem(SelectedItem).PART_ExplorerControl);

            //e.Handled = true;

            //e.ContinueRouting = false;
        }

        private void InputBox_Command_CanExecute(object sender, CanExecuteRoutedEventArgs e)

        {

            if (string.IsNullOrEmpty(((InputBox)sender).Text) || string.IsNullOrWhiteSpace(((InputBox)sender).Text))

            {

                ((InputBox)sender).Text = null;

                e.CanExecute = false;

            }

            // todo: to add other characters ...

            else if (((InputBox)sender).Text.Contains('\\')
                     || ((InputBox)sender).Text.Contains('/')
                     || ((InputBox)sender).Text.Contains(':')
                     || ((InputBox)sender).Text.Contains('*')
                     || ((InputBox)sender).Text.Contains('?')
                     || ((InputBox)sender).Text.Contains('"')
                     || ((InputBox)sender).Text.Contains('<')
                     || ((InputBox)sender).Text.Contains('>')
                     || ((InputBox)sender).Text.Contains('|'))

            {

                ((InputBox)sender).ErrorText = (string)Application.Current.Resources["PathContainsUnauthorizedCharacters"];

                e.CanExecute = false;

            }

            else

            {

                ((InputBox)sender).ErrorText = null;

                e.CanExecute = true;

            }

        }

        private void NewFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            InputBox GetInputBox(string prefix, out InputBox inputBox)

            {

                /*InputBox */

                inputBox = new InputBox
                {

                    Command = Util.Commands.FileSystemCommands.NewFolder,

                    // inputBox.CommandBindings.Add(new CommandBinding(WinCopies.Util.Commands.FileSystemCommands.NewFolder, (object _sender, ExecutedRoutedEventArgs _e) => { }, InputBox_Command_CanExecute));

                    Label = (string)Application.Current.Resources[$"{prefix}WindowLabel"],

                    // inputBox.Text = "azerty";

                    Placeholder = new Controls.PlaceholderProperties((string)Application.Current.Resources[$"{prefix}WindowPlaceholder"], false, false, new System.Windows.Media.FontFamily(), 12, FontStretches.Normal, FontStyles.Italic, FontWeights.Normal, System.Windows.Media.Brushes.DimGray, TextAlignment.Left, null, TextWrapping.NoWrap),

                    // inputBox.Orientation = Orientation.Vertical;

                    ButtonAlignment = Windows.Dialogs.HorizontalAlignment.Right,

                    DialogButton = DialogButton.OKCancel

                };

                inputBox.

                    CommandBindings.Add(new CommandBinding(Util.Commands.FileSystemCommands.NewFolder, null, InputBox_Command_CanExecute));

                return inputBox;

            }

            InputBox _inputBox = GetInputBox("NewFolder", out _inputBox);

#if DEBUG

            bool? result = _inputBox.ShowDialog();

            Debug.WriteLine(result == null ? "null" : result.ToString());

            Debug.WriteLine(_inputBox.MessageBoxResult.ToString());

            Debug.WriteLine(_inputBox.Text);

            if (result == true)

#else    

                if (inputBox.ShowDialog() == true)

#endif

                try
                {

                    Directory.CreateDirectory(SelectedItem.Value.Path + "\\\\" + _inputBox.Text);

                }

                catch (IOException)
                {

                    MessageBox.Show((string)Application.Current.Resources["FileAlreadyExistsExceptionMessage"]);

                }

                catch (UnauthorizedAccessException)

                {

                    MessageBox.Show((string)Application.Current.Resources["AccessDeniedExceptionMessage"]);

                }

        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e) => GetVisualTabItem(SelectedItem).PART_ExplorerControl.Copy(ActionsFromObjects.ListView);

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e) => GetVisualTabItem(SelectedItem).PART_ExplorerControl.Cut((ActionsFromObjects)e.Parameter);

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e) => GetVisualTabItem(SelectedItem).PART_ExplorerControl.Paste(ActionsFromObjects.ListView);

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            // todo

        }

        // todo: better gesture by checking directly for which comment is running.

        private void ViewStyleCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

#if DEBUG 

            Debug.WriteLine((string)e.Parameter);

#endif 

            App app = (App)Application.Current;

            if (e.Command == Commands.SizeOne)

                app.CommonProperties. ViewStyle = ViewStyles.SizeOne;

            else if (e.Command == Commands.SizeTwo)

                app.CommonProperties.ViewStyle = ViewStyles.SizeTwo;

            else if (e.Command == Commands.SizeThree)

                app.CommonProperties.ViewStyle = ViewStyles.SizeThree;

            else if (e.Command == Commands.SizeFour)

                app.CommonProperties.ViewStyle = ViewStyles.SizeFour;

            else if (e.Command == Commands.ListViewStyle)

                app.CommonProperties.ViewStyle = ViewStyles.List;

            else if (e.Command == Commands.DetailsViewStyle)

                app.CommonProperties.ViewStyle = ViewStyles.Details;

            else if (e.Command == Commands.TileViewStyle)

                app.CommonProperties.ViewStyle = ViewStyles.Tiles;

            else if (e.Command == Commands.ContentViewStyle)

                app.CommonProperties.ViewStyle = ViewStyles.Content;

        }

        private bool OnTabsClosing() => ((App)Application.Current)._IsClosing == false && Items.Count > 1 && MessageBox.Show((string)Application.Current.Resources["WindowClosingMessage"], System.Reflection.Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes;

        private void Window_Closing(object sender, CancelEventArgs e)
        {

            if (OnTabsClosing())

                e.Cancel = true;

        }

        private void QuitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            bool areMultipleMainWindowsOpen()

            {

                int openMainWindowsCount = 0;

                WindowCollection openWindows = Application.Current.Windows;

                foreach (Window window in openWindows)

                    if (window.GetType() == typeof(MainWindow))

                        if (openMainWindowsCount == 1) return true;

                        else openMainWindowsCount += 1;

                return false;

            }

            ((App)Application.Current)._IsClosing = true;

            bool multipleMainWindowsOpen = areMultipleMainWindowsOpen();

            if (multipleMainWindowsOpen)

                if (MessageBox.Show((string)Application.Current.Resources["ApplicationClosingMessage"], System.Reflection.Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)

                    Application.Current.Shutdown();

                else

                    ((App)Application.Current)._IsClosing = false;

            else

                Application.Current.Shutdown();
        }

        private void NewArchive_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ArchiveCompression archiveCompression = new ArchiveCompression();

            bool? showDialogResult = archiveCompression.ShowDialog();

            if (showDialogResult == true)

            {

                string args = $"\"Compression\" \"{archiveCompression.ArchiveFormat}\" \"{archiveCompression.CompressionLevel}\" \"{archiveCompression.CompressionMethod}\" \"{archiveCompression.CompressionMode}\" \"{archiveCompression.DirectoryStructure}\" \"{archiveCompression.IncludeEmptyDirectories}\" \"{archiveCompression.SourcePath}\" \"{archiveCompression.DestPath}\"";

#if DEBUG 

                Debug.WriteLine(args);

#endif 

                ProcessStartInfo processStartInfo = new ProcessStartInfo(WinCopiesProcessesManager, args);

                Process.Start(processStartInfo);

            }
        }

        private void ExtractArchive_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = (SelectedItem.Value.SelectedItem?.FileType == IO.FileType.Archive && SelectedItem.Value.SelectedItems.Count == 1) || (SelectedItem.Value.FileType == IO.FileType.Archive && SelectedItem.Value.SelectedItems.Count == 0);

        private void ExtractArchive_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            folderBrowserDialog.Mode = FolderBrowserDialogMode.OpenFolder;

            if (folderBrowserDialog.ShowDialog() == true)

            {

            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            //ShellObject so;so.Properties.System.ItemNameDisplay.Value;
            openFileDialog.AddExtension = true;

            openFileDialog.CheckFileExists = true;

            openFileDialog.CheckPathExists = true;

            openFileDialog.Filter = "*.zip|*.zip";

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            openFileDialog.Multiselect = false;

            openFileDialog.Title = "Open an archive...";

            // if (openFileDialog.ShowDialog() == true)

            //#if DEBUG

            //            {

            //#endif 

            // SevenZip.SevenZipExtractor sevenZipExtractor = new SevenZip.SevenZipExtractor(openFileDialog.FileName);

            //#if DEBUG 

            //                foreach (SevenZip.ArchiveProperty archiveProperty in sevenZipExtractor.ArchiveProperties)

            //                    Debug.WriteLine("Name: " + archiveProperty.Name + ", Value: " + archiveProperty.Value);

            //                foreach (SevenZip.ArchiveFileInfo archiveFileInfo in sevenZipExtractor.ArchiveFileData)

            //                    foreach (SevenZip.ArchiveProperty archiveFileInfoProperty in archiveFileInfo..ArchiveFileInfoProperties)

            //                        Debug.WriteLine("Name: " + archiveFileInfoProperty.Name + ", Value: " + archiveFileInfoProperty.Value);

            //            }

            //#endif 

        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //#if DEBUG
        //            Console.WriteLine(WinCopies.IO.Util.GetNormalizedOSPath(SelectedItem.WinCopies.GUI.Explorer.ExplorerControl.CurrentPath).Path);
        //            var machin = WinCopies.IO.Util.GetNormalizedOSPath(SelectedItem.WinCopies.GUI.Explorer.ExplorerControl.CurrentPath);
        //            if (machin.ShellObject != null)

        //            {

        //                if (machin.ShellObject.Name != null)

        //                    Console.WriteLine("Path: " + machin.ShellObject.Name);

        //                Console.WriteLine("Path: " + machin.ShellObject.ParsingName);
        //                Console.WriteLine("Path: " + machin.ShellObject.Properties.System.ItemFolderPathDisplay.Value);
        //                Console.WriteLine("Path: " + machin.ShellObject.Properties.System.ItemFolderPathDisplayNarrow.Value);
        //                Console.WriteLine("Path: " + machin.ShellObject.Properties.System.ItemPathDisplay.Value);
        //                Console.WriteLine("Path: " + machin.ShellObject.Properties.System.ItemPathDisplayNarrow.Value);
        //                Console.WriteLine("Path: " + machin.ShellObject.Properties.System.ParsingPath.Value);
        //                Console.WriteLine("Path: " + machin.ShellObject.Properties.System.FileName.Value);
        //                Console.WriteLine("Path: " + machin.ShellObject.Properties.System.ItemFolderNameDisplay.Value);
        //                Console.WriteLine("Path: " + machin.ShellObject.Properties.System.ItemName.Value);
        //                Console.WriteLine("Path: " + machin.ShellObject.Properties.System.ItemNameDisplay.Value);
        //                Console.WriteLine("Path: " + machin.ShellObject.Properties.System.OriginalFileName.Value);
        //                Console.WriteLine("Path: " + machin.ShellObject.Properties.System.ParsingName.Value);
        //                Console.WriteLine(((ShellFolder)machin.ShellObject).Name);
        //                Console.WriteLine(((ShellFolder)machin.ShellObject).ParsingName);

        //            }
        //#endif
        //SelectedItem.ExplorerControl.Open(WinCopies.IO.Util.GetNormalizedOSPath(SelectedItem.ExplorerControl.CurrentPath));
        //}

        //private void TextBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //if (e.Key == Key.Enter)

        //SelectedItem.ExplorerControl.Open(WinCopies.IO.Util.GetNormalizedOSPath(SelectedItem.ExplorerControl.CurrentPath));
        // new WinCopies.IO.ShellObjectInfo(Microsoft.WindowsAPICodePack.Shell.ShellObject.FromParsingName(WinCopies.IO.Util.GetNormalizedOSPath(WinCopies.GUI.Explorer.ExplorerControl.CurrentPath)), WinCopies.IO.Util.GetNormalizedOSPath(WinCopies.GUI.Explorer.ExplorerControl.CurrentPath), WinCopies.IO.SpecialFolders)
        //}

        //private void PreviousButton_Click(object sender, RoutedEventArgs e) => SelectedItem.ExplorerControl.Navigate(SelectedItem.ExplorerControl.HistorySelectedIndex - 1);

        //private void ForwardButton_Click(object sender, RoutedEventArgs e) => SelectedItem.ExplorerControl.Navigate(SelectedItem.ExplorerControl.HistorySelectedIndex + 1);

        private void RestoreTab_Executed(object sender, ExecutedRoutedEventArgs e) => Restore_Tab((ValueObject<IBrowsableObjectInfo>)e.Parameter);

        private void FileProperties_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = (SelectedItem.Value.SelectedItem is IO.ShellObjectInfo && SelectedItem.Value.SelectedItems.Count == 1) || (SelectedItem.Value is IO.ShellObjectInfo && SelectedItem.Value.SelectedItems.Count == 0);

        private void FileProperties_Executed(object sender, ExecutedRoutedEventArgs e) => new FilePropertiesDialog((IO.ShellObjectInfo)SelectedItem.Value.SelectedItem ?? (IO.ShellObjectInfo)SelectedItem.Value).ShowDialog();

        private void CloseTab_CanExecute(object sender, CanExecuteRoutedEventArgs e) => OnCloseTab_CanExecute(this, e);

        internal static void OnCloseTab_CanExecute(MainWindow mainWindow, CanExecuteRoutedEventArgs e) => e.CanExecute = mainWindow.Items.Count > 1;

        private void CloseTab_Executed(object sender, ExecutedRoutedEventArgs e) => OnCloseTab_Executed(this, e);

        internal static void OnCloseTab_Executed(MainWindow mainWindow, ExecutedRoutedEventArgs e)

        {

            ValueObject<IBrowsableObjectInfo> item = (ValueObject<IBrowsableObjectInfo>)e.Parameter;

            if (item == null)

                item = mainWindow.SelectedItem;

            if (item.Value.ItemsLoader != null && item.Value.ItemsLoader.IsBusy)

                item.Value.ItemsLoader.Cancel();

            item.Value.Dispose();

            var tabItem = mainWindow.GetVisualTabItem(item);

            mainWindow.History.Add(new HistoryItemData(tabItem.PART_ExplorerControl.Header, tabItem.PART_ExplorerControl.Path, new ScrollViewerOffset(tabItem.PART_ExplorerControl.ListView.ScrollHost.HorizontalOffset, tabItem.PART_ExplorerControl.ListView.ScrollHost.VerticalOffset), null));

            mainWindow.Items.Remove(item);

        }

        private TabItem GetVisualTabItem(ValueObject<IBrowsableObjectInfo> item) => (TabItem)TabControl.ItemContainerGenerator.ContainerFromItem(item);

        private void CloseAllTabs_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Items.Count > 1;

        private void CloseAllTabs_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            for (int i = 0; i <= Items.Count - 1; i++)

            {

                Items[i].Value.Dispose();

                Items.RemoveAt(i);

            }

        }

        // private void Window_Loaded(object sender, RoutedEventArgs e) => New_Tab();

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) => new About().ShowDialog();

        protected override void OnClosing(CancelEventArgs e)
        {

            base.OnClosing(e);

            foreach (ValueObject<IBrowsableObjectInfo> item in Items)

                item.Value.ItemsLoader.Cancel();

            if (((System.Collections.IEnumerable)((App)Application.Current).Windows).OfType<MainWindow>().Count() > 1)

                foreach (ValueObject<IBrowsableObjectInfo> item in Items)

                    item.Value.Dispose();

        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            TabItem item = null;

            if (e.AddedItems.Count > 0)

            {

                item = (TabItem)TabControl.ItemContainerGenerator.ContainerFromItem(e.AddedItems[0]);

                if (item != null)

                {

                    item.PART_ExplorerControl.VisibleItemsCountChanged += PART_ExplorerControl_VisibleItemsCountChanged;

                    SetValue(SelectedItemVisibleItemsCountPropertyKey, item.PART_ExplorerControl.VisibleItemsCount);

                }

            }

            if (e.RemovedItems.Count > 0)

            {

                item = (TabItem)TabControl.ItemContainerGenerator.ContainerFromItem(e.RemovedItems[0]);

                if (item != null)

                    item.PART_ExplorerControl.VisibleItemsCountChanged -= PART_ExplorerControl_VisibleItemsCountChanged;

            }

        }

        private void PART_ExplorerControl_VisibleItemsCountChanged(object sender, ValueChangedEventArgs e) => SetValue(SelectedItemVisibleItemsCountPropertyKey, e.NewValue);

        private void Rename_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            InputBox inputBox = new InputBox();

            IO.IBrowsableObjectInfo browsableObject = (ActionsFromObjects)e.Parameter == ActionsFromObjects.ListView ? SelectedItem.Value.SelectedItem ?? SelectedItem.Value : (IBrowsableObjectInfo)((TabItem)TabControl.ItemContainerGenerator.ContainerFromIndex(TabControl.SelectedIndex)).PART_ExplorerControl.TreeView.SelectedItem;

            inputBox.Text = browsableObject.Name;

            // inputBox.Command = WinCopies.Util.Commands.FileSystemCommands.Rename;

            // inputBox.CommandBindings.Add(new CommandBinding(WinCopies.Util.Commands.FileSystemCommands.Rename, (object _sender, ExecutedRoutedEventArgs _e) => { }, InputBox_Command_CanExecute));

            if (inputBox.ShowDialog() == true)

                browsableObject.Rename(inputBox.Text);

        }

        private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

        //private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{

        //    if (!(SelectedItem.Value.SelectedItems.Count > 1 && FileOperation.QueryRecycleBinInfo(System.IO.Path.GetPathRoot(SelectedItem.Value.Path), out RecycleBinInfo recycleBinInfo)) && GetFileSystemOperation_CanExecute("FileSystemItemCommand") && SelectedItem.Value.SelectedItem.FileType != IO.FileType.Drive) e.CanExecute = true;

        //}

        //private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        //{

        //}

        private void FileSystemOperationMenuItem_Loaded(object sender, RoutedEventArgs e) => ((System.Windows.Controls.MenuItem)sender).CommandTarget = GetVisualTabItem(SelectedItem)?.PART_ExplorerControl;



        //private void TabItem_Loaded(object sender, RoutedEventArgs e)
        //{

        //    GetExplorerControl((TabItem) sender).Navigate(WinCopies.IO.Util.GetNormalizedOSPath(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)), true);

        //}

        //private void GetExplorerControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    GetExplorerControl((TabItem) sender).
        //}

        //private void Open_Executed(object sender, ExecutedRoutedEventArgs e)

        //{

        //    var selectedFolders = SelectedItems.Count((IBrowsableObjectInfo _path) => _path.FileType == FileTypes.Folder || _path.FileType == FileTypes.Drive || _path.FileType == FileTypes.Archive);

        //    foreach (IBrowsableObjectInfo path in SelectedItems)

        //    {

        //        if (path.FileType == FileTypes.Folder || path.FileType == FileTypes.Drive || path.FileType == FileTypes.Archive)

        //            if (selectedFolders > 1)

        //            {



        //            }

        //    }

        //}
    }

    public static class Commands
    {

        #region File menu commands

        public static RoutedUICommand Quit { get; } = new RoutedUICommand((string)Application.Current.Resources[nameof(Quit)], nameof(Quit), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.Q, ModifierKeys.Control) });

        #endregion

        public static RoutedUICommand ExtractArchive { get; } = new RoutedUICommand((string)Application.Current.Resources[nameof(ExtractArchive)], nameof(ExtractArchive), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.E, ModifierKeys.Control) });

        #region View menu commands

        public static RoutedUICommand SizeOne { get; } = new RoutedUICommand("1", nameof(SizeOne), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.D1, ModifierKeys.Control | ModifierKeys.Shift) });

        public static RoutedUICommand SizeTwo { get; } = new RoutedUICommand("2", nameof(SizeTwo), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.D2, ModifierKeys.Control | ModifierKeys.Shift) });

        public static RoutedUICommand SizeThree { get; } = new RoutedUICommand("3", nameof(SizeThree), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.D3, ModifierKeys.Control | ModifierKeys.Shift) });

        public static RoutedUICommand SizeFour { get; } = new RoutedUICommand("4", nameof(SizeFour), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.D4, ModifierKeys.Control | ModifierKeys.Shift) });

        public static RoutedUICommand ListViewStyle { get; } = new RoutedUICommand((string)Application.Current.Resources[nameof(ListViewStyle)], nameof(ListViewStyle), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.D5, ModifierKeys.Control | ModifierKeys.Shift) });

        public static RoutedUICommand DetailsViewStyle { get; } = new RoutedUICommand((string)Application.Current.Resources[nameof(DetailsViewStyle)], nameof(DetailsViewStyle), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.D6, ModifierKeys.Control | ModifierKeys.Shift) });

        public static RoutedUICommand TileViewStyle { get; } = new RoutedUICommand((string)Application.Current.Resources[nameof(TileViewStyle)], nameof(TileViewStyle), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.D7, ModifierKeys.Control | ModifierKeys.Shift) });

        public static RoutedUICommand ContentViewStyle { get; } = new RoutedUICommand((string)Application.Current.Resources[nameof(ContentViewStyle)], nameof(ContentViewStyle), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.D8, ModifierKeys.Control | ModifierKeys.Shift) });

        #endregion

        public static RoutedUICommand RestoreTab { get; } = new RoutedUICommand((string)Application.Current.Resources[nameof(RestoreTab)], nameof(RestoreTab), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.T, ModifierKeys.Control | ModifierKeys.Shift) });

    }
}
