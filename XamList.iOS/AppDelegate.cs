using System.Threading.Tasks;

using UIKit;
using Foundation;

namespace XamList.iOS
{
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
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
#endif
    }
}
