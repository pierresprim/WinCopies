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

using System.Linq;
using System.Reactive.Linq;

using WinCopies.Util.Data;

namespace WinCopies
{
    public class MenuViewModel : ViewModelBase
    {
        private MenuItemViewModel _selectedItem;

        public MenuItemViewModel SelectedItem { get => _selectedItem; internal set { _selectedItem = value.IsSelected ? value : null; OnPropertyChanged(nameof(SelectedItem)); } }
    }

    public class MenuItemViewModel : ViewModelBase
    {
        private readonly MenuViewModel _parentMenu;

        public string ResourceId { get; set; }

        public MenuItemViewModel ParentMenuItem { get; }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected; set
            {
                _isSelected = value;

                _parentMenu.SelectedItem = this; OnPropertyChanged(nameof(IsSelected));
            }
        }

        //private ObservableCollection<MenuItemViewModel> _menuItems;

        //private ObservableCollection<MenuItemViewModel> _Items => _menuItems ??= new ObservableCollection<MenuItemViewModel>();

        //private ReadOnlyObservableCollection<MenuItemViewModel> _menuItemReadOnly;

        //public ReadOnlyObservableCollection<MenuItemViewModel> Items => _menuItemReadOnly ??= new ReadOnlyObservableCollection<MenuItemViewModel>(_Items);

        //public string Header { get; }

        //public RoutedCommand Command { get; }

        //public object CommandParameter { get; }

        //public ImageSource Icon { get; }

        private string _statusBarLabel;

        public string StatusBarLabel => _statusBarLabel ??= (string)typeof(Properties.Resources).GetProperties().FirstOrDefault(p => p.Name == $"{ResourceId}StatusBarLabel")?.GetValue(null);

        //private MenuItemViewModel(in string header, in string resourceId, in RoutedCommand command, Func commandParameter, ImageSource iconImageSource)
        //{
            //    parentItemsCollection.Add(this);

            //Header = header;

            //ResourceId = resourceId;

            //Command = command;

            //CommandParameter = commandParameter;

            //Icon = iconImageSource;
        //}

        public MenuItemViewModel(in MenuViewModel parentMenu/*, in string header, in string resourceId, in RoutedCommand command, Func commandParameter, ImageSource iconImageSource*/) /*: this(parentMenu._Items, header, resourceId, command, commandParameter, iconImageSource)*/ => _parentMenu = parentMenu;

        public MenuItemViewModel(in MenuItemViewModel parentMenuItem/*, in string header, in string resourceId, in RoutedCommand command, Func commandParameter, ImageSource iconImageSource*/) //: this(parentMenuItem._Items, header, resourceId, command, commandParameter, iconImageSource)
        {
            _parentMenu = parentMenuItem._parentMenu;

            ParentMenuItem = parentMenuItem;
        }
    }
}
