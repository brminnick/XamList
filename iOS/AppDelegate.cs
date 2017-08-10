using UIKit;
using Foundation;
using System.Threading.Tasks;

namespace XamList.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
			#if DEBUG
			Xamarin.Calabash.Start();
			#endif

            global::Xamarin.Forms.Forms.Init();
            EntryCustomReturn.Forms.Plugin.iOS.CustomReturnEntryRenderer.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

#if DEBUG
        [Export("removeTestContactsFromLocalDatabase:")]
		public void RemoveTestContactsFromLocalDatabase() =>
            Task.Run(async () => await UITestBackdoorMethodHelpers.RemoveTestContactsFromLocalDatabase()).Wait();
#endif
	}
}
