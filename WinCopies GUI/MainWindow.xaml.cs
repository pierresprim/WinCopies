using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using WinCopies.Collections;
using WinCopies.GUI;
using WinCopies.GUI.IO;
using WinCopies.IO;
using WinCopies.Properties;
using WinCopies.Util;
using WinCopies.Util.Commands;
using static WinCopies.App;

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

        public static RoutedCommand NewTab => Util.Commands.ApplicationCommands.NewTab;

        public static RoutedCommand NewWindow => Util.Commands.ApplicationCommands.NewWindow;

        public static RoutedCommand CloseTab => Util.Commands.ApplicationCommands.CloseTab;

        public static RoutedCommand CloseOtherTabs { get; } = new RoutedUICommand(Properties.Resources.CloseOtherTabs, nameof(CloseOtherTabs), typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.W, ModifierKeys.Control | ModifierKeys.Alt) });

        public static RoutedCommand CloseAllTabs => Util.Commands.ApplicationCommands.CloseAllTabs;

        public static RoutedCommand CloseWindow => System.Windows.Input.ApplicationCommands.Close;

        public static RoutedCommand Quit { get; } = new RoutedUICommand(Properties.Resources.Quit, nameof(Quit), typeof(MainWindow));

        public static RoutedCommand About { get; } = new RoutedUICommand(Properties.Resources.About, nameof(About), typeof(MainWindow));

        public static RoutedCommand SubmitABug { get; } = new RoutedUICommand(Properties.Resources.SubmitABug, nameof(SubmitABug), typeof(MainWindow));

        public static Func<MainWindowViewModel, object> GetCloseTabParameter { get; } = mainWindow => mainWindow.SelectedItem;

        public static Func<MainWindowViewModel, object> GetCloseOtherTabsParameter { get; } = mainWindow => mainWindow.SelectedItem;

        public static ImageSource NewTabIcon { get; } = GUI.Icons.Properties.Resources.tab_add.ToImageSource();

        public static ImageSource NewWindowIcon { get; } = GUI.Icons.Properties.Resources.application_add.ToImageSource();

        public static ImageSource CloseTabIcon { get; } = GUI.Icons.Properties.Resources.tab_delete.ToImageSource();

        public static ImageSource SubmitABugIcon { get; } = GUI.Icons.Properties.Resources.bug.ToImageSource();

        public MainWindow()
        {
            _ = _OpenWindows.AddFirst(this);

            var mainWindow = new MainWindowViewModel();

            var xml = new XmlDocument();

            xml.LoadXml(Properties.Resources.Menus);

            XmlElement xmlMenu = xml.DocumentElement;

            var arrayBuilder = new ArrayBuilder<PropertyInfo>();

            PropertyInfo[] mainWindowProperties = typeof(MainWindow).GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (PropertyInfo p in mainWindowProperties.Where(p => p.PropertyType.IsAssignableFrom(typeof(RoutedCommand))))

                _ = arrayBuilder.AddLast(p);

            PropertyInfo[] commands = arrayBuilder.ToArray(true);

            foreach (PropertyInfo p in mainWindowProperties.Where(p => p.Name.EndsWith("Parameter")))

                _ = arrayBuilder.AddLast(p);

            PropertyInfo[] commandParameters = arrayBuilder.ToArray(true);

            foreach (PropertyInfo p in mainWindowProperties.Where(p => p.PropertyType.IsAssignableFrom(typeof(ImageSource))))

                _ = arrayBuilder.AddLast(p);

            PropertyInfo[] iconImageSources = arrayBuilder.ToArray(true);

            string getStringResource(string resourceId) => (string)ResourceProperties.First(p => p.Name == resourceId).GetValue(null);

            RoutedCommand getCommand(string resourceId) => (RoutedCommand)commands.FirstOrDefault(c => c.Name == resourceId)?.GetValue(null);

            Func getCommandParameter(string resourceId)
            {
                PropertyInfo p = commandParameters.FirstOrDefault(p => p.Name == $"{resourceId}Parameter");

                object value;

                if (p != null)

                {

                    value = p.GetValue(null);

                    return new Func(() => value);

                }

                p = commandParameters.FirstOrDefault(p => p.Name == $"Get{resourceId}Parameter");

                if (p == null)

                    return null;

                value = p.GetValue(null);

                return new Func(() => ((Func<MainWindowViewModel, object>)value)(mainWindow));
            }

            ImageSource getIconImageSource(string resourceId) => (ImageSource)iconImageSources.FirstOrDefault(i => i.Name == $"{resourceId}Icon")?.GetValue(null);

            void addMenuItem(XmlElement xmlMenuItem, MenuItemViewModel menuItem)
            {
                string resourceId;

                foreach (XmlElement _xmlMenuItem in xmlMenuItem)
                {
                    resourceId = _xmlMenuItem.Attributes["Resource"].Value;

                    addMenuItem(_xmlMenuItem, new MenuItemViewModel(menuItem, getStringResource(resourceId), resourceId, getCommand(resourceId), getCommandParameter(resourceId), getIconImageSource(resourceId)));
                }
            }

            string resourceId;

            foreach (XmlElement xmlMenuItem in xmlMenu)
            {
                resourceId = xmlMenuItem.Attributes["Resource"].Value;

                addMenuItem(xmlMenuItem, new MenuItemViewModel(mainWindow.Menu, getStringResource(resourceId), resourceId, null, null, null));
            }

            DataContext = mainWindow;

            InitializeComponent();
        }

        ~MainWindow()
        {
            _ = _OpenWindows.Remove(this);
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void NewTab_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ExplorerControlBrowsableObjectInfoViewModel explorerControlBrowsableObjectInfo = MainWindowViewModel.GetNewExplorerControlBrowsableObjectInfo();

            explorerControlBrowsableObjectInfo.IsSelected = true;

            ((MainWindowViewModel)DataContext).Paths.Add(explorerControlBrowsableObjectInfo);

            e.Handled = true;
        }

        private void NewWindow_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new MainWindow().Show();

            e.Handled = true;
        }

        private void CloseTab_CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((MainWindowViewModel)DataContext).Paths.Count > 1;

            e.Handled = true;
        }

        private void CloseTab_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ = ((MainWindowViewModel)DataContext).Paths.Remove((ExplorerControlBrowsableObjectInfoViewModel)(e.Parameter is Func func ? func() : e.Parameter));

            e.Handled = true;
        }

        private void CloseOtherTabs_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ = RemoveAll(((MainWindowViewModel)DataContext).Paths, (ExplorerControlBrowsableObjectInfoViewModel)(e.Parameter is Func func ? func() : e.Parameter), false, false);

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

                                if (throwIfMultiple)

                                    throw new InvalidOperationException("More than one occurences was found.");

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

        private void CloseAllTabs_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Collections.ObjectModel.ObservableCollection<ExplorerControlBrowsableObjectInfoViewModel> paths = ((MainWindowViewModel)DataContext).Paths;

            paths.Clear();

            ExplorerControlBrowsableObjectInfoViewModel explorerControlBrowsableObjectInfo = MainWindowViewModel.GetNewExplorerControlBrowsableObjectInfo();

            explorerControlBrowsableObjectInfo.IsSelected = true;

            paths.Add(explorerControlBrowsableObjectInfo);

            e.Handled = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            System.Collections.ObjectModel.ObservableCollection<ExplorerControlBrowsableObjectInfoViewModel> paths = ((MainWindowViewModel)DataContext).Paths;

            if (!IsClosing && paths.Count > 1 && MessageBox.Show(this, Properties.Resources.WindowClosingMessage, "WinCopies", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)

                e.Cancel = true;

            base.OnClosing(e);
        }

        private void CloseWindow_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();

            e.Handled = true;
        }

        private void Quit_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Collections.Generic.LinkedList<MainWindow> openWindows = _OpenWindows;

            if (openWindows.Count == 1 || MessageBox.Show(this, Properties.Resources.ApplicationClosingMessage, "WinCopies", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)

            {

                IsClosing = true;

                while (openWindows.Count > 0)
                {
                    openWindows.First.Value.Close();

                    openWindows.RemoveFirst();
                }

            }

            e.Handled = true;
        }

        private void About_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ = new About().ShowDialog();

            e.Handled = true;
        }

        private void SubmitABug_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string url = "https://github.com/pierresprim/WinCopies/issues";

            // https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/

            //try
            //{
            //    Process.Start(url);
            //}
            //catch
            //{
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            //}
            //    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            //    {
            //        Process.Start("xdg-open", url);
            //    }
            //    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            //    {
            //        Process.Start("open", url);
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            e.Handled = true;
        }
    }
}
