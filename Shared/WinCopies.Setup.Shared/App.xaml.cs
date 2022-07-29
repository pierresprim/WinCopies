using System.Windows;

namespace WinCopies.Setup
{
    public partial class App : Desktop.Application
    {
        protected override Window GetWindow() => new MainWindow();
    }
}
