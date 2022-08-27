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
using System.Windows;
using System.Windows.Input;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.GUI.IO;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO;
using WinCopies.IO.Process;

using static System.Windows.MessageBoxButton;
using static System.Windows.MessageBoxImage;
using static System.Windows.MessageBoxResult;

using static WinCopies.App;
using static WinCopies.UtilHelpers;

namespace WinCopies
{
    public partial class MainWindow : BrowsableObjectInfoWindow//BrowsableObjectInfoWindow2
    {
        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow() => new MainWindow();

        public override ClientVersion ClientVersion => IO.ObjectModel.BrowsableObjectInfo.DefaultClientVersion;

        public MainWindow(in bool createDefaultTab = true) : base(new MainWindowViewModel(), createDefaultTab) => Init();

        public MainWindow(in IBrowsableObjectInfoWindowViewModel dataContext) : base(dataContext) => Init();

        private void Init()
        {
            InitializeComponent();

            _ = Current._OpenWindows.AddFirst(this);

            MainWindowModel.Init(((IBrowsableObjectInfoWindowViewModel)DataContext).Paths.Paths);
        }

        //protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow() => new MainWindow();

        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow(in IBrowsableObjectInfoWindowViewModel dataContext) => new MainWindow(dataContext);

        protected override void OnAboutWindowRequested(ExecutedRoutedEventArgs e) => new About().ShowDialog();

        private IProcessParameters GetProcessParameters(in IDirectProcessInfo processInfo) => processInfo.TryGetProcessParameters(GetEnumerable());

        protected override void OnDelete(ExecutedRoutedEventArgs e) => StartInstance(GetProcessFactory().Deletion, GetProcessParameters);

        protected override void OnEmpty(ExecutedRoutedEventArgs e) => StartInstance(GetProcessFactory().Clearing, GetProcessParameters);

        protected override void OnPaste(ExecutedRoutedEventArgs e) => StartInstance(GetProcessFactory().Copy, (in IRunnableProcessInfo processInfo) => processInfo.TryGetProcessParameters(10u));

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _ = Current._OpenWindows.Remove2(this);
        }

        protected override void OnQuit(ExecutedRoutedEventArgs e)
        {
            ObservableLinkedCollection<Window> openWindows = Current._OpenWindows;

            if (openWindows.Count == 1u || MessageBox.Show(this, Properties.Resources.ApplicationClosingMessage, "WinCopies", YesNo, Question, No) == Yes)
            {
                Current.IsClosing = true;

                while (openWindows.Count > 0)
                {
                    openWindows.First.Value.Close();

                    openWindows.RemoveFirst();
                }
            }
        }

        protected override void OnRecycle(ExecutedRoutedEventArgs e) => StartInstance(GetProcessFactory().Recycling, GetProcessParameters);

        protected override void OnSubmitABug(ExecutedRoutedEventArgs e)
        {
            string url = "https://github.com/pierresprim/WinCopies/issues";

            _ = StartProcessNetCore(url);
        }

        protected override IBrowsableObjectInfoWindowViewModel GetDefaultDataContextOverride() => new MainWindowViewModel(GetDefaultBrowsableObjectInfoCollection());
        protected override IBrowsableObjectInfoCollectionViewModel GetDefaultBrowsableObjectInfoCollection() => new MainWindowPathCollectionViewModel();
        protected override IBrowsableObjectInfoCollectionViewModel GetDefaultBrowsableObjectInfoCollection(in System.Collections.Generic.IEnumerable<IExplorerControlViewModel> items) => new MainWindowPathCollectionViewModel(items);

        //public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(nameof(Menu), typeof(MenuViewModel), typeof(MainWindow));

        //public MenuViewModel Menu { get => (MenuViewModel)GetValue(MenuProperty); set => SetValue(MenuProperty, value); }

        //public static readonly DependencyPropertyKey SelectedMenuItemPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedMenuItem), typeof(MenuItemViewModel), typeof(MainWindow), new PropertyMetadata(false));

        //public static readonly DependencyProperty SelectedMenuItemProperty = SelectedMenuItemPropertyKey.DependencyProperty;

        //public MenuItemViewModel SelectedMenuItem { get => (MenuItemViewModel)GetValue(SelectedMenuItemProperty); internal set => SetValue(SelectedMenuItemPropertyKey, value); }

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
