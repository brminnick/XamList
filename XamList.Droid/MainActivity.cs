using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Autofac;
using Java.Interop;

namespace XamList.Droid
{
    [Activity(Label = "XamList.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());
        }

#if DEBUG
        [Export(nameof(RemoveTestContactsFromLocalDatabase))]
        public async void RemoveTestContactsFromLocalDatabase() =>
             await ServiceCollection.Container.Resolve<UITestBackdoorMethodService>().RemoveTestContactsFromLocalDatabase().ConfigureAwait(false);

        [Export(nameof(TriggerPullToRefresh))]
        public void TriggerPullToRefresh() => ServiceCollection.Container.Resolve<UITestBackdoorMethodService>().TriggerPullToRegresh();
#endif
    }
}
