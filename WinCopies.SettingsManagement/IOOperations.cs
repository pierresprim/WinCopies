using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCopies.Util;

namespace WinCopies.SettingsManagement
{
    public class IOOperations : INotifyPropertyChanged
    {

        protected virtual void OnPropertyChanged(string propertyName, string fieldName, object newValue, Type declaringType)

        {

            (bool propertyChanged, object oldValue) = ((INotifyPropertyChanged)this).SetProperty(propertyName, fieldName, newValue, declaringType);

            if (propertyChanged) PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName)); // OnPropertyChanged(propertyName, result.oldValue, newValue);

        }

        private readonly FilesFusionExceptionDefaultActions _defaultActionWhenFilesFusionException = FilesFusionExceptionDefaultActions.None;

        [SerializableProperty(new string[] { "iooperations", "defaultActionWhenFilesFusionException" })]
        public FilesFusionExceptionDefaultActions DefaultActionWhenFilesFusionException { get => _defaultActionWhenFilesFusionException; set => OnPropertyChanged(nameof(DefaultActionWhenFilesFusionException), nameof(_defaultActionWhenFilesFusionException), value, typeof(IOOperations)); }

        private readonly bool _automaticallyIgnoreItemWhenOtherExceptions = false;

        [SerializableProperty(new string[] { "iooperations", "automaticallyIgnoreItemWhenOtherExceptions" })]
        public bool AutomaticallyIgnoreItemWhenOtherExceptions { get => _automaticallyIgnoreItemWhenOtherExceptions; set => OnPropertyChanged(nameof(AutomaticallyIgnoreItemWhenOtherExceptions), nameof(_automaticallyIgnoreItemWhenOtherExceptions), value, typeof(IOOperations)); }

        private readonly bool _raiseSoundWhenProcessesCompleted = false;

        [SerializableProperty(new string[] { "iooperations", "raiseSoundWhenProcessesCompleted" })]
        public bool RaiseSoundWhenProcessesCompleted { get => _raiseSoundWhenProcessesCompleted; set => OnPropertyChanged(nameof(RaiseSoundWhenProcessesCompleted), nameof(_raiseSoundWhenProcessesCompleted), value, typeof(IOOperations)); }

        private readonly bool _automaticallyCloseProcessesWindowWhenProcessesAreCompleted = false;

        [SerializableProperty(new string[] { "iooperations", "automaticallyCloseProcessesWindowWhenProcessesAreCompleted" })]
        public bool AutomaticallyCloseProcessesWindowWhenProcessesAreCompleted { get => _automaticallyCloseProcessesWindowWhenProcessesAreCompleted; set => OnPropertyChanged(nameof(AutomaticallyCloseProcessesWindowWhenProcessesAreCompleted), nameof(_automaticallyCloseProcessesWindowWhenProcessesAreCompleted), value, typeof(IOOperations)); }

        public event PropertyChangedEventHandler PropertyChanged;

        public IOOperations() { }

        public IOOperations(bool autoLoad)

        {

            if (autoLoad)

                SettingsManagement.LoadSettings(this);

        }
    }
}
