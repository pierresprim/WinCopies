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

using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.COMNative.Shell;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Desktop;
using WinCopies.GUI.IO.Controls;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.Shell;
using WinCopies.GUI.Windows;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.Linq;

using static System.Windows.MessageBoxButton;
using static System.Windows.MessageBoxImage;
using static System.Windows.MessageBoxResult;

using static WinCopies.App;
using static WinCopies.UtilHelpers;

namespace WinCopies
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BrowsableObjectInfoWindow
    {
        public MainWindow() : base(new MainWindowViewModel()) => InitializeComponent();

        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow() => new MainWindow();

        protected override void OnAboutWindowRequested() => new About().ShowDialog();

        protected override void OnDelete() => StartInstance(GetProcessFactory().Deletion.TryGetProcessParameters(GetEnumerable()));

        protected override void OnEmpty() => StartInstance(GetProcessFactory().Clearing.TryGetProcessParameters(GetEnumerable()));

        protected override void OnPaste() => StartInstance(GetProcessFactory().Copy.TryGetProcessParameters(10u));

        protected override void OnQuit()
        {
            ObservableLinkedCollection<System.Windows.Window> openWindows = Current._OpenWindows;

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

        protected override void OnRecycle() => StartInstance(GetProcessFactory().Recycling.TryGetProcessParameters(GetEnumerable()));

        protected override void OnSubmitABug()
        {
            string url = "https://github.com/pierresprim/WinCopies/issues";

            _ = StartProcessNetCore(url);
        }

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
