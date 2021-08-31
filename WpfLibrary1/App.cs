using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Linq;
using WinCopies.IPCService.Extensions;

using static WinCopies.ThrowHelper;
using WinCopies;
using System.Threading;
using System.Linq.Expressions;
using WinCopies.Collections.DotNetFix;

namespace WpfLibrary1
{
    public unsafe delegate void Action(string[] args, ref ArrayBuilder<string> arrayBuilder, in int* i);

    public interface IQueueBase : ISimpleLinkedListBase
    {
        Task Run();

        string PeekAsString();

        string DequeueAsString();
    }

    public interface IQueue : WinCopies.Collections.DotNetFix.IQueue, IQueueBase
    {

    }

    public interface IQueue<T> : WinCopies.Collections.DotNetFix.Generic.IQueue<T>, IQueueBase
    {

    }

    public abstract class Queue : WinCopies.Collections.DotNetFix.Generic.Queue<string>, IQueue<string>
    {
        string IQueueBase.DequeueAsString() => Dequeue();

        string IQueueBase.PeekAsString() => Peek();

        Task IQueueBase.Run() => Run();

        protected abstract Task Run();
    }

    public interface IUpdater
    {
        int Run(string[] args);
    }

    public abstract class Loader
    {
        public abstract string DefaultKey { get; }

        public abstract IQueueBase DefaultQueue { get; }

        public abstract IDictionary<string, Action> GetActions();

        public abstract System.Collections.Generic.IEnumerable<IQueueBase> GetQueues();
    }

    public abstract class AppLoader<T> : Loader where T : class
    {
        public SingleInstanceApp<T> App { get; internal set; }
    }

    public abstract class SingleInstanceApp
    {
        public static void StartInstance(in System.Collections.Generic.IEnumerable<string> parameters) => System.Diagnostics.Process.Start(GetAssemblyDirectory() + "\\WinCopies.exe", parameters);

        public static unsafe System.Collections.Generic.IEnumerable<string> GetArray(ref ArrayBuilder<string> arrayBuilder, System.Collections.Generic.IEnumerable<string> keys, int* i, params string[] args)
        {
            if (arrayBuilder == null)

                arrayBuilder = new ArrayBuilder<string>();

            else

                arrayBuilder.Clear();

            foreach (string value in new Enumerable<string>(() => new ArrayEnumerator(args, i)).TakeWhile(arg => !keys.Contains(arg)))

                _ = arrayBuilder.AddLast(value);

            // (*i)++;

            return arrayBuilder.ToArray();
        }

        #region Main Methods
        public async Task MainMutex<TItems, TClass>(ISingleInstanceApp<IUpdater, int> app, bool paths, IQueue<TItems> pathQueue) where TClass : class, IUpdater
        {
            (Mutex mutex, bool mutexExists, NullableGeneric<int> serverResult) = await app.StartInstanceAsync<IUpdater, TClass, int>();

            using (mutex)

                if (mutexExists)

                    if (paths && pathQueue == null)

                        await WinCopies.IPCService.Extensions.Extensions.StartThread(() => GetDefaultApp<TItems>(null).Run(), 0);

                    else

                        Environment.Exit(serverResult == null ? 0 : serverResult.Value);
        }

        private protected abstract Task MainDefault();

        public async Task Main(params string[] args)
        {
            if (args.Length == 0)
            {
                await MainDefault();

                return;
            }

            Loader loader = GetLoader();

            Initialize(loader.GetActions(), args);

            IQueueBase defaultArgs = loader.DefaultQueue;

            System.Collections.Generic.IEnumerable<IQueueBase> queues = loader.GetQueues();

            bool runDefault = true;

            async Task run(IQueueBase queue) => await queue.Run();

            foreach (IQueueBase queue in queues)

                if (queue.HasItems)
                {
                    runDefault = false;

                    if (defaultArgs.HasItems)
                    {
                        System.Collections.Generic.IEnumerable<string> pathsToEnumerable()
                        {
                            do
                            {
                                yield return loader.DefaultKey;

                                yield return defaultArgs.DequeueAsString();
                            }
                            while (defaultArgs.HasItems);
                        }

                        StartInstance(pathsToEnumerable());
                    }

                    await run(queue);
                }

            if (runDefault)

                await run(defaultArgs);
        }
        #endregion

        protected abstract Loader GetLoader();

        public abstract App GetDefaultApp<TItems>(IQueue<TItems> queue);

        private unsafe delegate void Action(int* i);

        public static string GetAssemblyDirectory() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void StartInstance(in string fileName, in System.Collections.Generic.IEnumerable<string> parameters) => System.Diagnostics.Process.Start(GetAssemblyDirectory() + $"\\{fileName}", parameters);

        private static unsafe void RunAction(in Action action, int* i)
        {
            (*i)++;

            action(i);
        }

        private static unsafe void RunAction(ref int i, ref ArrayBuilder<string> arrayBuilder, KeyValuePair<string, WpfLibrary1.Action> keyValuePair, params string[] args)
        {
            int _i = i;
            ArrayBuilder<string> _arrayBuilder = arrayBuilder;

            RunAction(i => keyValuePair.Value(args, ref _arrayBuilder, i), &_i);

            i = _i;
            arrayBuilder = _arrayBuilder;
        }

        public static void Initialize(in IDictionary<string, WpfLibrary1.Action> actions, params string[] args)
        {
            ArrayBuilder<string> arrayBuilder = null;
            KeyValuePair<string, WpfLibrary1.Action> keyValuePair;

            for (int i = 0; i < args.Length;)

                if (actions.FirstOrDefaultValue(keyValuePair => keyValuePair.Key == args[i], out keyValuePair))

                    RunAction(ref i, ref arrayBuilder, keyValuePair, args);

            arrayBuilder?.Clear();
        }

        protected static ObservableLinkedCollection<Window> GetOpenWindows(in App app) => app._OpenWindows;

        protected static void SetOpenWindows(in App app, in WinCopies.Collections.DotNetFix.Generic.IUIntCountableEnumerable<Window> enumerable) => app.OpenWindows = enumerable;
    }

    public abstract class SingleInstanceApp<T> : SingleInstanceApp where T : class
    {
        public abstract SingleInstanceAppInstance<IQueue<T>> GetDefaultSingleInstanceApp(IQueue<T> args);

        public async Task MainDefault(IQueue<T> args) => await MainMutex<T, IUpdater>(GetDefaultSingleInstanceApp(args), true, args);

        private protected override Task MainDefault() => MainDefault(null);
    }

    public abstract class SingleInstanceApp2<T> : SingleInstanceApp<T> where T : class
    {
        protected override Loader GetLoader()
        {
            AppLoader<T> loader = GetLoaderOverride();

            loader.App = this;

            return loader;
        }

        protected abstract AppLoader<T> GetLoaderOverride();
    }

    public abstract class App : Application
    {
        public bool IsClosing { get; protected set; }

        protected internal ObservableLinkedCollection<Window> _OpenWindows { get; } = new ObservableLinkedCollection<Window>();

        public WinCopies.Collections.DotNetFix.Generic.IUIntCountableEnumerable<Window> OpenWindows { get; internal set; }

        public static ResourceDictionary GetResourceDictionary(in string name) => new() { Source = new Uri(name, UriKind.Relative) };

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _OpenWindows.CollectionChanged += OpenWindows_CollectionChanged;

            //MainWindow = new MainWindow();

            //MainWindow.Closed += MainWindow_Closed;

            OnStartup2(e);

            MainWindow.Show();
        }

        protected abstract void OnStartup2(StartupEventArgs e);

        private void OpenWindows_CollectionChanged(object sender, LinkedCollectionChangedEventArgs<Window> e) => Environment.Exit(0);
    }

    public abstract class SingleInstanceAppInstance<T> : ISingleInstanceApp<IUpdater, int> where T : class
    {
        private readonly string _pipeName;

        protected T InnerObject { get; private set; }

        protected SingleInstanceAppInstance(in string pipeName, in T innerObject)
        {
            _pipeName = pipeName;

            InnerObject = innerObject;
        }

        public string GetPipeName() => _pipeName;

        public abstract string GetClientName();

        private void Run()
        {
            App app = GetApp();

            InnerObject = null;

            _ = app.Run();
        }

        public ThreadStart GetThreadStart(out int maxStackSize)
        {
            maxStackSize = 0;

            return Run;
        }

        protected abstract App GetApp();

        protected abstract Expression<Func<IUpdater, int>> GetExpressionOverride();

        public Expression<Func<IUpdater, int>> GetExpression()
        {
            Expression<Func<IUpdater, int>> result = GetExpressionOverride();

            InnerObject = null;

            return result;
        }

        public Expression<Func<IUpdater, Task<int>>> GetAsyncExpression() => null;

        public CancellationToken? GetCancellationToken() => null;
    }

#if !WinCopies4
    public class ArrayEnumerator<T> : Enumerator<T>, ICountableDisposableEnumeratorInfo<T>
    {
        private System.Collections.Generic.IReadOnlyList<T> _array;
        private readonly unsafe int* _currentIndex;
        private readonly int _startIndex;
        private Func<bool> _condition;
        private System.Action _moveNext;

        protected System.Collections.Generic.IReadOnlyList<T> Array => IsDisposed ? throw GetExceptionForDispose(false) : _array;

        public int Count => IsDisposed ? throw GetExceptionForDispose(false) : _array.Count;

        protected unsafe int CurrentIndex => IsDisposed ? throw GetExceptionForDispose(false) : *_currentIndex;

        public unsafe ArrayEnumerator(in System.Collections.Generic.IReadOnlyList<T> array, in bool reverse = false, int* startIndex = null)
        {
            _array = array ?? throw GetArgumentNullException(nameof(array));

            if (startIndex != null && (*startIndex < 0 || *startIndex >= array.Count))

                throw new ArgumentOutOfRangeException(nameof(startIndex), *startIndex, $"The given index is less than zero or greater than or equal to {nameof(array.Count)}.");

            _currentIndex = startIndex;

            if (reverse)
            {
                _startIndex = startIndex == null ? _array.Count - 1 : *startIndex;
                _condition = () => *_currentIndex > 0;
                _moveNext = () => (*_currentIndex)--;
            }

            else
            {
                _startIndex = startIndex == null ? 0 : *startIndex;
                _condition = () => *_currentIndex < _array.Count - 1;
                _moveNext = () => (*_currentIndex)++;
            }
        }

        protected override unsafe T CurrentOverride => _array[*_currentIndex];

        public override bool? IsResetSupported => true;

        protected override bool MoveNextOverride()
        {
            if (_condition())
            {
                _moveNext();

                return true;
            }

            return false;
        }

        protected override unsafe void ResetCurrent() => *_currentIndex = _startIndex;

        protected override void DisposeManaged()
        {
            _array = null;
            _condition = null;
            _moveNext = null;

            Reset();
        }
    }

    public class CustomEnumeratorProvider<TItems, TEnumerator> : System.Collections.Generic.IEnumerable<TItems> where TEnumerator : System.Collections.Generic.IEnumerator<TItems>
    {
        protected Func<TEnumerator> Func { get; }

        public CustomEnumeratorProvider(in Func<TEnumerator> func) => Func = func;

        public TEnumerator GetEnumerator() => Func();

        System.Collections.Generic.IEnumerator<TItems> System.Collections.Generic.IEnumerable<TItems>.GetEnumerator() => Func();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Func();
    }

    public class UIntCountableProvider<TItems, TEnumerator> : CustomEnumeratorProvider<TItems, TEnumerator>, WinCopies.Collections.DotNetFix.Generic.IUIntCountableEnumerable<TItems> where TEnumerator : IEnumeratorInfo2<TItems>
    {
        private Func<uint> CountFunc { get; }

        uint IUIntCountable.Count => CountFunc();

        public UIntCountableProvider(in Func<TEnumerator> func, in Func<uint> countFunc) : base(func) => CountFunc = countFunc;

        IUIntCountableEnumerator<TItems> IUIntCountableEnumerable<TItems, IUIntCountableEnumerator<TItems>>.GetEnumerator() => new UIntCountableEnumeratorInfo<TItems>(GetEnumerator(), CountFunc);
    }
#endif

    public class ArrayEnumerator : ArrayEnumerator<string>
    {
        public unsafe ArrayEnumerator(in System.Collections.Generic.IReadOnlyList<string> array, int* startIndex = null) : base(array, false, startIndex) { }

        protected override void ResetCurrent() { }
    }
}
