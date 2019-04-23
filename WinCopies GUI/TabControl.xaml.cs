using System.Windows;
using System.Windows.Input;

namespace WinCopies.GUI
{
    /// <summary>
    /// Logique d'interaction pour TabControl.xaml
    /// </summary>
    public partial class TabControl : WinCopies.GUI.Controls.TabControl
    {
        public TabControl() => InitializeComponent();

        protected override DependencyObject GetContainerForItemOverride() => new TabItem();

        private void CloseTab_CanExecute(object sender, CanExecuteRoutedEventArgs e) => MainWindow.OnCloseTab_CanExecute((MainWindow)Window.GetWindow(this), e);

        private void CloseTab_Executed(object sender, ExecutedRoutedEventArgs e) => MainWindow.OnCloseTab_Executed((MainWindow)Window.GetWindow(this), e);

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // MessageBox.Show((( SelectedItem.Header);
        }
    }
}
