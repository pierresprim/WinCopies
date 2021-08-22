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
using System.Reflection;
using System.Windows;
using System.Windows.Input;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.GUI.IO.ObjectModel;
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

        public static RoutedCommand NewRegistryTab { get; } = new RoutedUICommand(Properties.Resources.NewRegistryTab, nameof(NewRegistryTab), typeof(MainWindow));

        public static RoutedCommand NewWMITab { get; } = new RoutedUICommand(Properties.Resources.NewWMITab, nameof(NewWMITab), typeof(MainWindow));

        public static RoutedCommand CloseOtherTabs { get; } = new RoutedUICommand(Properties.Resources.CloseOtherTabs, nameof(CloseOtherTabs), typeof(MainWindow), new InputGestureCollection() { new KeyGesture(Key.W, ModifierKeys.Control | ModifierKeys.Alt) });

        public static RoutedCommand Quit { get; } = new RoutedUICommand(Properties.Resources.Quit, nameof(Quit), typeof(MainWindow));

        public static RoutedCommand SubmitABug { get; } = new RoutedUICommand(Properties.Resources.SubmitABug, nameof(SubmitABug), typeof(MainWindow));

        public MainWindow()
        {
            _ = Current._OpenWindows.AddFirst(this);

            DataContext = new MainWindowViewModel();

            InitializeComponent();
        }

        private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void AddNewTab(IExplorerControlBrowsableObjectInfoViewModel viewModel, ExecutedRoutedEventArgs e)
        {
            viewModel.IsSelected = true;

            ((MainWindowViewModel)DataContext).Paths.Add(viewModel);

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
            _ = RemoveAll(((MainWindowViewModel)DataContext).Paths, ((MainWindowViewModel)DataContext).SelectedItem, false, false);

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

                        while (collection.Count != 1)
                        {
                            if (collection[1] == itemToKeep)
                            {
                                if (throwIfMultiple) throw new InvalidOperationException("More than one occurence were found.");

                                else

                                    while (collection.Count != 1)

                                        collection.RemoveAt(1);

                                return false;
                            }

                            collection.RemoveAt(1);

                            return true;
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
            ObservableLinkedCollection<Window> openWindows = Current._OpenWindows;

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

        private System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetEnumerable() => ((MainWindowViewModel)DataContext).SelectedItem.Path.Items.WhereSelect(item => item.IsSelected, item => item.Model);

        private void CanRunCommand(in IProcessFactoryProcessInfo processFactory, in CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = processFactory.CanRun(GetEnumerable());

            e.Handled = true;
        }

        private static void RunCommand(in Action action, in ExecutedRoutedEventArgs e)
        {
            action();

            e.Handled = true;
        }

        private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>

            // todo:
            //e.CanExecute = new EmptyCheckEnumerator<IBrowsableObjectInfoViewModel>(((MainWindowViewModel)DataContext).SelectedItem.Path.Items.Where(item => item.IsSelected).GetEnumerator()).HasItems;

            CanRunCommand(GetProcessFactory().Copy, e);

        private IProcessFactory GetProcessFactory() => ((MainWindowViewModel)DataContext).SelectedItem.Path.ProcessFactory;

        private static void RunCommand(in Action action, in IRunnableProcessFactoryProcessInfo processFactory)
        {
            if (processFactory.UserConfirmationRequired && MessageBox.Show(processFactory.GetUserConfirmationText(), Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)

                return;

            action();
        }

        private void RunProcess(in ExecutedRoutedEventArgs e, in FuncIn<IProcessFactory, IRunnableProcessFactoryProcessInfo> func)
        {
            IRunnableProcessFactoryProcessInfo processFactory = func(GetProcessFactory());

            RunCommand(() => RunCommand(() => processFactory.Run(GetEnumerable(), 10u), processFactory), e);
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e) => RunProcess(e, (in IProcessFactory processFactory) => processFactory.Copy);

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e) => RunProcess(e, (in IProcessFactory processFactory) => processFactory.Cut);

        private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GetProcessFactory().CanPaste(10u);

            e.Handled = true;
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e) =>

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

            RunCommand(() => StartInstance(GetProcessFactory().Copy.TryGetProcessParameters(10u)), e);

        private void Recycle_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory().Recycling, e);

        private void Recycle_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(() => StartInstance(GetProcessFactory().Recycling.TryGetProcessParameters(GetEnumerable())), e);

        private void Empty_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory().Clearing, e);

        private void Empty_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(() => StartInstance(GetProcessFactory().Clearing.TryGetProcessParameters(GetEnumerable())), e);

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e) => CanRunCommand(GetProcessFactory().Deletion, e);

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e) => RunCommand(() => StartInstance(GetProcessFactory().Deletion.TryGetProcessParameters(GetEnumerable())), e);

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
