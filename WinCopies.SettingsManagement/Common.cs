
using System;
using System.ComponentModel;
using WinCopies.GUI.Explorer;
using WinCopies.Util;
using static WinCopies.SettingsManagement.SettingsManagement;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;

namespace WinCopies.SettingsManagement
{
    public class Common : INotifyPropertyChanged
    {

        protected virtual void OnPropertyChanged(string propertyName, string fieldName, object newValue, Type declaringType)

        {

            (bool propertyChanged, object oldValue) = ((INotifyPropertyChanged)this).SetProperty(propertyName, fieldName, newValue, declaringType);

            if (propertyChanged) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // OnPropertyChanged(propertyName, result.oldValue, newValue);

        }

        private readonly string _startDirectory = "%userprofile%\\desktop";

        [SerializableProperty(new string[] { "common", "startDirectory" })]
        public string StartDirectory { get => _startDirectory; set => OnPropertyChanged(nameof(StartDirectory), nameof(_startDirectory), value, typeof(Common)); }

        private readonly ViewStyles _viewStyle = ViewStyles.SizeThree;

        [SerializableProperty(new string[] { "common", "viewStyle" })]
        public ViewStyles ViewStyle
        {
            get => _viewStyle; set => OnPropertyChanged(nameof(ViewStyle), nameof(_viewStyle), value, typeof(Common));
        }

        private readonly bool _showItemsCheckBox = false;

        [SerializableProperty(new string[] { "common", "showItemsCheckBox" })]
        public bool ShowItemsCheckBox
        {
            get => _showItemsCheckBox; set => OnPropertyChanged(nameof(ShowItemsCheckBox), nameof(_showItemsCheckBox), value, typeof(Common));
        }

        private readonly bool _showHiddenItems = false;

        [SerializableProperty(new string[] { "common", "showHiddenItems" })]
        public bool ShowHiddenItems
        {
            get => _showHiddenItems; set => OnPropertyChanged(nameof(ShowHiddenItems), nameof(_showHiddenItems), value, typeof(Common));
        }

        private readonly bool _showSystemItems = false;

        [SerializableProperty(new string[] { "common", "showSystemItems" })]
        public bool ShowSystemItems
        {
            get => _showSystemItems; set => OnPropertyChanged(nameof(ShowSystemItems), nameof(_showSystemItems), value, typeof(Common));
        }

        [SerializableProperty(new string[] { "common", "knownExtensionsToOpenDirectly" })]
        public CheckableObject[] KnownExtensionsToOpenDirectly { get; } = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public Common() { }

        public Common(bool autoLoad)

        {

            // todo: to use a function in the Common class of the SettingsManagement dll to update this property

            CheckableObject[] knownExtensions = new CheckableObject[12];

            // todo: to add other extensions and offer to the user the possibility to select archive formats in addition to archive extensions

            string[] knownExtensionsString = { ".zip", ".7z", ".arj", ".bz2", ".cab", ".chm", ".cfb", ".cpio", ".deb", ".udeb", ".gz", ".iso" };

            CheckableObject checkableString = null;

            string knownExtension = null;

            for (int i = 0; i <= 11; i++)

            {

                knownExtension = knownExtensionsString[i];

                checkableString = new CheckableObject(true, knownExtension);

                checkableString.PropertyChanged += (object sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);

                knownExtensions[i] = checkableString;

            }

            KnownExtensionsToOpenDirectly = knownExtensions;

            if (autoLoad)

                LoadSettings(this);

        }

    }
}
