//using Microsoft.WindowsAPICodePack.Shell;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Collections.Specialized;
//using WinCopies.GUI.Explorer;
//using WinCopies.IO.FileProcesses;

//namespace WinCopiesProcessesManager
//{
//    public class FileSystemInfoCollection : IReadOnlyList<FileSystemInfo>, INotifyCollectionChanged
//    {

//        public class FileSystemInfoFull

//        {

//            private FileSystemInfoCollection _fileSystemInfos = null;

//            private int _index = -1;

//            public ShellObjectInfo FileToCopy { get; }

//            public FileSystemInfoFull()

//            {

//                FileToCopy = new ShellObjectInfo(new WinCopies.IO.ShellObjectInfo(ShellObject.FromParsingName(    _fileSystemInfos._innerCollection[_index].FileSystemInfoProperties.FullName;

//            }    

//        } 

//        private ReadOnlyObservableCollection<FileSystemInfo> _innerCollection = null;

//        private int _startIndex = -1;

//        private bool _browsed = false;

//        public FileSystemInfo this[int index] => 

//        public int Count { get; private set; } = 0;

//        public event NotifyCollectionChangedEventHandler CollectionChanged;

//        public FileSystemInfoCollection(ReadOnlyObservableCollection<FileSystemInfo> fileSystemInfos)

//        {

//            _innerCollection = fileSystemInfos;

//            ((INotifyCollectionChanged)_innerCollection).CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>

//                {

//                    switch (e.Action)

//                    {

//                        case NotifyCollectionChangedAction.Remove:

//                            Count -= e.OldItems.Count;

//                            break;

//                    }

//                };

//            Browse();

//        }

//        private void Browse()

//        {

//            bool foundStartIndex = false;

//            bool ok = false;

//            for (int i = 0; i < _innerCollection.Count && !ok; i++)

//                if (_innerCollection[i].Exception == Exceptions.FileAlreadyExists)

//                {

//                    _startIndex = i;

//                    foundStartIndex = true;

//                }

//                else if (foundStartIndex)

//                {

//                    Count = i - _startIndex;

//                    ok = true;

//                }

//            _browsed = true;

//        }

//        public IEnumerator<FileSystemInfo> GetEnumerator() => throw new NotImplementedException();

//        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
//    }
//}
