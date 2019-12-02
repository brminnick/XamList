using Foundation;
using UIKit;

namespace XamList.iOS
{
    [Register(nameof(AppDelegate))]
    public class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
#if DEBUG
            Xamarin.Calabash.Start();
#endif
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

#if DEBUG
        [Export("removeTestContactsFromLocalDatabase:")]
        public async void RemoveTestContactsFromLocalDatabase(NSString unusedString) =>
            await UITestBackdoorMethodHelpers.RemoveTestContactsFromLocalDatabase().ConfigureAwait(false);

        [Export("triggerPullToRefresh:")]
        public void TriggerPullToRefresh(NSString unusedString) => UITestBackdoorMethodHelpers.TriggerPullToRegresh();
#endif
    }
}
