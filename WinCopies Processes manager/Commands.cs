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

        public static RoutedUICommand Resume { get; } = new RoutedUICommand(nameof(Resume), nameof(Resume), typeof(Commands));

        public static RoutedUICommand Pause { get; } = new RoutedUICommand(nameof(Pause), nameof(Pause), typeof(Commands));

        public static RoutedUICommand Cancel { get; } = new RoutedUICommand(nameof(Cancel), nameof(Cancel), typeof(Commands));

    }
}
