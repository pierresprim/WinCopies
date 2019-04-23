using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WinCopiesGUIWizard
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public const string Common = "Common";

        public const string Ergonomics = "Ergonomics";

        public const string History = "History";

        public const string IOOperations = "IOOperations";

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public string Source { get => (string)GetValue(SourceProperty); set => SetValue(SourceProperty, value); }

        public static readonly DependencyProperty TreeViewItemsSourceProperty = DependencyProperty.Register(nameof(TreeViewItemsSource), typeof(TreeViewItem[]), typeof(MainWindow), new PropertyMetadata(null));

        public TreeViewItem[] TreeViewItemsSource { get => (TreeViewItem[])GetValue(TreeViewItemsSourceProperty); set => SetValue(TreeViewItemsSourceProperty, value); }

        public MainWindow()

        {

            TreeViewItem getTreeViewItem(string headerResourceName, string pageUri, object[] items)

            {

                if (items != null)

                    return new TreeViewItem((string)Application.Current.Resources[headerResourceName], pageUri, new TreeViewItem[] { getTreeViewItem((string)items[0], (string)items[1], (object[])items[2]) });

                else

                    return new TreeViewItem((string)Application.Current.Resources[headerResourceName], pageUri, null);

            }

            TreeViewItem[] getTreeViewItems(object[][] treeViewItems)

            {

                TreeViewItem[] _treeViewItems = new TreeViewItem[treeViewItems.Length];

                for (int i = 0; i < treeViewItems.Length; i++)

                    _treeViewItems[i] = getTreeViewItem((string)treeViewItems[i][0], (string)treeViewItems[i][1], (object[])treeViewItems[i][2]);

                return _treeViewItems;

            }

            InitializeComponent();

            TreeViewItem[] treeViewItems_ = getTreeViewItems(new object[][] { new object[] { Common, $"{Common}/{Common}.xaml", new object[] { Common, $"{Common}/{Common}.xaml", null } },

            new object[] { Ergonomics, $"{Ergonomics}/{Common}.xaml", new object[] { Common, $"{Ergonomics}/{Common}.xaml", null } },

            new object [] { History, $"{History}/{Common}.xaml", new object[] { Common, $"{History}/{Common}.xaml", null } } ,

            new object[] { IOOperations, $"{IOOperations}/{Common}.xaml", new object[] { Common, $"{IOOperations}/{Common}.xaml", null } } });

            treeViewItems_[0].IsExpanded = true;

            treeViewItems_[0][0].IsSelected = true;

            TreeViewItemsSource = treeViewItems_;

            Source = $"{Common}/{Common}.xaml";

        }

        public bool SaveSettings()

        {

            ((App)Application.Current).IsApplyingChanges = true;

            bool result = WinCopies.SettingsManagement.SettingsManagement.SaveSettings((Page)Frame.Content);

            if (result)

                ((App)Application.Current).IsSaved = true;

            else

                App.OnUnableToSaveSettingFile();

            return result;

        }

        private bool OnTreeViewSelectionChanged()

        {

            if (((App)Application.Current).IsSaved)

                return true;

            MessageBoxResult messageBoxResult = MessageBox.Show((string)Application.Current.Resources["SettingsAreNotSaved"], System.Reflection.Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            return messageBoxResult == MessageBoxResult.Yes ? SaveSettings() : messageBoxResult == MessageBoxResult.No ? true : false;

        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            if (OnTreeViewSelectionChanged())

                Source = ((TreeViewItem)e.NewValue).PageUri;

            else

                ((TreeViewItem)e.OldValue).IsSelected = true;

        }

        private void Window_Closing(object sender, CancelEventArgs e)

        {

            if (!OnTreeViewSelectionChanged())

                e.Cancel = true;

        }

        private void Button_Click(object sender, RoutedEventArgs e) => SaveSettings();

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            if (((App)Application.Current).IsSaved)

            {

                Close();

                return;

            }

            // MessageBoxResult result = MessageBox.Show("Are you sure you do not want to save settings?", "WinCopies", MessageBoxButton.YesNo);

            // if (result == MessageBoxResult.Yes)

            // Close();

        }
    }
}
