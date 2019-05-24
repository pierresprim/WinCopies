using Microsoft.WindowsAPICodePack.Shell;
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

        public static string[] ViewStyles { get; } = { (string) Application.Current.Resources["SizeOne"], (string)Application.Current.Resources["SizeTwo"], (string)Application.Current.Resources["SizeThree"], (string)Application.Current.Resources["SizeFour"],
        (string) Application.Current.Resources["List"], (string) Application.Current.Resources["Details"], (string) Application.Current.Resources["Tiles"], (string) Application.Current.Resources["Content"]};

        //[SerializableProperty]
        //public WinCopies.Util.CheckableObject[] KnownExtensionsToOpenDirectly { get; } = null;

        //public static readonly DependencyProperty StartDirectoryProperty = DependencyProperty.Register(nameof(StartDirectory), typeof(string), typeof(Common), new PropertyMetadata(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), App.PropertyChangedCallback));

        //[SerializableProperty]
        //public string StartDirectory { get => (string)GetValue(StartDirectoryProperty); set => SetValue(StartDirectoryProperty, value); }

        //// todo: value mismatch when languages are different :

        //public static readonly DependencyProperty ViewStyleProperty = DependencyProperty.Register(nameof(ViewStyle), typeof(ViewStyles), typeof(Common), new PropertyMetadata(WinCopies.GUI.Explorer.ViewStyles.SizeThree, App.PropertyChangedCallback));

        //[SerializableProperty]
        //public ViewStyles ViewStyle { get => (ViewStyles)GetValue(ViewStyleProperty); set => SetValue(ViewStyleProperty, value); }

        //public static readonly DependencyProperty ShowItemsCheckBoxProperty = DependencyProperty.Register(nameof(ShowItemsCheckBox), typeof(bool), typeof(Common), new PropertyMetadata(false, App.PropertyChangedCallback));

        //[SerializableProperty]
        //public bool ShowItemsCheckBox { get => (bool)GetValue(ShowItemsCheckBoxProperty); set => SetValue(ShowItemsCheckBoxProperty, value); }

        //public static readonly DependencyProperty ShowHiddenItemsProperty = DependencyProperty.Register(nameof(ShowHiddenItems), typeof(bool), typeof(Common), new PropertyMetadata(false, App.PropertyChangedCallback));

        //[SerializableProperty]
        //public bool ShowHiddenItems { get => (bool)GetValue(ShowHiddenItemsProperty); set => SetValue(ShowHiddenItemsProperty, value); }

        //public static readonly DependencyProperty ShowSystemItemsProperty = DependencyProperty.Register(nameof(ShowSystemItems), typeof(bool), typeof(Common), new PropertyMetadata(false, App.PropertyChangedCallback));

        //[SerializableProperty]
        //public bool ShowSystemItems { get => (bool)GetValue(ShowSystemItemsProperty); set => SetValue(ShowSystemItemsProperty, value); }

        public Common()
        {
            InitializeComponent();

            WinCopies.SettingsManagement.Common dataContext = new WinCopies.SettingsManagement.Common(true);

            dataContext.PropertyChanged += App.PropertyChangedCallback;

            DataContext = dataContext;

            //WinCopies.Util.CheckableObject[] knownExtensions = new WinCopies.Util.CheckableObject[12];

            //// todo: to add other extensions and offer to the user the possibility to select archive formats in addition to archive extensions

            //string[] knownExtensionsString = { ".zip", ".7z", ".arj", ".bz2", ".cab", ".chm", ".cfb", ".cpio", ".deb", ".udeb", ".gz", ".iso" };

            //WinCopies.Util.CheckableObject checkableString = null;

            //string knownExtension = null;

            //for (int i = 0; i <= 11; i++)

            //{

            //    knownExtension = knownExtensionsString[i];

            //    checkableString = new WinCopies.Util.CheckableObject(true, knownExtension);

            //    checkableString.PropertyChanged += CheckableString_PropertyChanged;

            //    knownExtensions[i] = checkableString;

            //}

            //KnownExtensionsToOpenDirectly = knownExtensions;

            //SettingsManagement.LoadSettings(this);

            ((App)Application.Current).IsSaved = true;

            // todo: to create a parameter in the SettingsManagement.LoadSettings method in order to determine whether we want to be notified of the value change (to change the property) or not (to change the field directly).
        }

        private void CheckableString_PropertyChanged(object sender, PropertyChangedEventArgs e) =>

        ((App)Application.Current).IsSaved = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            WinCopies.GUI.Windows.Dialogs.FolderBrowserDialog commonOpenFileDialog = new WinCopies.GUI.Windows.Dialogs.FolderBrowserDialog();

            commonOpenFileDialog.Mode = WinCopies.GUI.Windows.Dialogs.FolderBrowserDialogMode.OpenFolder;

            WinCopies.SettingsManagement.Common common = (WinCopies.SettingsManagement.Common)DataContext;

            string directory = common.StartDirectory;

            if (directory.Contains("%"))

                directory = WinCopies.IO.Path.GetRealPathFromEnvironmentVariables(directory);

            // commonOpenFileDialog.DefaultDirectory = directory;

            commonOpenFileDialog.Owner = Window.GetWindow(this);

            // todo: to make data binding

            // commonOpenFileDialog.ExplorerControl.ArchiveFormatsToOpen = InArchiveFormats.Zip;

            commonOpenFileDialog.ExplorerControl.ShowItemsCheckBox = common.ShowItemsCheckBox;

            commonOpenFileDialog.ExplorerControl.ShowHiddenItems = common.ShowHiddenItems;

            commonOpenFileDialog.ExplorerControl.ShowSystemItems = common.ShowSystemItems;

            commonOpenFileDialog.ExplorerControl.Navigate(new ShellObjectInfo(ShellObject.FromParsingName(directory), directory), true);

            if (commonOpenFileDialog.ShowDialog() == true)

                common.StartDirectory = WinCopies.IO.Path.GetShortcutPath(commonOpenFileDialog.ExplorerControl.Path.Path);

        }

        // todo: to do this action only if the start directory has changed in the meantime.

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {

            WinCopies.SettingsManagement.Common common = (WinCopies.SettingsManagement.Common)DataContext;

            common.StartDirectory = WinCopies.IO.Path.GetShortcutPath(common.StartDirectory);

        }
    }
}
