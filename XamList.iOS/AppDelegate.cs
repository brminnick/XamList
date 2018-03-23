using System.Threading.Tasks;

using UIKit;
using Foundation;

using Microsoft.AppCenter.Distribute;

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
            EntryCustomReturn.Forms.Plugin.iOS.CustomReturnEntryRenderer.Init();
            Distribute.DontCheckForUpdatesInDebug();

            LoadApplication(new App());

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            Distribute.OpenUrl(url);
            return base.OpenUrl(app, url, options);
        }

#if DEBUG
        [Export("removeTestContactsFromLocalDatabase:")]
        public void RemoveTestContactsFromLocalDatabase(NSString unusedString) =>
            Task.Run(async () => await UITestBackdoorMethodHelpers.RemoveTestContactsFromLocalDatabase()).GetAwaiter().GetResult();
#endif
    }
}
