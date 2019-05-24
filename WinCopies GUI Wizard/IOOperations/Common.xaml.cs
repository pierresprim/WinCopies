using System.Windows;
using System.Windows.Controls;
using WinCopies.SettingsManagement;

namespace WinCopiesGUIWizard.IOOperations
{
    /// <summary>
    /// Logique d'interaction pour Common.xaml
    /// </summary>
    public partial class Common : Page
    {

        // todo:

        //public static readonly DependencyProperty HideFoldersFusionExceptionsProperty = DependencyProperty.Register(nameof(HideFoldersFusionExceptions), typeof(bool), typeof(Common), new PropertyMetadata(true, App.PropertyChangedCallback));

        //[SerializableProperty()]
        //public bool HideFoldersFusionExceptions { get => (bool)GetValue(HideFoldersFusionExceptionsProperty); set => SetValue(HideFoldersFusionExceptionsProperty, value); }

        public Common()

        {

            InitializeComponent();

            WinCopies.SettingsManagement.IOOperations dataContext = new WinCopies.SettingsManagement.IOOperations(true);

            dataContext.PropertyChanged += App.PropertyChangedCallback;

            DataContext = dataContext;

            ((App)Application.Current).IsSaved = true;

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) => ((WinCopies.SettingsManagement.IOOperations)DataContext).DefaultActionWhenFilesFusionException = FilesFusionExceptionDefaultActions.Ignore;
    }
}
