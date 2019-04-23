using System.ComponentModel;

namespace WinCopiesGUIWizard
{
    public class TreeViewItem : INotifyPropertyChanged
    {

        public string Header { get; } = null;

        private bool isSelected = false;

        public bool IsSelected { get => isSelected; set { isSelected = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected))); } }

        private bool isExpanded = false;

        public bool IsExpanded { get => isExpanded; set { isExpanded = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded))); } }

        public string PageUri { get; } = null;

        public TreeViewItem[] Items { get; } = null;

        public TreeViewItem this[int index] { get => Items[index]; set => Items[index] = value; }

        public TreeViewItem Parent { get; private set; } = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public TreeViewItem(string header, string pageUri, TreeViewItem[] items)

        {

            this.Header = header;

            this.PageUri = pageUri;

            this.Items = items;

            if (items != null) 

                foreach (TreeViewItem item in items)

                    item.Parent = this;

        }
    }
}
