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
        public static readonly DependencyProperty OpenFolderModeProperty = DependencyProperty.Register("OpenFolderMode", typeof(OpenFolderMode), typeof(Common), new PropertyMetadata(OpenFolderMode.OpenFoldersInSameTab, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

#if DEBUG 
        PropertyChanged(
            e
#else
            App.PropertyChangedCallback
#endif
            )));

        [SerializableProperty]
        public OpenFolderMode OpenFolderMode { get => (OpenFolderMode)GetValue(OpenFolderModeProperty); set => SetValue(OpenFolderModeProperty, value); }
        
        public static readonly DependencyProperty ClicksToOpenProperty = DependencyProperty.Register("ClicksToOpen", typeof(ClicksToOpen), typeof(Common), new PropertyMetadata(ClicksToOpen.OpenOnSecondClick, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

#if DEBUG 
        PropertyChanged(
            e
#else
            App.PropertyChangedCallback
#endif
            )));
        
        [SerializableProperty]
        public ClicksToOpen ClicksToOpen { get => (ClicksToOpen)GetValue(ClicksToOpenProperty); set => SetValue(ClicksToOpenProperty, value); }

        public static readonly DependencyProperty UnderliningModeProperty = DependencyProperty.Register("UnderliningMode", typeof(UnderliningMode), typeof(Common), new PropertyMetadata(UnderliningMode.UnderlineWhenItemIsPointed, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

#if DEBUG 
        PropertyChanged(
            e
#else
            App.PropertyChangedCallback
#endif
            ))); 
        
        [SerializableProperty] 
        public UnderliningMode UnderliningMode { get => (UnderliningMode)GetValue(UnderliningModeProperty); set => SetValue(UnderliningModeProperty, value); }

        public Common()

        {

            InitializeComponent();

            SettingsManagement.LoadSettings(this);

            ((App)Application.Current).IsSaved = true;

        } 

#if DEBUG 
        private static void PropertyChanged(
            DependencyPropertyChangedEventArgs e
            )

        {

            Debug.WriteLine(e.NewValue);

            ((App)Application.Current).IsSaved = false;

        } 

#endif 
    }
}
