using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WinCopies.Util;
using WinCopies.Util.Data;
using static WinCopies.App;

namespace WinCopies
{
    public class MenuViewModel : ViewModelBase
    {
        private MenuItemViewModel _selectedItem;

        private ObservableCollection<MenuItemViewModel> _menuItems;

        internal ObservableCollection<MenuItemViewModel> _MenuItems => _menuItems ??= new ObservableCollection<MenuItemViewModel>();

        private ReadOnlyObservableCollection<MenuItemViewModel> _menuItemsReadOnly;

        public ReadOnlyObservableCollection<MenuItemViewModel> MenuItems => _menuItemsReadOnly ??= new ReadOnlyObservableCollection<MenuItemViewModel>(_MenuItems);

        public MenuItemViewModel SelectedItem { get => _selectedItem; internal set { 
                
                _selectedItem = value.IsSelected ? value:null; OnPropertyChanged(nameof(SelectedItem));  
            
            } }
    }

    public class MenuItemViewModel : ViewModelBase
    {
        private readonly MenuViewModel _parentMenu;

        public string ResourceId { get; }

        public MenuItemViewModel ParentMenuItem { get; }

        private bool _isSelected;

        public bool IsSelected { get => _isSelected; set { _isSelected = value;
                
                _parentMenu.SelectedItem = this; OnPropertyChanged(nameof(IsSelected)); } }

        private ObservableCollection<MenuItemViewModel> _menuItems;

        private ObservableCollection<MenuItemViewModel> _MenuItems => _menuItems ??= new ObservableCollection<MenuItemViewModel>();

        private ReadOnlyObservableCollection<MenuItemViewModel> _menuItemReadOnly;

        public ReadOnlyObservableCollection<MenuItemViewModel> MenuItems => _menuItemReadOnly ??= new ReadOnlyObservableCollection<MenuItemViewModel>(_MenuItems);

        public string Header { get; }

        public RoutedCommand Command { get; }

        public object CommandParameter { get; }

        public ImageSource Icon { get; }

        private string _statusBarLabel;

        public string StatusBarLabel => _statusBarLabel ??= (string)ResourceProperties.First(p => p.Name == $"{ResourceId}StatusBarLabel").GetValue(null);

        private MenuItemViewModel(in ObservableCollection<MenuItemViewModel> parentItemsCollection, in string header, in string resourceId, in RoutedCommand command, Func commandParameter, ImageSource iconImageSource)
        {
            parentItemsCollection.Add(this);

            Header = header;

            ResourceId = resourceId;

            Command = command;

            CommandParameter = commandParameter;

            Icon = iconImageSource;
        }

        public MenuItemViewModel(in MenuViewModel parentMenu, in string header, in string resourceId, in RoutedCommand command, Func commandParameter, ImageSource iconImageSource) : this(parentMenu._MenuItems, header, resourceId, command, commandParameter, iconImageSource) => _parentMenu = parentMenu;

        public MenuItemViewModel(in MenuItemViewModel parentMenuItem, in string header, in string resourceId, in RoutedCommand command, Func commandParameter, ImageSource iconImageSource) : this(parentMenuItem._MenuItems, header, resourceId, command, commandParameter, iconImageSource)
        {
            _parentMenu = parentMenuItem._parentMenu;

            ParentMenuItem = parentMenuItem;
        }
    }
}
