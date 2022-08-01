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

using static WinCopies.Setup.Installer;
using static WinCopies.Setup.Properties.Resources;

using InstallerPageData = WinCopies.Installer.InstallerPageData;
using LocalResources = WinCopies.Setup.Properties.Resources;
#endregion Usings

namespace WinCopies.Setup
{
    public class Installer : WinCopies.Installer.Installer
    {
        public sealed override string ProgramName => "WinCopies";

        public override bool Is32Bit => true;

        public override bool RequiresRestart => false;

        protected override IStartPage GetStartPage() => new StartPage(this);

        public static ImageSource GetHorizontalImageSource() => horizontal.ToImageSource();

        public static ImageSource GetVerticalImageSource() => vertical.ToImageSource();
    }

    public class StartPage : WinCopies.Installer.StartPage
    {
        public override ImageSource ImageSource { get; } = GetVerticalImageSource();

        internal StartPage(in WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }

        protected override ILicenseAgreementPage GetNextPage() => new LicenseAgreementPage(this);
    }

    public class LicenseAgreementPage : WinCopies.Installer.LicenseAgreementPage
    {
        private class LicenseAgreementData : InstallerPageData, ILicenseAgreementData
        {
            public DataFormat DataFormat => DataFormats.GetDataFormat(DataFormats.Rtf);

            public LicenseAgreementData(in WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }

            public System.IO.Stream GetText() => System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WinCopies.Setup.Resources.gpl-3.0.rtf");
        }

        public override ImageSource ImageSource { get; } = GetHorizontalImageSource();

        public override Icon Icon => LocalResources.WinCopies;

        protected internal LicenseAgreementPage(in StartPage startPage) : base(startPage) { /* Left empty. */ }

        protected override IUserGroupPage GetNextPage() => new UserGroupPage(this);

        protected override ILicenseAgreementData GetData() => new LicenseAgreementData(Installer);
    }

    public class UserGroupPage : WinCopies.Installer.UserGroupPage
    {
        public override ImageSource ImageSource { get; } = GetHorizontalImageSource();

        public override Icon Icon => LocalResources.WinCopies;

        internal UserGroupPage(in LicenseAgreementPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IDestinationPage GetNextPage() => new DestinationPage(this);
    }

    public class DestinationPage : WinCopies.Installer.DestinationPage
    {
        public override ImageSource ImageSource { get; } = GetHorizontalImageSource();

        public override Icon Icon => LocalResources.WinCopies;

        protected override IOptionsPage GetNextPage() => new OptionsPage(this);

        internal DestinationPage(in UserGroupPage previousPage) : base(previousPage) { /* Left empty. */ }
    }

    public class OptionsPage : WinCopies.Installer.OptionsPage
    {
        public override ImageSource ImageSource { get; } = GetHorizontalImageSource();

        public override Icon Icon => LocalResources.WinCopies;

        internal OptionsPage(in WinCopies.Installer.DestinationPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IProcessPage GetNextPage() => new ProcessPage(Installer);

        protected override IOptionsData GetData() => new DefaultOptionsData(Installer);
    }

    public class ProcessPage : WinCopies.Installer.ProcessPage
    {
        protected new class ProcessData : DefaultProcessData
        {
            protected const string RELATIVE_PATH = $"{nameof(WinCopies)}.{nameof(Setup)}.{nameof(Resources)}.Files.";

            protected override string RelativePath => RELATIVE_PATH;

            protected override Type RelativePathResourcesType => null;

            protected override Predicate<KeyValuePair<string, System.IO.Stream>> Predicate => item => item.Key.EndsWith("WinCopies.exe");

            public ProcessData(in WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }
        }

        public override Icon Icon => LocalResources.WinCopies;

        public override ImageSource ImageSource { get; } = GetHorizontalImageSource();

        public ProcessPage(in WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }

        protected override IProcessData GetData() => new ProcessData(Installer);
        protected override IEndPage GetNextPage() => new EndPage(Installer);
    }

    public class EndPage : WinCopies.Installer.EndPage
    {
        public override ImageSource ImageSource { get; } = GetVerticalImageSource();

        public EndPage(WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }
    }

    public partial class MainWindow : InstallerWindow
    {
        public MainWindow() : base(new InstallerViewModel(new Installer())) { /* Left empty. */ }
    }
}
