using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WinCopies.SettingsManagement;

namespace WinCopiesGUIWizard.Ergonomics
{
    /// <summary>
    /// Logique d'interaction pour Ergonomics.xaml
    /// </summary>
    public partial class Common : Page
    {
        public Common()

        {

            InitializeComponent();

            WinCopies.SettingsManagement.Ergonomics dataContext = new WinCopies.SettingsManagement.Ergonomics(true);

            dataContext.PropertyChanged += App.PropertyChangedCallback;

            DataContext = dataContext;

            ((App)Application.Current).IsSaved = true;

        }
    }
}
