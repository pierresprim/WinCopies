/* Copyright © Pierre Sprimont, 2021
 *
 * This file is part of the WinCopies Framework.
 *
 * The WinCopies Framework is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The WinCopies Framework is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.GUI.IO.Process;
using WinCopies.IO.Process;

namespace WinCopies
{
    public class ProcessCollectionUpdater : IUpdater
    {
        public static IProcessWindowModel Instance { get; internal set; }

        public int Run(string[] args)
        {
            IQueue<IProcessParameters> queue = new Collections.DotNetFix.Generic.Queue<IProcessParameters>();

            App.InitQueues(args, null, queue);

            App.Run(queue);

            return 0;
        }
    }

    public interface IProcessWindowModel
    {
        ICollection<IProcess> Processes { get; }
    }

    public class _Processes : IProcessWindowModel
    {
        public ICollection<IProcess> Processes { get; }

        private _Processes(in ICollection<IProcess> processes) => Processes = processes;

        public static void Init(in ICollection<IProcess> processes) => ProcessCollectionUpdater.Instance = new _Processes(processes);
    }

    public sealed partial class ProcessWindow : GUI.Windows.Window
    {
        internal ProcessWindow()
        {
            var processes = new System.Collections.ObjectModel.ObservableCollection<IProcess>();

            processes.CollectionChanged += Processes_CollectionChanged;

            ContentTemplateSelector = new InterfaceDataTemplateSelector();

            Content = new ProcessManager<IProcess>() { Processes = processes };

            _ = App.Current._OpenWindows.AddLast(this);

            _Processes.Init(processes);
        }

        private void Processes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (object process in e.NewItems)

                    ((IProcess)process).RunWorkerAsync();

                // _ = Activate();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            var processes = (Collection<IProcess>)((ProcessManager<IProcess>)Content).Processes;

            bool error = false;

            for (int i = 0; i < processes.Count; i++)

                if (processes[i].IsBusy)
                {
                    WindowState = WindowState.Minimized;

                    e.Cancel = true;

                    return;
                }

                else if (processes[i].Status == ProcessStatus.Error)

                    error = true;

            if (error && MessageBox.Show("There are errors in some processes. Are you sure you want to cancel they?", "WinCopies", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)

                e.Cancel = true;

            else

                _ = App.Current._OpenWindows.Remove(this);
        }
    }
}
