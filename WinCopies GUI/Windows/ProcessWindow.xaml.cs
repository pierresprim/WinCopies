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

using Microsoft.WindowsAPICodePack.Taskbar;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Collections.Generic;
using WinCopies.Commands;
using WinCopies.Desktop;
using WinCopies.GUI.IO.Process;
using WinCopies.IO.Process;
using WinCopies.IPCService.Extensions;

namespace WinCopies
{
    public class ProcessCollectionUpdater : IUpdater
    {
        public static IProcessWindowModel Instance { get; internal set; }

        public unsafe int Run(string[] args)
        {
            IQueue<IProcessParameters> queue = new ProcessQueue();

            IPCService.Extensions.SingleInstanceApp.Initialize(new Dictionary<string, IPCService.Extensions.Action>(1) { { Keys.Process, (string[] args, ref ArrayBuilder<string> arrayBuilder, in int* i) => Loader.LoadProcessParameters(queue, ref arrayBuilder, i, args) } }, args);

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

    public sealed partial class ProcessWindow : GUI.IO.Process.ProcessWindow
    {
        public ProcessWindow()
        {

            CommandBindings.Add(NotificationIconCommands.Close, () =>
            {
                App.Current.IsClosing = true;

                Close();
            });
            _ = App.Current._OpenWindows.AddLast(this);

            _Processes.Init(((ProcessManager<IProcess>)Content).Processes);
        }

        protected override NotificationIconData GetNotificationIconData() => new NotificationIconData(Properties.Resources.WinCopies, "WinCopies");

        protected override void OnClosingCancelled() => App.Current.IsClosing = false;

        protected override bool ValidateClosing() => App.Current.IsClosing;

        protected override void OnClosingValidated() => App.Current._OpenWindows.Remove(this);

    }
}
