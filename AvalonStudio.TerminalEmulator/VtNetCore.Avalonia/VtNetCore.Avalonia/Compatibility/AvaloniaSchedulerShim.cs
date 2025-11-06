using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace AvalonStudio.Compat
{
    // Minimal IScheduler implementation that dispatches work to Avalonia's Dispatcher.
    public sealed class AvaloniaDispatcherScheduler : IScheduler
    {
        public static readonly AvaloniaDispatcherScheduler Instance = new AvaloniaDispatcherScheduler();
        public DateTimeOffset Now => DateTimeOffset.UtcNow;

        private AvaloniaDispatcherScheduler() { }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            var disposed = false;
            Dispatcher.UIThread.Post(() =>
            {
                if (!disposed)
                {
                    action(this, state);
                }
            });
            return System.Reactive.Disposables.Disposable.Create(() => disposed = true);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var cts = new CancellationTokenSource();
            var disposed = false;
            Task.Delay(dueTime).ContinueWith(t =>
            {
                if (!cts.IsCancellationRequested && !disposed)
                {
                    Dispatcher.UIThread.Post(() => action(this, state));
                }
            });
            return System.Reactive.Disposables.Disposable.Create(() => { cts.Cancel(); disposed = true; });
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var dt = dueTime - DateTimeOffset.UtcNow;
            if (dt < TimeSpan.Zero) dt = TimeSpan.Zero;
            return Schedule(state, dt, action);
        }
    }
}
