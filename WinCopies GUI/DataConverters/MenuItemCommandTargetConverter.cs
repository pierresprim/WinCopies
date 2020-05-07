using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WinCopies.GUI.Explorer;

namespace WinCopies.GUI
{
    public class MenuItemCommandTargetConverter : Util.Data.MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values[0] == null) return null;

            return ((TabItem)((MainWindow)values[1]).TabControl.ItemContainerGenerator.ContainerFromItem(values[0])).PART_ExplorerControl;

        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
