using Xamarin.UITest;

namespace XamList.UITests
{
    public static class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
                return ConfigureApp.Android.StartApp(Xamarin.UITest.Configuration.AppDataMode.Clear);

            return ConfigureApp.iOS.StartApp(Xamarin.UITest.Configuration.AppDataMode.Clear);
        }
    }
}
