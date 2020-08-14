using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace XamList.UnitTests
{
    class MockLauncher : ILauncher
    {
        readonly static WeakEventManager<Uri> _openAsyncExecutedEventHandler = new WeakEventManager<Uri>();

        bool _canOpen = true;

        public static event EventHandler<Uri> OpenAsyncExecuted
        {
            add => _openAsyncExecutedEventHandler.AddEventHandler(value);
            remove => _openAsyncExecutedEventHandler.RemoveEventHandler(value);
        }

        public void SetCanOpenResult(bool canOpen) => _canOpen = canOpen;

        public Task<bool> TryOpenAsync(string uri) => CanOpenAsync(uri);

        public Task<bool> TryOpenAsync(Uri uri) => CanOpenAsync(uri);

        public Task<bool> CanOpenAsync(string uri) => CanOpenAsync(new Uri(uri));

        public Task<bool> CanOpenAsync(Uri uri) => Task.FromResult(_canOpen);

        public Task OpenAsync(string uri) => OpenAsync(new Uri(uri));

        public Task OpenAsync(Uri uri)
        {
            OnOpenAsyncExecuted(uri);
            return Task.CompletedTask;
        }

        public Task OpenAsync(OpenFileRequest request) => throw new NotImplementedException();

        void OnOpenAsyncExecuted(Uri uri) => _openAsyncExecutedEventHandler.RaiseEvent(this, uri, nameof(OpenAsyncExecuted));
    }
}
