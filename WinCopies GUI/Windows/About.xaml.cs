/* Copyright © Pierre Sprimont, 2020
 *
 * This file is part of the WinCopies Framework.
 *
 * The WinCopies Framework is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The WinCopies Framework is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;

using WinCopies.GUI.Windows;

namespace WinCopies
{
    public class FrameworkVersions
    {
        public string WinCopiesUtilities { get; }

        public string WinCopies { get; }

        public string WindowsAPICodePack { get; }

        public FrameworkVersions()
        {
            string assemblyDirectory = App.GetAssemblyDirectory();

            string getVersion(in string assemblyName) => Assembly.LoadFile($"{assemblyDirectory}\\{assemblyName}.dll").GetName().Version.ToString();

            WinCopiesUtilities = getVersion($"{nameof(WinCopies)}.Util");

            WinCopies = getVersion($"{nameof(WinCopies)}.IO");

            WindowsAPICodePack = getVersion($"{nameof(WinCopies)}.{nameof(WindowsAPICodePack)}");
        }
    }

    public partial class About : DialogWindow
    {
        private static string GetVersion() => $"Version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion} Alpha";

        private static readonly DependencyPropertyKey VersionPropertyKey = DependencyProperty.RegisterReadOnly("Version", typeof(string), typeof(About), new PropertyMetadata(GetVersion()));

        public static readonly DependencyProperty VersionProperty = VersionPropertyKey.DependencyProperty;

        public string Version => (string)GetValue(VersionProperty);

        private static readonly DependencyPropertyKey FrameworkVersionsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(FrameworkVersions), typeof(FrameworkVersions), typeof(About), new PropertyMetadata(new FrameworkVersions()));

        public static readonly DependencyProperty FrameworkVersionsProperty = FrameworkVersionsPropertyKey.DependencyProperty;

        public FrameworkVersions FrameworkVersions => (FrameworkVersions)GetValue(FrameworkVersionsProperty);

        public About()
        {
            InitializeComponent();

            DataContext = this;

            var s = new MemoryStream();

            var w = new StreamWriter(s);

            w.Write(Properties.Resources.gpl_3_0);

            var textRange = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);

            textRange.Load(s, DataFormats.Rtf);
        }
    }
}
