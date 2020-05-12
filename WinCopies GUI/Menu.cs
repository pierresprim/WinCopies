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
