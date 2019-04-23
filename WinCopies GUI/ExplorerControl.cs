// public bool CanMoveToParentFolder { get => _LoadFolder.Path != null && _LoadFolder.Path.ShellObject.Name != Microsoft.WindowsAPICodePack.Shell.KnownFolders.Computer.LocalizedName; }

// public WinCopies.IO.SpecialFolders SpecialFolder { get => _SpecialFolder; private set { SetProperty("SpecialFolder", "_SpecialFolder", value); } }

// private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
// {
    // Process.Start((string)e.Argument);
// }

//public void Bidule()
//{



//}

//        public virtual void OnPropertyChanged(string propertyName, string fieldName, object previousValue, object newValue)
//        {

//            CommonHelper.OnPropertyChangedHelper(this, propertyName, fieldName, previousValue, newValue);

//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, previousValue, newValue));

//        }

//        public virtual void OnPropertyChanged(string propertyName)
//        {
//            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));

//            // throw new NotImplementedException();
//        }

//        public void OnPropertyChangedReadOnly(string propertyName, object previousValue, object newValue)
//        {
//#if DEBUG 
//            CommonHelper.OnPropertyChangedReadOnlyHelper(this, propertyName, previousValue, newValue);
//#endif

//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, previousValue, newValue));
//        }

//public void OnPropertyChanged(string propertyName, object previousValue, object newValue)
//{
//    throw new NotImplementedException();
//}

// internal void RaisePropertyChangedEvent(string propertyName)

// {

// PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));

// }

//private void SetProperty(string propertyName, string fieldName, object newValue)

//{

//    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
//                 BindingFlags.Static | BindingFlags.Instance |
//                 BindingFlags.DeclaredOnly;
//    this.GetType().GetField(fieldName, flags).SetValue(this, newValue);

//#if DEBUG

//    if (propertyName == nameof(SelectedItems) && SelectedItems != null)

//        Console.WriteLine("SelectedItems.GetType().ToString(): " + SelectedItems.GetType().ToString());

//#endif

//    RaisePropertyChangedEvent(propertyName);

//}

//public void Reload() { reload(LoadFolder.Path, false); }

//public void reload(WinCopies.IO.ShellObjectInfo path, bool addPathToHistory)
//{

//    fsw.EnableRaisingEvents = false;

//    if (LoadFolder != null && LoadFolder.Path != null && LoadFolder.Path.Path != null)
//    {
//        Console.WriteLine("Add path to history: " + (addPathToHistory && path.Path != LoadFolder.Path.Path).ToString());
//        Console.WriteLine("Add path to history: " + addPathToHistory.ToString() + " " + (path.Path != LoadFolder.Path.Path).ToString());

//        if (addPathToHistory && path.Path != LoadFolder.Path.Path)
//        {

//            History.Add(new WinCopies.GUI.Explorer.HistoryItemData(Header, path, new WinCopies.GUI.Explorer.ScrollViewerOffset(PART_ListView.ScrollHost.HorizontalOffset, PART_ListView.ScrollHost.VerticalOffset), new List<object>(SelectedItems))); HistorySelectedIndex = History.Count - 1;

//        }

//    }



//    #region Comments

//    //if (path.ShellObject is IKnownFolder)

//    //{
//    //    Console.WriteLine("((IKnownFolder)path.ShellObject == KnownFolders.Desktop).ToString(): " + ((IKnownFolder)path.ShellObject == KnownFolders.Desktop).ToString());
//    //    Console.WriteLine(path.ShellObject.Name);
//    //    if ((IKnownFolder)path.ShellObject == KnownFolders.Libraries)

//    //    {

//    //        SpecialFolder = WinCopies.IO.SpecialFolders.Libraries;

//    //        CurrentPath = path.ShellObject.GetDisplayName(DisplayNameType.Default);

//    //    }

//    //    else if ((IKnownFolder)path.ShellObject == KnownFolders.Computer)

//    //    {

//    //        SpecialFolder = WinCopies.IO.SpecialFolders.Computer;

//    //        CurrentPath = path.ShellObject.GetDisplayName(DisplayNameType.Default);

//    //    }

//    //    else if ((IKnownFolder)path.ShellObject == KnownFolders.Desktop)

//    //    {

//    //        SpecialFolder = WinCopies.IO.SpecialFolders.Desktop;

//    //        CurrentPath = path.ShellObject.GetDisplayName(DisplayNameType.Default);

//    //    }

//    //    else

//    //    {

//    //        SpecialFolder = WinCopies.IO.SpecialFolders.OtherFolder; CurrentPath = path.Path;

//    //    }

//    //}

//    //else { SpecialFolder = WinCopies.IO.SpecialFolders.OtherFolder; CurrentPath = path.Path; }

//    #endregion



//    if (path.SpecialFolder == WinCopies.IO.SpecialFolders.OtherFolderOrFile)

//    {

//        CurrentPath = path.Path;

//        Header = System.IO.Path.GetFileName(path.Path);

//    }

//    else

//    {

//        CurrentPath = path.ShellObject.GetDisplayName(DisplayNameType.Default);

//        Header = path.ShellObject.GetDisplayName(DisplayNameType.Default);

//    }



//    if (path.Path != null && path.FileType == WinCopies.IO.FileTypes.Folder || path.FileType == WinCopies.IO.FileTypes.Drive)

//    {

//        fsw.Path = path.Path;

//        fsw.EnableRaisingEvents = true;

//    }

//    else

//    {

//        fsw.EnableRaisingEvents = false;

//        fsw.Path = null;

//    }





//    LoadFolder.getitems(path);

//    /*_CurrentPath = path;*/

//    /*PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));*/

//    /*PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPath"));*/
//}

// private delegate void d(FileSystemEventArgs e);

//public void Open(WinCopies.IO.ShellObjectInfo path)

//{

//    if (path.ShellObject is ShellFolder) reload(path, true);

//    else if (path.ShellObject is ShellFile) { bgWorker.RunWorkerAsync(path.Path); }

//    else if (path.ShellObject is ShellLink) Open(WinCopies.IO.Util.GetNormalizedOSPath(((ShellLink)path.ShellObject).TargetLocation));

//    // else if (path.ShellObject is ShellLibrary) reload(path);

//}

//public void Open(IList<WinCopies.IO.ShellObjectInfo> paths)

//{

//    if (paths.Count == 1) { Open(paths[0]); return; }

//    var previous_Type = paths[0].GetType();

//    for (int i = 1; i <= paths.Count - 1; i++)

//    {

//        if (paths[i].GetType() != previous_Type || paths[i].ShellObject is ShellLink) return;

//        else previous_Type = paths[i].GetType();

//    }

//    if (paths[0].ShellObject is ShellFolder)

//    {

//        var isLibrary = false;

//        reload(paths[0], true);

//        for (int i = 1; i <= paths.Count - 1; i++)

//        {

//            var newTabItem = new TabItem();

//            ((MainWindow)((App)Application.Current).MainWindow).Items.Add(newTabItem);

//            newTabItem.WinCopies.GUI.Explorer.ExplorerControl.reload(paths[i], true);

//        }

//    }

//    else if (paths[0].ShellObject is ShellFile)

//    {

//        bgWorker.DoWork += (object sender, DoWorkEventArgs e) =>
//        {

//            foreach (WinCopies.IO.ShellObjectInfo path in paths)

//            {

//                Process.Start(path.Path);

//            }

//        };

//        bgWorker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => { bgWorker.Dispose(); };

//        bgWorker.RunWorkerAsync();

//    }

//}

//public void Navigate(int index)

//{

//    var historyEntry = History[index];

//    Open(historyEntry.Path);

//    PART_ListView.ScrollHost.ScrollToHorizontalOffset(historyEntry.WinCopies.GUI.Explorer.ScrollViewerOffset.HorizontalOffset);

//    PART_ListView.ScrollHost.ScrollToVerticalOffset(historyEntry.WinCopies.GUI.Explorer.ScrollViewerOffset.VerticalOffset);

//    foreach (object o in historyEntry.SelectedItems)

//    {

//        if (o.GetType() == typeof(ShellObjectInfo))

//        {

//            var o_ShellObjectInfo = (ShellObjectInfo)o;

//            foreach (ShellObjectInfo shellObjectInfo in LoadFolder.Paths)

//                shellObjectInfo.IsSelected = o_ShellObjectInfo.Path == shellObjectInfo.Path;

//        }

//    }

//    HistorySelectedIndex = index;

//}
//}
//}
