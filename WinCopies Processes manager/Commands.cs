using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WinCopiesProcessesManager
{
    public static class Commands 
    {

        public static RoutedUICommand Resume { get; } = new RoutedUICommand("Resume", "Resume", typeof(Commands));    

        public static RoutedUICommand Pause { get; } = new RoutedUICommand("Pause", "Pause", typeof(Commands)) ;

        public static RoutedUICommand Cancel { get; } = new RoutedUICommand("Cancel", "Cancel", typeof(Commands)); 

    }
}
