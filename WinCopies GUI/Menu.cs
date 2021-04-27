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
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WinCopies
{
    public class Menu : System.Windows.Controls.Menu
    {
        protected override DependencyObject GetContainerForItemOverride() => new MenuItem();
    }

    public class MenuItem : System.Windows.Controls.MenuItem
    {
        private static readonly DependencyPropertyKey IsSelectedPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsSelected), typeof(bool), typeof(MenuItem), new PropertyMetadata(false));

        public static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;

        public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); internal set => SetValue(IsSelectedPropertyKey, value); }

        protected override DependencyObject GetContainerForItemOverride() => new MenuItem();

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            IsSelected = true;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            IsSelected = false;
        }
    }
}
