using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCopies.GUI.Explorer;
using WinCopies.Util;

namespace WinCopies.SettingsManagement
{
    public class Ergonomics : INotifyPropertyChanged
    {

        protected virtual void OnPropertyChanged(string propertyName, string fieldName, object newValue, Type declaringType)

        {

            (bool propertyChanged, object oldValue) = ((INotifyPropertyChanged)this).SetProperty(propertyName, fieldName, newValue, declaringType);

            if (propertyChanged) PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName)); // OnPropertyChanged(propertyName, result.oldValue, newValue);

        }

        private readonly OpenFolderMode _openFolderMode = OpenFolderMode.OpenFoldersInSameTab;

        [SerializableProperty(new string[] { "ergonomics", "openFolderMode" })]
        public OpenFolderMode OpenFolderMode { get => _openFolderMode; set => OnPropertyChanged(nameof(OpenFolderMode), nameof(_openFolderMode), value, typeof(Ergonomics)); }

        private readonly OpenMode _clicksToOpen = OpenMode.OnDoubleClick;

        [SerializableProperty(new string[] { "ergonomics", "clicksToOpen" })]
        public OpenMode ClicksToOpen { get => _clicksToOpen; set => OnPropertyChanged(nameof(ClicksToOpen), nameof(_clicksToOpen), value, typeof(Ergonomics)); }

        private readonly UnderliningMode _underliningMode = UnderliningMode.UnderlineWhenItemIsPointed;

        [SerializableProperty(new string[] { "ergonomics", "underliningMode" })]
        public UnderliningMode UnderliningMode { get => _underliningMode; set => OnPropertyChanged(nameof(UnderliningMode), nameof(_underliningMode), value, typeof(Ergonomics)); }

        public event PropertyChangedEventHandler PropertyChanged;

        public Ergonomics() { }

        public Ergonomics(bool autoLoad)

        {

            if (autoLoad)

                SettingsManagement.LoadSettings(this);

        }
    }
}
