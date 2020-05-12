using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WinCopies.GUI.IO;
using WinCopies.IO;
using WinCopies.Util.Data;

namespace WinCopies
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MenuViewModel Menu { get; }

        public static ExplorerControlBrowsableObjectInfoViewModel GetNewExplorerControlBrowsableObjectInfo() => new ExplorerControlBrowsableObjectInfoViewModel(new BrowsableObjectInfoViewModel(ShellObjectInfo.From(ShellObject.FromParsingName(KnownFolders.Desktop.ParsingName)))) { TreeViewItems = new ObservableCollection<IBrowsableObjectInfoViewModel>() { new BrowsableObjectInfoViewModel(ShellObjectInfo.From(ShellObject.FromParsingName(KnownFolders.UsersLibraries.ParsingName))), new BrowsableObjectInfoViewModel(ShellObjectInfo.From(ShellObject.FromParsingName(KnownFolders.Desktop.ParsingName))), new BrowsableObjectInfoViewModel(ShellObjectInfo.From(ShellObject.FromParsingName(KnownFolders.Computer.ParsingName))), new BrowsableObjectInfoViewModel(ShellObjectInfo.From(ShellObject.FromParsingName(KnownFolders.RecycleBin.ParsingName))) } };

        public ObservableCollection<ExplorerControlBrowsableObjectInfoViewModel> Paths { get; } = new ObservableCollection<ExplorerControlBrowsableObjectInfoViewModel>() { GetNewExplorerControlBrowsableObjectInfo() };

        private ExplorerControlBrowsableObjectInfoViewModel _selectedItem;

        public ExplorerControlBrowsableObjectInfoViewModel SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); } } 

        public MainWindowViewModel() => Menu = new MenuViewModel();
    }
}
