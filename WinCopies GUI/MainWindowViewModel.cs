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

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.GUI.IO.Process;
using WinCopies.IO.Process;
using WinCopies.Util.Data;

namespace WinCopies
{
    public class PathCollectionUpdater : IUpdater
    {
        public static IMainWindowModel Instance { get; internal set; }

        public int Run(string[] args)
        {
            if (args != null)
            {
                IQueue<string> queue = new Collections.DotNetFix.Generic.Queue<string>();

                App.InitQueues(args, queue, null);

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

    public class MainWindowViewModel : ViewModelBase
    {
        public static IProcessPathCollectionFactory DefaultProcessPathCollectionFactory { get; } = new ProcessPathCollectionFactory();

        public MenuViewModel Menu { get; }

        public System.Collections.ObjectModel.ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> Paths { get; } = new System.Collections.ObjectModel.ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>();

        private ExplorerControlBrowsableObjectInfoViewModel _selectedItem;

        public ExplorerControlBrowsableObjectInfoViewModel SelectedItem { get => _selectedItem; set { UpdateValue(ref _selectedItem, value, nameof(SelectedItem)); } }

        public MainWindowViewModel()
        {
            Menu = new MenuViewModel();

            Paths.CollectionChanged += Paths_CollectionChanged;

            MainWindowModel.Init(Paths);
        }

        private void Paths_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                IExplorerControlBrowsableObjectInfoViewModel item;

                foreach (object _item in e.NewItems)
                {
                    (item = ((IExplorerControlBrowsableObjectInfoViewModel)_item)).IsCheckBoxVisible = true;

                    item.CustomProcessParametersGeneratedEventHandler += Item_CustomProcessParametersGeneratedEventHandler;
                }
            }

            if (e.OldItems != null)

                foreach (object _item in e.OldItems)

                    ((IExplorerControlBrowsableObjectInfoViewModel)_item).CustomProcessParametersGeneratedEventHandler -= Item_CustomProcessParametersGeneratedEventHandler;
        }

        private void Item_CustomProcessParametersGeneratedEventHandler(object sender, GUI.IO.CustomProcessParametersGeneratedEventArgs e) => App.StartInstance(App.GetProcessParameters(e.ProcessParameters));
    }
}
