using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WinCopies.GUI.Explorer;
using WinCopies.IO.FileProcesses;
using BackgroundWorker = WinCopies.Util.BackgroundWorker;

namespace WinCopiesProcessesManager
{
    /// <summary>
    /// Logique d'interaction pour FileCheck.xaml
    /// </summary>
    public partial class FileCheck : WinCopies.GUI.Windows.Dialogs.DialogWindow
    {

        private CopyProcessInfo _copyProcessInfo = null;

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<FileSystemInfo>), typeof(FileCheck), new PropertyMetadata(null));

        public ObservableCollection<FileSystemInfo> Items { get => (ObservableCollection<FileSystemInfo>)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

        public static readonly DependencyProperty HowToRetryProperty = DependencyProperty.Register(nameof(HowToRetry), typeof(HowToRetry), typeof(FileCheck), new PropertyMetadata(HowToRetry.CheckFiles));

        public HowToRetry HowToRetry { get => (HowToRetry)GetValue(HowToRetryProperty); set => SetValue(HowToRetryProperty, value); }

        public static readonly DependencyProperty ShowAllProperty = DependencyProperty.Register(nameof(ShowAll), typeof(bool), typeof(FileCheck), new PropertyMetadata(true));

        public bool ShowAll { get => (bool)GetValue(ShowAllProperty); set => SetValue(ShowAllProperty, value); }

        private bool _isAutomaticUpdate = false;

        public static readonly DependencyProperty ApplyToAllProperty = DependencyProperty.Register(nameof(ApplyToAll), typeof(bool?), typeof(FileCheck), new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

        {

            if (((FileCheck)d)._isAutomaticUpdate) return;

            if (!((bool?)e.NewValue).HasValue)

                throw new ArgumentException("The ApplyToAll property cannot be set with a null value. The ApplyToAll property is set to null only when some, but not all, items are selected.");

            ((FileCheck)d)._isAutomaticUpdate = true;

            foreach (var item in (ObservableCollection<FileSystemInfo>)d.GetValue(ItemsProperty))

            {

                item.FileToCopy.IsSelected = ((bool?)e.NewValue).Value;

                item.FileThatAlreadyExists.IsSelected = ((bool?)e.NewValue).Value;

            }

            ((FileCheck)d)._isAutomaticUpdate = false;

        }));

        public bool? ApplyToAll { get => (bool?)GetValue(ApplyToAllProperty); set => SetValue(ApplyToAllProperty, value); }

        public FileCheck(CopyProcessInfo copyProcessInfo)

        {

            InitializeComponent();

            _copyProcessInfo = copyProcessInfo;

            bool foundStartIndex = false;

            bool ok = false;

            string path = null;

            var items = new ObservableCollection<FileSystemInfo>();

            Items = items;

            for (int i = 0; i < copyProcessInfo.Exceptions.Count && !ok; i++)

                if (copyProcessInfo.Exceptions[i].Exception == Exceptions.FileAlreadyExists)

                {

                    path = copyProcessInfo.GetCopyPath(copyProcessInfo.Exceptions[i].FileSystemInfoProperties.FullName, false);

                    ShellObject shellObject = ShellObject.FromParsingName(copyProcessInfo.Exceptions[i].FileSystemInfoProperties.FullName);

                    shellObject.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;

                    shellObject = ShellObject.FromParsingName(path);

                    shellObject.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;

                    Items.Add(new FileSystemInfo()
                    {

                        FileToCopy = new FileSystemInfo.ShellObjectInfo(shellObject, copyProcessInfo.Exceptions[i].FileSystemInfoProperties.FullName),

                        FileThatAlreadyExists = new FileSystemInfo.ShellObjectInfo(shellObject, path)

                    });

                    items[i].FileToCopy.PropertyChanged += Item_PropertyChanged;

                    items[i].FileThatAlreadyExists.PropertyChanged += Item_PropertyChanged;

                    foundStartIndex = true;

                }

                else if (foundStartIndex)

                    ok = true;

            DataContext = this;

            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (object sender, DoWorkEventArgs e) =>
            {

                SHA512 sha512 = null;

                string hex_value = null;

                string computeHash(FileSystemInfo.ShellObjectInfo shellObject)

                {

                    sha512 = SHA512.Create();

                    hex_value = null;

                    FileStream fileStream = File.OpenRead(shellObject.Path);

                    sha512.ComputeHash(fileStream);

                    fileStream.Close();

                    fileStream.Dispose();

                    foreach (byte b in sha512.Hash)

                        hex_value += b.ToString("X2");

                    return hex_value;

                }

                foreach (FileSystemInfo _path in items)

                {

                    if (backgroundWorker.CancellationPending) return;

                    _path.ComputingContent = true;

                    _path.FilesHaveSameContent = computeHash(_path.FileToCopy) == computeHash(_path.FileThatAlreadyExists);

                    _path.ComputingContent = false;

                }

            };

            backgroundWorker.RunWorkerAsync();

        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)

        {

            if (_isAutomaticUpdate) return;

            if (e.PropertyName == nameof(ShellObjectInfo.IsSelected))

            {

                bool foundSelectedItem = false;

                bool foundNonSelectedItem = false;

                foreach (var item in Items)

                {

                    if (item.FileToCopy.IsSelected && item.FileThatAlreadyExists.IsSelected) foundSelectedItem = true;

                    if (!item.FileToCopy.IsSelected || !item.FileThatAlreadyExists.IsSelected) foundNonSelectedItem = true;

                }

                _isAutomaticUpdate = true;

                if (foundSelectedItem && foundNonSelectedItem)

                    ApplyToAll = null;

                else if (foundSelectedItem)

                    ApplyToAll = true;

                else

                    ApplyToAll = false;

                _isAutomaticUpdate = false;

            }

        }
    }
}
