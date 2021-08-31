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
using WinCopies.GUI.IO.Process;
using WinCopies.IO.Process;
using WpfLibrary1;

namespace WinCopies
{
    public class ProcessCollectionUpdater : IUpdater
    {
        public static IProcessWindowModel Instance { get; internal set; }

        public unsafe int Run(string[] args)
        {
            IQueue<IProcessParameters> queue = new ProcessQueue();

            WpfLibrary1.SingleInstanceApp.Initialize(new Dictionary<string, WpfLibrary1.Action>(1) { { Keys.Process, (string[] args, ref ArrayBuilder<string> arrayBuilder, in int* i) => Loader.LoadProcessParameters(queue, ref arrayBuilder, i, args) } }, args);

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
        private NotificationIcon _icon;
        private readonly RoutedUICommand _showWindow = GetRoutedUICommand(string.Empty, "ShowWindow");
        private readonly RoutedUICommand _close = GetRoutedUICommand("Quit", "Quit");
        private System.Windows.Controls.MenuItem _showWindowMenuItem;

        internal ProcessWindow()
        {
            var processes = new System.Collections.ObjectModel.ObservableCollection<IProcess>();

            processes.CollectionChanged += Processes_CollectionChanged;

            void addCommandBinding(in ICommand command, System.Action _delegate) => CommandBindings.Add(new CommandBinding(command, (object sender, ExecutedRoutedEventArgs e) => _delegate(), (object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true));

            addCommandBinding(_showWindow, ChangeWindowVisibilityState);

            addCommandBinding(_close, () =>
            {
                App.Current.IsClosing = true;

                Close();
            });

            ContentTemplateSelector = new InterfaceDataTemplateSelector();

            Content = new ProcessManager<IProcess>() { Processes = processes };

            _ = App.Current._OpenWindows.AddLast(this);

            _Processes.Init(processes);
        }

        private static RoutedUICommand GetRoutedUICommand(in string text, in string name) => new(text, name, typeof(ProcessWindow));

        private void HideWindow()
        {
            Hide();

            _showWindowMenuItem.Header = "Show window";
        }

        private void ChangeWindowVisibilityState()
        {
            if (Visibility == Visibility.Visible)

                HideWindow();

            else
            {
                Show();

                _ = Activate();

                _showWindowMenuItem.Header = "Minimize window";
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var contextMenu = new ContextMenu();

            System.Windows.Controls.MenuItem addMenuItem(in ICommand command, in string header)
            {
                var menuItem = new System.Windows.Controls.MenuItem() { Command = command, CommandTarget = this };

                _ = contextMenu.Items.Add(menuItem);

                if (header != null)

                    menuItem.Header = header;

                return menuItem;
            }

            _showWindowMenuItem = addMenuItem(_showWindow, "Minimize window");

            _ = addMenuItem(_close, null);

            contextMenu.ContextMenuOpening += (object sender, ContextMenuEventArgs e) => CommandManager.InvalidateRequerySuggested();

            _icon = new NotificationIcon(this, Guid.NewGuid(), Properties.Resources.WinCopies, "WinCopies", contextMenu, true);

            _icon.LeftButtonUp += (object? sender, EventArgs e) => ChangeWindowVisibilityState();

            _ = _icon.Initialize();
        }

        private void Processes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)

                foreach (object process in e.NewItems)

                    ((IProcess)process).RunWorkerAsync();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (App.Current.IsClosing)
            {
                var processes = (Collection<IProcess>)((ProcessManager<IProcess>)Content).Processes;

                void cancel(in System.Action action)
                {
                    action?.Invoke();

                    App.Current.IsClosing = false;

                    e.Cancel = true;
                }

                for (int i = 0; i < processes.Count; i++)

                    if (processes[i].IsBusy)
                    {
                        cancel(() => MessageBox.Show("There are running processes. All processes have to be completed in order to close the application.", "WinCopies", MessageBoxButton.OK, MessageBoxImage.Information));

                        return;
                    }

                    else if (processes[i].Status == ProcessStatus.Error && MessageBox.Show("There are errors in some processes. Are you sure you want to cancel they?", "WinCopies", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    {
                        cancel(null);

                        return;
                    }

                _icon.Dispose();

                _ = App.Current._OpenWindows.Remove(this);
            }

            else
            {
                HideWindow();

                e.Cancel = true;
            }
        }
    }
}
