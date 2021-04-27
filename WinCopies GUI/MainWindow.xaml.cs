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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Desktop;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.Linq;

using static WinCopies.App;
using static WinCopies.UtilHelpers;

namespace WinCopies
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(nameof(Menu), typeof(MenuViewModel), typeof(MainWindow));

        //public MenuViewModel Menu { get => (MenuViewModel)GetValue(MenuProperty); set => SetValue(MenuProperty, value); }

        //public static readonly DependencyPropertyKey SelectedMenuItemPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedMenuItem), typeof(MenuItemViewModel), typeof(MainWindow), new PropertyMetadata(false));

        //public static readonly DependencyProperty SelectedMenuItemProperty = SelectedMenuItemPropertyKey.DependencyProperty;

        //public MenuItemViewModel SelectedMenuItem { get => (MenuItemViewModel)GetValue(SelectedMenuItemProperty); internal set => SetValue(SelectedMenuItemPropertyKey, value); }

        public static RoutedCommand NewFileSystemTab => Commands.ApplicationCommands.NewTab;

        public static RoutedCommand NewRegistryTab { get; } = new RoutedUICommand(Properties.Resources.NewRegistryTab, nameof(NewRegistryTab), typeof(MainWindow));

        public static RoutedCommand NewWMITab { get; } = new RoutedUICommand(Properties.Resources.NewWMITab, nameof(NewWMITab), typeof(MainWindow));

        public static RoutedCommand NewWindow => Commands.ApplicationCommands.NewWindow;

        public static RoutedCommand CloseTab => Commands.ApplicationCommands.CloseTab;

        public static RoutedCommand CloseOtherTabs { get; } = new RoutedUICommand(Properties.Resources.CloseOtherTabs, nameof(CloseOtherTabs), typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.W, ModifierKeys.Control | ModifierKeys.Alt) });

        public static RoutedCommand CloseAllTabs => Commands.ApplicationCommands.CloseAllTabs;

        public static RoutedCommand CloseWindow => ApplicationCommands.Close;

        public static RoutedCommand Quit { get; } = new RoutedUICommand(Properties.Resources.Quit, nameof(Quit), typeof(MainWindow));

        public static RoutedCommand Copy { get; } = new RoutedUICommand(Properties.Resources.CopyStatusBarLabel, nameof(Copy), typeof(MainWindow));

        public static RoutedCommand Paste { get; } = new RoutedUICommand(Properties.Resources.Paste, nameof(Paste), typeof(MainWindow));

        public static RoutedCommand About { get; } = new RoutedUICommand(Properties.Resources.About, nameof(About), typeof(MainWindow));

        public static RoutedCommand SubmitABug { get; } = new RoutedUICommand(Properties.Resources.SubmitABug, nameof(SubmitABug), typeof(MainWindow));

        public static Func<MainWindowViewModel, object> GetCloseTabParameter { get; } = mainWindow => mainWindow.SelectedItem;

        public static Func<MainWindowViewModel, object> GetCloseOtherTabsParameter { get; } = mainWindow => mainWindow.SelectedItem;

        public static ImageSource NewTabIcon { get; } = GUI.Icons.Properties.Resources.tab_add.ToImageSource();

        public static ImageSource NewWindowIcon { get; } = GUI.Icons.Properties.Resources.application_add.ToImageSource();

        public static ImageSource CloseTabIcon { get; } = GUI.Icons.Properties.Resources.tab_delete.ToImageSource();

        public static ImageSource CopyIcon { get; } = GUI.Icons.Properties.Resources.page_copy.ToImageSource();

        public static ImageSource PasteIcon { get; } = GUI.Icons.Properties.Resources.page_paste.ToImageSource();

        public static ImageSource SubmitABugIcon { get; } = GUI.Icons.Properties.Resources.bug.ToImageSource();

        public MainWindow()
        {
            const string Parameter = "Parameter";
            const string Resource = "Resource";

            _ = Current._OpenWindows.AddFirst(this);

            var mainWindow = new MainWindowViewModel();

            var xml = new XmlDocument();

            xml.LoadXml(Properties.Resources.Menus);

            XmlElement xmlMenu = xml.DocumentElement;

            var arrayBuilder = new ArrayBuilder<PropertyInfo>();

            PropertyInfo[] mainWindowProperties = typeof(MainWindow).GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (PropertyInfo p in mainWindowProperties.Where(p => p.PropertyType.IsAssignableFrom(typeof(RoutedCommand))))

                _ = arrayBuilder.AddLast(p);

            PropertyInfo[] commands = arrayBuilder.ToArray(true);

            foreach (PropertyInfo p in mainWindowProperties.Where(p => p.Name.EndsWith(Parameter)))

                _ = arrayBuilder.AddLast(p);

            PropertyInfo[] commandParameters = arrayBuilder.ToArray(true);

            foreach (PropertyInfo p in mainWindowProperties.Where(p => p.PropertyType.IsAssignableFrom(typeof(ImageSource))))

                _ = arrayBuilder.AddLast(p);

            PropertyInfo[] iconImageSources = arrayBuilder.ToArray(true);

            string getStringResource(string resourceId) => (string)ResourceProperties.First(p => p.Name == resourceId).GetValue(null);

            RoutedCommand getCommand(string resourceId) => (RoutedCommand)commands.FirstOrDefault(c => c.Name == resourceId)?.GetValue(null);

            Func getCommandParameter(string resourceId)
            {
                PropertyInfo p = commandParameters.FirstOrDefault(p => p.Name == $"{resourceId}{Parameter}");

                object value;

                if (p != null)
                {
                    value = p.GetValue(null);

                    return new Func(() => value);
                }

                p = commandParameters.FirstOrDefault(p => p.Name == $"Get{resourceId}{Parameter}");

                if (p == null) return null;

                value = p.GetValue(null);

                return new Func(() => ((Func<MainWindowViewModel, object>)value)(mainWindow));
            }

            ImageSource getIconImageSource(string resourceId) => (ImageSource)iconImageSources.FirstOrDefault(i => i.Name == $"{resourceId}Icon")?.GetValue(null);

            void addMenuItem(XmlElement xmlMenuItem, MenuItemViewModel menuItem)
            {
                string resourceId;

                foreach (XmlElement _xmlMenuItem in xmlMenuItem)
                {
                    resourceId = _xmlMenuItem.Attributes[Resource].Value;

                    addMenuItem(_xmlMenuItem, new MenuItemViewModel(menuItem, getStringResource(resourceId), resourceId, getCommand(resourceId), getCommandParameter(resourceId), getIconImageSource(resourceId)));
                }
            }

            string resourceId;

            foreach (XmlElement xmlMenuItem in xmlMenu)
            {
                resourceId = xmlMenuItem.Attributes[Resource].Value;

                addMenuItem(xmlMenuItem, new MenuItemViewModel(mainWindow.Menu, getStringResource(resourceId), resourceId, null, null, null));
            }

            DataContext = mainWindow;

            // InitializeComponent();
        }

        private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void AddNewTab(IExplorerControlBrowsableObjectInfoViewModel viewModel, ExecutedRoutedEventArgs e)
        {
            IExplorerControlBrowsableObjectInfoViewModel explorerControlBrowsableObjectInfo = viewModel;

            explorerControlBrowsableObjectInfo.IsSelected = true;

            ((MainWindowViewModel)DataContext).Paths.Add(explorerControlBrowsableObjectInfo);

            e.Handled = true;
        }

        private void NewTab_Executed(object sender, ExecutedRoutedEventArgs e) => AddNewTab(GetDefaultExplorerControlBrowsableObjectInfoViewModel(), e);

        private void NewRegistryTab_Executed(object sender, ExecutedRoutedEventArgs e) => AddNewTab(GetDefaultExplorerControlBrowsableObjectInfoViewModel(new RegistryItemInfo()), e);

        private void NewWMITab_Executed(object sender, ExecutedRoutedEventArgs e) => AddNewTab(GetDefaultExplorerControlBrowsableObjectInfoViewModel(new WMIItemInfo()), e);

        private void NewWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new MainWindow().Show();

            e.Handled = true;
        }

        private void CloseTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((MainWindowViewModel)DataContext).Paths.Count > 1;

            e.Handled = true;
        }

        private void CloseTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ = ((MainWindowViewModel)DataContext).Paths.Remove((IExplorerControlBrowsableObjectInfoViewModel)(e.Parameter is Func func ? func() : e.Parameter));

            e.Handled = true;
        }

        private void CloseOtherTabs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ = RemoveAll(((MainWindowViewModel)DataContext).Paths, (IExplorerControlBrowsableObjectInfoViewModel)(e.Parameter is Func func ? func() : e.Parameter), false, false);

            e.Handled = true;
        }

        // todo: replace by the WinCopies.Util's same method.

        public static bool RemoveAll<T>(in IList<T> collection, in T itemToKeep, in bool onlyOne, in bool throwIfMultiple) where T : class
        {
            while (collection.Count != 1)
            {
                if (collection[0] == itemToKeep)
                {
                    if (onlyOne)
                    {
                        while (collection.Count != 1)
                        {
                            if (collection[1] == itemToKeep)
                            {
                                if (throwIfMultiple) throw new InvalidOperationException("More than one occurences was found.");

                                else

                                    while (collection.Count != 1)

                                        collection.RemoveAt(1);

                                return false;
                            }

                            collection.RemoveAt(1);

                            return true;
                        }
                    }

                    while (collection.Count != 1)

                        collection.RemoveAt(1);

                    return true;
                }

                collection.RemoveAt(0);
            }

            return false;
        }

        private void CloseAllTabs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Collections.ObjectModel.ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> paths = ((MainWindowViewModel)DataContext).Paths;

            paths.Clear();

            IExplorerControlBrowsableObjectInfoViewModel explorerControlBrowsableObjectInfo = GetDefaultExplorerControlBrowsableObjectInfoViewModel();

            explorerControlBrowsableObjectInfo.IsSelected = true;

            paths.Add(explorerControlBrowsableObjectInfo);

            e.Handled = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            System.Collections.ObjectModel.ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> paths = ((MainWindowViewModel)DataContext).Paths;

            if (!Current.IsClosing && paths.Count > 1 && MessageBox.Show(this, Properties.Resources.WindowClosingMessage, "WinCopies", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)

                e.Cancel = true;

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _ = Current._OpenWindows.Remove2(this);
        }

        private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();

            e.Handled = true;
        }

        private void Quit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ObservableLinkedCollection<MainWindow> openWindows = Current._OpenWindows;

            if (openWindows.Count == 1 || MessageBox.Show(this, Properties.Resources.ApplicationClosingMessage, "WinCopies", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Current.IsClosing = true;

                while (openWindows.Count > 0)
                {
                    openWindows.First.Value.Close();

                    openWindows.RemoveFirst();
                }
            }

            e.Handled = true;
        }

        private void About_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ = new About().ShowDialog();

            e.Handled = true;
        }

        private void SubmitABug_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string url = "https://github.com/pierresprim/WinCopies/issues";

            _ = StartProcessNetCore(url);

            e.Handled = true;
        }

        static System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetEnumerable(in MainWindowViewModel dataContext) => dataContext.SelectedItem.Path.Items.WhereSelect(item => item.IsSelected, item => item.Model);

        private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // todo:
            //e.CanExecute = new EmptyCheckEnumerator<IBrowsableObjectInfoViewModel>(((MainWindowViewModel)DataContext).SelectedItem.Path.Items.Where(item => item.IsSelected).GetEnumerator()).HasItems;

            var dataContext = (MainWindowViewModel)DataContext;

            e.CanExecute = dataContext.SelectedItem.Path.ProcessFactory.CanCopy(GetEnumerable(dataContext));

            e.Handled = true;
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dataContext = (MainWindowViewModel)DataContext;

            dataContext.SelectedItem.Path.ProcessFactory.Copy(GetEnumerable(dataContext), 10u);

            e.Handled = true;
        }

        private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((MainWindowViewModel)DataContext).SelectedItem.Path.ProcessFactory.CanPaste(10u);

            e.Handled = true;
        }

        private static void _Paste(in ProcessFactorySelectorDictionaryParameters parameters)
        {
            IProcess result = new Process(BrowsableObjectInfo.DefaultProcessSelectorDictionary.Select(parameters));

            ((System.Collections.ObjectModel.ObservableCollection<IProcess>)ProcessWindow.Instance.Processes).Add(result);

            result.RunWorkerAsync();
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //StringCollection sc = System.Windows.Clipboard.GetFileDropList();

            //StringEnumerator paths = sc.GetEnumerator();

            //if (paths.MoveNext())
            //{
            //    var _paths = new List<WinCopies.IO.IPathInfo>(sc.Count);

            //    sc = null;

            //    string firstPathDirectory = System.IO.Path.GetDirectoryName(paths.Current);

            //    void addPath() => _paths.Add(new WinCopies.IO.PathInfo(System.IO.Path.GetFileName(paths.Current), System.IO.Directory.Exists(paths.Current)));

            //    addPath();

            //    while (paths.MoveNext())

            //        if (System.IO.Path.GetDirectoryName(paths.Current) == firstPathDirectory)

            //            addPath();

            //        else
            //        {
            //            _ = MessageBox.Show("The paths on the Clipboard do not have the same root.", "WinCopies", MessageBoxButton.OK, MessageBoxImage.Information);

            //            e.Handled = true;

            //            return;
            //        }

            //    var copyProcess = new CopyProcess(new PathCollection(firstPathDirectory, _paths), ((MainWindowViewModel)DataContext).SelectedItem.Path.Path);

            //    (new ProcessWindow() { Content = copyProcess }).Show();

            //    copyProcess.RunWorkerAsync();
            //}

            _Paste(new ProcessFactorySelectorDictionaryParameters(((MainWindowViewModel)DataContext).SelectedItem.Path.ProcessFactory.TryGetCopyProcessParameters(10u), App.DefaultProcessPathCollectionFactory));

            e.Handled = true;
        }

        // todo: replace with WinCopies.Util.Desktop implementation

        //#region File Drop List

        //private static void ThrowOnInvalidSleepTime(in int sleepTime)
        //{
        //    if (sleepTime < 0)

        //        throw new ArgumentOutOfRangeException(nameof(sleepTime), sleepTime, $"{nameof(sleepTime)} must be greater or equal to zero.");
        //}

        //private static bool TrySetClipboardOnSuccessHResult(in StringCollection fileDropList)
        //{
        //    try
        //    {
        //      System.Windows.  Clipboard.SetFileDropList(fileDropList);

        //        return true;
        //    }
        //    catch (Exception ex) when (CoreErrorHelper.Succeeded(ex.HResult))
        //    {
        //        return false;
        //    }
        //}

        ////private static bool TrySetClipboardOnSuccessHResult(in StringCollection fileDropList, in int sleepTime, in uint tryCount)
        ////{
        ////    ThrowOnInvalidSleepTime(sleepTime);

        ////    for (uint i = 0; i < tryCount; i++)
        ////    {
        ////        if (TrySetClipboardOnSuccessHResult(fileDropList)) return true;

        ////        Thread.Sleep(sleepTime);
        ////    }

        ////    return false;
        ////}

        //#endregion
    }
}
