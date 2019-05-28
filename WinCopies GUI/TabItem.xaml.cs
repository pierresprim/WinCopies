﻿using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WinCopies.GUI.Explorer;
using WinCopies.SettingsManagement;
using WinCopies.Util;
using WinCopies.Util.Data;

namespace WinCopies.GUI
{
    public partial class TabItem : System.Windows.Controls.TabItem
    {

        // public ExplorerControl PART_ExplorerControl { get; private set; } = null;



        public TabItem()

        {

            InitializeComponent();

            var b = new Binding("Header") { Source = PART_ExplorerControl };

            SetBinding(HeaderProperty, b);

            PART_ExplorerControl.PasteAction = new PasteHandler((bool isAFileMoving, System.Collections.Specialized.StringCollection sc, string destPath) =>
         {
#if DEBUG
             Debug.WriteLine("Is a file moving: " + isAFileMoving.ToString());
#endif

             string args = null;

             args += isAFileMoving ? "\"FileMove\" " : "\"Copy\" ";

             foreach (string s in sc)

                 args += "\"" + s + "\" ";

             args += "\"" + destPath + "\"";

#if DEBUG
             Debug.WriteLine(args);
#endif

             Process.Start(new ProcessStartInfo((string)Resources["WinCopiesProcessesManager"], args));
         });

            // PART_ExplorerControl = (ExplorerControl)Content;

        }

        private void PART_ExplorerControl_PathChanged(object sender, RoutedEventArgs<ValueChangedEventArgs<IBrowsableObjectInfo>> e)
        {

            ValueObject<IBrowsableObjectInfo> shellObject = (ValueObject<IBrowsableObjectInfo>)DataContext;

            IBrowsableObjectInfo newPath = e.OriginalEventArgs.NewValue;

            newPath.IsSelected = shellObject.Value.IsSelected;

            shellObject.Value = newPath;

        }

        private void PART_ExplorerControl_MultiplePathsOpeningRequested(object sender, RoutedEventArgs<EventArgs<IBrowsableObjectInfo[]>> e)
        {

            // todo: copy items in memory to avoid reference errors

            System.Collections.Generic.List<IBrowsableObjectInfo> paths = e.OriginalEventArgs.Value.ToList();

            if (((App)Application.Current).ErgonomicsProperties.OpenFolderMode == OpenFolderMode.OpenFoldersInMultipleWindows)

                for (int i = 1; i < paths.Count; i++)

                    new MainWindow().New_Tab(new ValueObject<IBrowsableObjectInfo>((IBrowsableObjectInfo)e.OriginalEventArgs.Value[i].Clone()));

            else

            {

                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

                for (int i = 1; i < paths.Count; i++)

                    mainWindow.New_Tab(new ValueObject<IBrowsableObjectInfo>((IBrowsableObjectInfo)e.OriginalEventArgs.Value[i].Clone()));

            }

        }

        private void PART_ExplorerControl_NavigationRequested(object sender, RoutedEventArgs<EventArgs<IBrowsableObjectInfo>> e) =>

            (((App)Application.Current).ErgonomicsProperties.OpenFolderMode == OpenFolderMode.OpenFoldersInMultipleWindows ? new MainWindow() : ((MainWindow)Window.GetWindow(this))).New_Tab(new ValueObject<IBrowsableObjectInfo>((IBrowsableObjectInfo) e.OriginalEventArgs. Value.Clone()));

        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();

        //    PART_ExplorerControl = (ExplorerControl)ContentTemplate.FindName(nameof(PART_ExplorerControl), this);
        //}

        //private ExplorerControl GetExplorerControl(TabItem item)

        //{
        //    Console.WriteLine(item.ContentTemplate == null);
        //    Console.WriteLine(item.ContentTemplate.GetType());
        //    Console.WriteLine(item.ContentTemplate.Template == null);
        //    Console.WriteLine(item.ContentTemplate.Template.GetType());
        //    Console.WriteLine(((ExplorerControl)item.FindName("PART_ExplorerControl")).Path);
        //    Console.WriteLine(((DataTemplate)this.ContentTemplate).FindName("PART_ExplorerControl", this));
        //    return (ExplorerControl)item.ContentTemplate.FindName("PART_ExplorerControl", item);

        //}

        //private void PART_ExplorerControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    PART_ExplorerControl.Navigate(WinCopies.IO.Util.GetNormalizedOSPath(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)), true);
        //}

        // private bool _isSelected = false;

        // private IList<Object> _SelectedItems = null;



        // public event PropertyChangedEventHandler PropertyChanged;



        // public bool IsSelected { get => _isSelected; set { SetProperty("IsSelected", "_isSelected", value); } }

        // public IList<Object> SelectedItems { get => _SelectedItems; set { _SelectedItems = value; OnPropertyChanged("SelectedItems"); } }



        //public String Header { get { return System.IO.Path.GetFileName(WinCopies.GUI.Explorer.ExplorerControl.LoadFolder.Path.Path); } }

        //public virtual void OnPropertyChanged(string propertyName)
        //{
        //    throw new NotImplementedException();
        //}

        //public virtual void OnPropertyChanged(string propertyName, string fieldName, object previousValue, object newValue)
        //{
        //    CommonHelper.OnPropertyChangedHelper(this, propertyName, fieldName, previousValue, newValue);

        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, previousValue, newValue));
        //}

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

        // private protected virtual void OnPropertyChanged(String propName)
        // {

        //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        //     Console.WriteLine(propName);

        // }

        //private void SetProperty(string propertyName, string fieldName, object newValue)

        //{

        //    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
        //                 BindingFlags.Static | BindingFlags.Instance |
        //                 BindingFlags.DeclaredOnly;
        //    this.GetType().GetField(fieldName, flags).SetValue(this, newValue);

        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //}

    }
}
