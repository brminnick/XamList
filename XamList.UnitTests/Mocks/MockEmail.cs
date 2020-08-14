using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace XamList.UnitTests
{
    public class MockEmail : IEmail
    {
        readonly static WeakEventManager<EmailMessage> _composeAsyncInvokedEventManager = new WeakEventManager<EmailMessage>();

        public static event EventHandler<EmailMessage> ComposeAsyncInvoked
        {
            add => _composeAsyncInvokedEventManager.AddEventHandler(value);
            remove => _composeAsyncInvokedEventManager.RemoveEventHandler(value);
        }

        public Task ComposeAsync() => ComposeAsync(new EmailMessage());

        public Task ComposeAsync(string subject, string body, params string[] to) => ComposeAsync(new EmailMessage(subject, body, to));

        public Task ComposeAsync(EmailMessage message)
        {
            _composeAsyncInvokedEventManager.RaiseEvent(this, message, nameof(ComposeAsyncInvoked));
            return Task.CompletedTask;
        }
    }
}
