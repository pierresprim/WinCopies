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
using System.Collections.Specialized;
using WinCopies.Collections.Generic;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.Shell;
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

                IPCService.Extensions.SingleInstanceApp.Initialize(new Dictionary<string, Action>(1) { { Keys.Path, (string[] args, ref ArrayBuilder<string> arrayBuilder, in int* i) => Loader.LoadPathParameters(queue, i, args) } }, args);

                App.Run(queue);
            }

            return 0;
        }
    }

    public interface IMainWindowModel
    {
        ICollection<IExplorerControlBrowsableObjectInfoViewModel> Paths { get; }
    }

    public class MainWindowModel : IMainWindowModel
    {
        public ICollection<IExplorerControlBrowsableObjectInfoViewModel> Paths { get; }

        private MainWindowModel(in ICollection<IExplorerControlBrowsableObjectInfoViewModel> paths) => Paths = paths;

        public static void Init(in ICollection<IExplorerControlBrowsableObjectInfoViewModel> paths) => PathCollectionUpdater.Instance ??= new MainWindowModel(paths);
    }

    public class MainWindowPathCollectionViewModel : BrowsableObjectInfoCollectionViewModel
    {
        private void Item_CustomProcessParametersGeneratedEventHandler(object sender, GUI.IO.CustomProcessParametersGeneratedEventArgs e) => IPCService.Extensions.SingleInstanceApp.StartInstance(App.FileName, App.GetProcessParameters(e.ProcessParameters));

        protected override void OnPathAdded(IExplorerControlBrowsableObjectInfoViewModel path)
        {
            base.OnPathAdded(path);

            path.CustomProcessParametersGeneratedEventHandler += Item_CustomProcessParametersGeneratedEventHandler;
        }

        protected override void OnPathCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnPathCollectionChanged(e);

            if (e.OldItems != null)

                foreach (object _item in e.OldItems)

                    ((IExplorerControlBrowsableObjectInfoViewModel)_item).CustomProcessParametersGeneratedEventHandler -= Item_CustomProcessParametersGeneratedEventHandler;
        }
    }

    public class MainWindowViewModel : BrowsableObjectInfoWindowViewModel, IMainWindowModel
    {
        ICollection<IExplorerControlBrowsableObjectInfoViewModel> IMainWindowModel.Paths => Paths.Paths;

        public MainWindowViewModel() : base(new MainWindowPathCollectionViewModel()) => Paths.IsCheckBoxVisible = true;
    }
}
