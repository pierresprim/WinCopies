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

        public static readonly DependencyProperty HideFoldersFusionExceptionsProperty = DependencyProperty.Register(nameof(HideFoldersFusionExceptions), typeof(bool), typeof(Common), new PropertyMetadata(true, App.PropertyChangedCallback));

        [SerializableProperty()]
        public bool HideFoldersFusionExceptions { get => (bool)GetValue(HideFoldersFusionExceptionsProperty); set => SetValue(HideFoldersFusionExceptionsProperty, value); }

        public static readonly DependencyProperty DefaultActionWhenFilesFusionExceptionProperty = DependencyProperty.Register(nameof(DefaultActionWhenFilesFusionException), typeof(FilesFusionExceptionDefaultActions), typeof(Common), new PropertyMetadata(FilesFusionExceptionDefaultActions.None, App.PropertyChangedCallback));

        [SerializableProperty()]
        public FilesFusionExceptionDefaultActions DefaultActionWhenFilesFusionException { get => (FilesFusionExceptionDefaultActions)GetValue(DefaultActionWhenFilesFusionExceptionProperty); set => SetValue(DefaultActionWhenFilesFusionExceptionProperty, value); }

        public static readonly DependencyProperty AutomaticallyIgnoreItemWhenOtherExceptionsProperty = DependencyProperty.Register(nameof(AutomaticallyIgnoreItemWhenOtherExceptions), typeof(bool), typeof(Common), new PropertyMetadata(false, App.PropertyChangedCallback));

        [SerializableProperty()]
        public bool AutomaticallyIgnoreItemWhenOtherExceptions { get => (bool)GetValue(AutomaticallyIgnoreItemWhenOtherExceptionsProperty); set => SetValue(AutomaticallyIgnoreItemWhenOtherExceptionsProperty, value); }

        public static readonly DependencyProperty RaiseSoundWhenProcessesCompletedProperty = DependencyProperty.Register(nameof(RaiseSoundWhenProcessesCompleted), typeof(bool), typeof(Common), new PropertyMetadata(false, App.PropertyChangedCallback));

        [SerializableProperty()]
        public bool RaiseSoundWhenProcessesCompleted { get => (bool)GetValue(RaiseSoundWhenProcessesCompletedProperty); set => SetValue(RaiseSoundWhenProcessesCompletedProperty, value); }

        public static readonly DependencyProperty AutomaticallyCloseProcessesWindowWhenProcessesAreCompletedProperty = DependencyProperty.Register(nameof(AutomaticallyCloseProcessesWindowWhenProcessesAreCompleted), typeof(bool), typeof(Common), new PropertyMetadata(false, App.PropertyChangedCallback));

        [SerializableProperty()]
        public bool AutomaticallyCloseProcessesWindowWhenProcessesAreCompleted { get => (bool)GetValue(AutomaticallyCloseProcessesWindowWhenProcessesAreCompletedProperty); set => SetValue(AutomaticallyCloseProcessesWindowWhenProcessesAreCompletedProperty, value); }

        public Common()

        {

            InitializeComponent();

            SettingsManagement.LoadSettings(this);

            ((App)Application.Current).IsSaved = true; 

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) => DefaultActionWhenFilesFusionException = FilesFusionExceptionDefaultActions.Ignore;
    }
}
