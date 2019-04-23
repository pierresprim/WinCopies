using System.Windows;
using System.Windows.Controls;
using WinCopies.SettingsManagement;

namespace WinCopiesGUIWizard.History
{
    /// <summary>
    /// Logique d'interaction pour Common.xaml
    /// </summary>
    public partial class Common : Page
    {

        public static readonly DependencyProperty ShowFileHistoryProperty = DependencyProperty.Register("ShowFileHistory", typeof(bool), typeof(Common), new PropertyMetadata(true, App.PropertyChangedCallback));

        [SerializableProperty]
        public bool ShowFileHistory { get => (bool)GetValue(ShowFileHistoryProperty); set => SetValue(ShowFileHistoryProperty, value); }

        public static readonly DependencyProperty ShowFolderHistoryProperty = DependencyProperty.Register("ShowFolderHistory", typeof(bool), typeof(Common), new PropertyMetadata(true, App.PropertyChangedCallback));

        [SerializableProperty]
        public bool ShowFolderHistory { get => (bool)GetValue(ShowFolderHistoryProperty); set => SetValue(ShowFolderHistoryProperty, value); }

        public Common()

        {

            InitializeComponent();

            SettingsManagement.LoadSettings(this);

            ((App)Application.Current).IsSaved = true;

        }
    }
}
