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

using System.Collections.Generic;

using WinCopies.Collections.Generic;
using WinCopies.GUI.IO;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;
using WinCopies.IPCService.Extensions;

namespace WinCopies
{
    public class PathCollectionUpdater : IUpdater
    {
        public static IMainWindowModel Instance { get; internal set; }

        public unsafe int Run(string[] args)
        {
            if (args != null)
            {
                IQueue<string> queue = new PathQueue();

                IPCService.Extensions.SingleInstanceApp.Initialize(new Dictionary<string, Action>(1) { { Keys.Path, (string[] args, ref ArrayBuilder<string> arrayBuilder, in int* i) => Loader.LoadPathParameters(queue, i, args) } }, App.OnArgumentError, args);

                App.Run(queue);
            }

            return 0;
        }
    }

    public interface IMainWindowModel
    {
        System.Collections.Generic.ICollection<IExplorerControlViewModel> Paths { get; }
    }

    public class MainWindowModel : IMainWindowModel
    {
        public System.Collections.Generic.ICollection<IExplorerControlViewModel> Paths { get; }

        private MainWindowModel(in System.Collections.Generic.ICollection<IExplorerControlViewModel> paths) => Paths = paths;

        public static void Init(in System.Collections.Generic.ICollection<IExplorerControlViewModel> paths) => PathCollectionUpdater.Instance ??= new MainWindowModel(paths);
    }

    public class MainWindowPathCollectionViewModel : BrowsableObjectInfoCollectionViewModel
    {
        public MainWindowPathCollectionViewModel() { /* Left empty. */ }

        public MainWindowPathCollectionViewModel(in IEnumerable<IExplorerControlViewModel> items) : base(items) { /* Left empty. */ }

        private void Item_CustomProcessParametersGenerated(object sender, CustomProcessParametersGeneratedEventArgs e) => IPCService.Extensions.SingleInstanceApp.StartInstance(App.FileName, App.GetProcessParameters(e.ProcessParameters));

        protected override void OnPathAdded(IExplorerControlViewModel path)
        {
            base.OnPathAdded(path);

            path.CustomProcessParametersGenerated += Item_CustomProcessParametersGenerated;
        }

        protected override void OnPathRemoved(IExplorerControlViewModel path)
        {
            path.CustomProcessParametersGenerated -= Item_CustomProcessParametersGenerated;

            base.OnPathRemoved(path);
        }

        public override IBrowsableObjectInfo GetDefaultBrowsableObjectInfo() => new BrowsableObjectInfoStartPage();
    }

    public class MainWindowViewModel : BrowsableObjectInfoWindowViewModel, IMainWindowModel
    {
        System.Collections.Generic.ICollection<IExplorerControlViewModel> IMainWindowModel.Paths => Paths.Paths;

        public MainWindowViewModel() : base(new MainWindowPathCollectionViewModel()) => Paths.IsCheckBoxVisible = true;

        public MainWindowViewModel(in IBrowsableObjectInfoCollectionViewModel paths) : base(paths) { /* Left empty. */ }
    }
}
