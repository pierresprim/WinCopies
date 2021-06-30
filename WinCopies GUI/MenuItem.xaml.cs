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

using System.Linq;
using System.Windows;

namespace WinCopies
{
    public partial class MenuItem : System.Windows.Controls.MenuItem
    {
        private static readonly DependencyPropertyKey IsSelectedPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsSelected), typeof(bool), typeof(MenuItem), new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e)=>((MenuItemViewModel)((MenuItem)d).DataContext).IsSelected=(bool)e.NewValue));

        public static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;

        public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); internal set => SetValue(IsSelectedPropertyKey, value); }

        public static DependencyProperty StatusBarLabelProperty = DependencyProperty.Register(nameof(StatusBarLabel), typeof(string), typeof(MenuItem));

        public string StatusBarLabel { get => (string)GetValue(StatusBarLabelProperty); set => SetValue(StatusBarLabelProperty, value); }

        public static readonly DependencyProperty ResourceKeyProperty = DependencyProperty.Register(nameof(ResourceKey), typeof(string), typeof(MenuItem), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MenuItem)d).OnResourceKeyChanged((string)e.NewValue)));

        public string ResourceKey { get => (string)GetValue(ResourceKeyProperty); set => SetValue(ResourceKeyProperty, value); }

        public MenuItem()
        {
            InitializeComponent();
        }

        protected void OnResourceKeyChanged( string newResourceKey)
        {
            DataContext = DataContext is MenuItemViewModel menuItemViewModel ? new MenuItemViewModel(menuItemViewModel) : new MenuItemViewModel((MenuViewModel)DataContext);

            ((MenuItemViewModel)DataContext).ResourceId = newResourceKey;

            Header = (string)typeof(Properties.Resources).GetProperties().FirstOrDefault(p => p.Name == newResourceKey)?.GetValue(null);

            //StatusBarLabel = getResource($"{newResourceKey}StatusBarLabel");
        }

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
