using Microsoft.WindowsAPICodePack.Shell;
using System.ComponentModel;

namespace WinCopiesProcessesManager
{
    public class FileSystemInfo : INotifyPropertyChanged
    {

        private ShellObjectInfo _fileToCopy = null;

        public ShellObjectInfo FileToCopy
        {
            get => _fileToCopy; set

            {

                _fileToCopy = value;

                value._fileSystemInfo = this;

            }

        }

        private ShellObjectInfo _fileThatAlreadyExists = null;

        public ShellObjectInfo FileThatAlreadyExists
        {

            get => _fileThatAlreadyExists; set

            {

                _fileThatAlreadyExists = value;

                value._fileSystemInfo = this;

            }

        }

        private bool _computingContent = false;

        public bool ComputingContent

        {

            get => _computingContent;

            internal set

            {

                if (value == _computingContent) return;

                _computingContent = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComputingContent)));

            }

        }

        private bool? _filesHaveSameContent = null;

        public bool? FilesHaveSameContent

        {

            get => _filesHaveSameContent;

            internal set { if (value == _filesHaveSameContent) return; _filesHaveSameContent = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilesHaveSameContent))); }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public class ShellObjectInfo : WinCopies.GUI.Explorer.ShellObjectInfo

        {

            public FileSystemInfo _fileSystemInfo = null;

            public FileSystemInfo FileSystemInfo => _fileSystemInfo;

            public ShellObjectInfo(ShellObject shellObject, string path) : base(shellObject, path)
            {



            }

        }

    }
}
