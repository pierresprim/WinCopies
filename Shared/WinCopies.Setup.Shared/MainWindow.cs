#region Usings
#region System
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
#endregion System

#region WinCopies
using WinCopies.Desktop;
using WinCopies.Installer;
using WinCopies.Installer.GUI;
#endregion WinCopies
#endregion Usings

namespace WinCopies.Setup
{
    public class Installer : Common.Installer
    {
        protected override IStartPage GetStartPage() => new StartPage(this);
    }

    public class StartPage : Common.StartPage
    {
        internal StartPage(in Installer installer) : base(installer) { /* Left empty. */ }

        protected override ILicenseAgreementPage? GetNextPage() => new LicenseAgreementPage(this);
    }

    public class LicenseAgreementPage : Common.LicenseAgreementPage
    {
        internal LicenseAgreementPage(in StartPage startPage) : base(startPage) { /* Left empty. */ }

        protected override IUserGroupPage? GetNextPage() => new UserGroupPage(this);
    }

    public class UserGroupPage : Common.UserGroupPage
    {
        internal UserGroupPage(in LicenseAgreementPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IDestinationPage? GetNextPage() => new DestinationPage(this);
    }

    public class DestinationPage : Common.DestinationPage
    {
        internal DestinationPage(in UserGroupPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IOptionsPage? GetNextPage() => new OptionsPage(this);
    }

    public class OptionsPage : Common.OptionsPage
    {
        internal OptionsPage(in DestinationPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IProcessPage? GetNextPage() => new ProcessPage(Installer);
    }

    public class ProcessPage : Common.ProcessPage
    {
        protected new class ProcessData : DefaultEmbeddedProcessData
        {
            protected const string RELATIVE_PATH = $"{nameof(WinCopies)}.{nameof(Setup)}.{nameof(Resources)}.Files.";

            protected override string RelativePath => RELATIVE_PATH;

            protected override Type RelativePathResourcesType => null;

            protected override Predicate<string> Predicate => item => item.EndsWith($"{nameof(WinCopies)}.exe");

            public ProcessData(in WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }
        }

        internal ProcessPage(in WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }

        protected override IProcessData GetData() => new ProcessData(Installer);
    }

    public partial class MainWindow : InstallerWindow
    {
        public MainWindow() : base(new InstallerViewModel(new Installer())) { /* Left empty. */ }
    }
}
