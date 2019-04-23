using System;

namespace WinCopies.SettingsManagement
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SerializablePropertyAttribute : Attribute
    {

        public string[] RefersTo { get; } = null;

        public SerializablePropertyAttribute() { }

        public SerializablePropertyAttribute(string[] refersTo) => RefersTo = refersTo;

    }
}
