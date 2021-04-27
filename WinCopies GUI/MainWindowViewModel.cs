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

using Microsoft.WindowsAPICodePack.Shell;
using System.Collections.ObjectModel;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;
using WinCopies.Util.Data;

namespace WinCopies
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MenuViewModel Menu { get; }

        public ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel> Paths { get; } = new ObservableCollection<IExplorerControlBrowsableObjectInfoViewModel>() { App.GetDefaultExplorerControlBrowsableObjectInfoViewModel() };

        private ExplorerControlBrowsableObjectInfoViewModel _selectedItem;

        public ExplorerControlBrowsableObjectInfoViewModel SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); } } 

        public MainWindowViewModel() => Menu = new MenuViewModel();
    }
}
