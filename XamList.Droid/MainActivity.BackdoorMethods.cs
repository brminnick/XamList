#if DEBUG
using Autofac;
using Java.Interop;
using XamList.Mobile.Shared;

namespace XamList.Droid
{
    public partial class MainActivity
    {
        [Export(BackdoorMethodConstants.RemoveTestContactsFromLocalDatabase)]
        public async void RemoveTestContactsFromLocalDatabase() =>
             await ServiceCollection.Container.Resolve<UITestBackdoorMethodService>().RemoveTestContactsFromLocalDatabase().ConfigureAwait(false);

        [Export(BackdoorMethodConstants.TriggerPullToRefresh)]
        public void TriggerPullToRefresh() => ServiceCollection.Container.Resolve<UITestBackdoorMethodService>().TriggerPullToRegresh();
    }
}
#endif
