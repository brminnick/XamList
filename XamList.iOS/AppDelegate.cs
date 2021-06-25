using Autofac;
using Foundation;
using UIKit;

namespace XamList.iOS
{
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(ServiceCollection.Container.Resolve<App>());

            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}
