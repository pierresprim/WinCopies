using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;

namespace WinCopies
{
    /// <summary>
    /// Logique d'interaction pour About.xaml
    /// </summary>
    public partial class About : WinCopies.GUI.Windows.Dialogs.DialogWindow
    {

        private static string GetVersion() => $"Version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion} Beta";

        private static readonly DependencyPropertyKey VersionPropertyKey = DependencyProperty.RegisterReadOnly("Version", typeof(string), typeof(About), new PropertyMetadata(GetVersion()));

        public static readonly DependencyProperty VersionProperty = VersionPropertyKey.DependencyProperty;

        public string Version => (string)GetValue(VersionProperty);

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
