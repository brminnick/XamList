#if DEBUG
using Autofac;
using Foundation;
using XamList.Mobile.Shared;

namespace XamList.iOS
{
    public partial class AppDelegate
    {
        public AppDelegate() => Xamarin.Calabash.Start();

        [Export(BackdoorMethodConstants.RemoveTestContactsFromLocalDatabase + ":")]
        public async void RemoveTestContactsFromLocalDatabase(NSString unusedString) =>
            await ServiceCollection.Container.Resolve<UITestBackdoorMethodService>().RemoveTestContactsFromLocalDatabase().ConfigureAwait(false);

        [Export(BackdoorMethodConstants.TriggerPullToRefresh + ":")]
        public void TriggerPullToRefresh(NSString unusedString) => ServiceCollection.Container.Resolve<UITestBackdoorMethodService>().TriggerPullToRegresh();
    }
}
#endif
