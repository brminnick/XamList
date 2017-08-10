using System.Threading.Tasks;

using Android.OS;
using Android.App;
using Android.Content.PM;

using Java.Interop;

namespace XamList.Droid
{
    [Activity(Label = "XamList.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            EntryCustomReturn.Forms.Plugin.Android.CustomReturnEntryRenderer.Init();

            LoadApplication(new App());
        }

#if DEBUG
        [Export("RemoveTestContactsFromLocalDatabase")]
        public void RemoveTestContactsFromLocalDatabase() =>
            Task.Run(async () => await UITestBackdoorMethodHelpers.RemoveTestContactsFromLocalDatabase()).Wait();
#endif
    }
}
