using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WinCopies.GUI.Explorer;
using WinCopies.SettingsManagement;

namespace WinCopiesGUIWizard.Common
{
    /// <summary>
    /// Logique d'interaction pour Common.xaml
    /// </summary>
    public partial class Common : Page
    {
        public static readonly string[] EnvironmentPathVariables = { "AllUserProfile", "AppData", "CommonProgramFiles", "CommonProgramFiles(x86)", "HomeDrive", "LocalAppData", "ProgramData", "ProgramFiles", "ProgramFiles(x86)", "Public", "SystemDrive", "SystemRoot", "Temp", "UserProfile" };
        public static string[] ViewStyles { get; } = { (string) Application.Current.Resources["SizeOne"], (string)Application.Current.Resources["SizeTwo"], (string)Application.Current.Resources["SizeThree"], (string)Application.Current.Resources["SizeFour"],
        (string) Application.Current.Resources["List"], (string) Application.Current.Resources["Details"], (string) Application.Current.Resources["Tiles"], (string) Application.Current.Resources["Content"]};

        [SerializableProperty]
        public WinCopies.Util.CheckableObject[] KnownExtensionsToOpenDirectly { get; } = null;

        public static readonly DependencyProperty StartDirectoryProperty = DependencyProperty.Register(nameof(StartDirectory), typeof(string), typeof(Common), new PropertyMetadata(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), App.PropertyChangedCallback));

        [SerializableProperty]
        public string StartDirectory { get => (string)GetValue(StartDirectoryProperty); set => SetValue(StartDirectoryProperty, value); }

        // todo: value mismatch when languages are different :

        public static readonly DependencyProperty ViewStyleProperty = DependencyProperty.Register(nameof(ViewStyle), typeof(ViewStyles), typeof(Common), new PropertyMetadata(WinCopies.GUI.Explorer.ViewStyles.SizeThree, App.PropertyChangedCallback));

        [SerializableProperty]
        public ViewStyles ViewStyle { get => (ViewStyles)GetValue(ViewStyleProperty); set => SetValue(ViewStyleProperty, value); }

        public static readonly DependencyProperty ShowItemsCheckBoxProperty = DependencyProperty.Register(nameof(ShowItemsCheckBox), typeof(bool), typeof(Common), new PropertyMetadata(false, App.PropertyChangedCallback));

        [SerializableProperty]
        public bool ShowItemsCheckBox { get => (bool)GetValue(ShowItemsCheckBoxProperty); set => SetValue(ShowItemsCheckBoxProperty, value); }

        public static readonly DependencyProperty ShowHiddenItemsProperty = DependencyProperty.Register(nameof(ShowHiddenItems), typeof(bool), typeof(Common), new PropertyMetadata(false, App.PropertyChangedCallback));

        [SerializableProperty]
        public bool ShowHiddenItems { get => (bool)GetValue(ShowHiddenItemsProperty); set => SetValue(ShowHiddenItemsProperty, value); }

        public static readonly DependencyProperty ShowSystemItemsProperty = DependencyProperty.Register(nameof(ShowSystemItems), typeof(bool), typeof(Common), new PropertyMetadata(false, App.PropertyChangedCallback));

        [SerializableProperty]
        public bool ShowSystemItems { get => (bool)GetValue(ShowSystemItemsProperty); set => SetValue(ShowSystemItemsProperty, value); }

        public Common()
        {
            InitializeComponent();

            WinCopies.Util.CheckableObject[] knownExtensions = new WinCopies.Util.CheckableObject[12];

            string[] knownExtensionsString = { ".zip", ".7z", ".arj", ".bz2", ".cab", ".chm", ".cfb", ".cpio", ".deb", ".udeb", ".gz", ".iso" };

            WinCopies.Util.CheckableObject checkableString = null;

            string knownExtension = null;

            for (int i = 0; i <= 11; i++)

            {

                knownExtension = knownExtensionsString[i];

                checkableString = new WinCopies.Util.CheckableObject(true, knownExtension);

                checkableString.PropertyChanged += CheckableString_PropertyChanged;

                knownExtensions[i] = checkableString;

            }

            KnownExtensionsToOpenDirectly = knownExtensions;

            SettingsManagement.LoadSettings(this);

            ((App)Application.Current).IsSaved = true;

            // todo: to create a parameter in the SettingsManagement.LoadSettings method in order to determine whether we want to be notified of the value change (to change the property) or not (to change the field directly).
        }

        private void CheckableString_PropertyChanged(object sender, PropertyChangedEventArgs e) =>

        ((App)Application.Current).IsSaved = false;    

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog commonOpenFileDialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();

            commonOpenFileDialog.IsFolderPicker = true;

            string directory = StartDirectory.Contains("%") ? Environment.GetEnvironmentVariable(StartDirectory.Replace("%", null)) : StartDirectory;

            commonOpenFileDialog.InitialDirectory = directory;

            commonOpenFileDialog.DefaultDirectory = directory;

            if (commonOpenFileDialog.ShowDialog(Window.GetWindow(this)) == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)

                StartDirectory = GetShortcutPath(commonOpenFileDialog.FileName);

        }

        public static string GetShortcutPath(string path)

        {

            List<KeyValuePair<string, string>> paths = new List<KeyValuePair<string, string>>();

            foreach (var environmentPathVariable in EnvironmentPathVariables)

            {

                string _path = Environment.GetEnvironmentVariable(environmentPathVariable);

                if (_path != null)

                    paths.Add(new KeyValuePair<string, string>(environmentPathVariable, _path));

            }



            paths.Sort((KeyValuePair<string, string> x, KeyValuePair<string, string> y) => x.Value.Length < y.Value.Length ? 1 : x.Value.Length == y.Value.Length ? 0 : -1);



            foreach (KeyValuePair<string, string> _path in paths)

                if (path.StartsWith(_path.Value))

                {

                    path = "%" + _path.Key + "%" + path.Substring(_path.Value.Length);

                    break;

                }

            return path;

        }

        // todo: to do this action only if the start directory has changed in the meantime.

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => StartDirectory = GetShortcutPath(StartDirectory);

    }
}
