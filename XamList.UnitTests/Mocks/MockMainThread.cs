using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Xamarin.Essentials.Interfaces;

namespace XamList.UnitTests
{
    class MockMainThread : IMainThread
    {
        readonly static WeakEventManager _mainThreadRequestedEventManager = new WeakEventManager();

        public static event EventHandler MainThreadRequested
        {
            add => _mainThreadRequestedEventManager.AddEventHandler(value);
            remove => _mainThreadRequestedEventManager.RemoveEventHandler(value);
        }

        public bool IsMainThread => true;

        public void BeginInvokeOnMainThread(Action action) => action();

        public Task<SynchronizationContext> GetMainThreadSynchronizationContextAsync() => Task.FromResult(SynchronizationContext.Current ?? new SynchronizationContext());

        public Task InvokeOnMainThreadAsync(Action action)
        {
            OnMainThreadRequested();

            action();
            return Task.CompletedTask;
        }

        public Task<T> InvokeOnMainThreadAsync<T>(Func<T> func)
        {
            OnMainThreadRequested();

            var result = func();
            return Task.FromResult(result);
        }

        public Task InvokeOnMainThreadAsync(Func<Task> funcTask)
        {
            OnMainThreadRequested();

            return funcTask();
        }

        public Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask)
        {
            OnMainThreadRequested();

            return funcTask();
        }

        void OnMainThreadRequested() => _mainThreadRequestedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(MainThreadRequested));
    }
}
