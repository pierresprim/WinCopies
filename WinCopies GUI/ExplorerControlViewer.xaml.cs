//using System;
//using System.Diagnostics;
//using System.Windows;
//using System.Windows.Controls;

//namespace WinCopiesGUI
//{
//    /// <summary>
//    /// Logique d'interaction pour ExplorerControlViewer.xaml
//    /// </summary>
//    public partial class ExplorerControlViewer : Control
//    {

//        public static readonly DependencyProperty ExplorerControlProperty = DependencyProperty.Register("ExplorerControl", typeof(WinCopies.GUI.Explorer.ExplorerControl), typeof(ExplorerControlViewer));



//        // todo: using the DataContext property instead of this

//        public WinCopies.GUI.Explorer.ExplorerControl ExplorerControl
//        {
//            get => (WinCopies.GUI.Explorer.ExplorerControl)GetValue(ExplorerControlProperty);

//            set { SetValue(ExplorerControlProperty, value); }
//        }



//        public ExplorerControlViewer()
//        {
//            InitializeComponent();
//            // DataContext = WinCopies.GUI.Explorer.ExplorerControl;
//        }

//        //public override void OnApplyTemplate()
//        //{
//        //    base.OnApplyTemplate();

//        //    ExplorerControl.PART_ListView = (WinCopies.GUI.ModernControls.ListView)Template.FindName("PART_ListView", this);
//        //}

//        //private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//        //{
//        //    ExplorerControl.Open(ExplorerControl.SelectedItem);
//        //}

//        private void MenuItem_Click(object sender, RoutedEventArgs e)
//        {
//            if (!IsLoaded) return;
//            var fileDropList = new System.Collections.Specialized.StringCollection();
//            // foreach (Microsoft.WindowsAPICodePack.ShellObject shellObject in Resources.FindName("bidule"))
//            // Clipboard.SetFileDropList(fileDropList);
//#if DEBUG
//            Console.WriteLine(((TabItem)DataContext).ExplorerControl.SelectedItems.Count.ToString());
//#endif
//        }
//    }
//}
