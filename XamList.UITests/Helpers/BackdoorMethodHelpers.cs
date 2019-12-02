using System;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Android;

namespace XamList.UITests
{
    static class BackdoorMethodHelpers
    {
		public static void RemoveTestContactsFromLocalDatabase(IApp app)
		{
            switch(app)
            {
                case iOSApp iosApp:
                   iosApp.Invoke("removeTestContactsFromLocalDatabase:", "");
                    break;
                case AndroidApp androidApp:
                    androidApp.Invoke("RemoveTestContactsFromLocalDatabase");
                    break;

                default:
                    throw new NotSupportedException($"{app.GetType()} is not supported");
            }
		}

        public static void TriggerPullToRefresh(IApp app)
        {
            switch (app)
            {
                case iOSApp iosApp:
                    iosApp.Invoke("triggerPullToRefresh:", "");
                    break;
                case AndroidApp androidApp:
                    androidApp.Invoke("TriggerPullToRefresh");
                    break;

                default:
                    throw new NotSupportedException($"{app.GetType()} is not supported");
            }
        }
    }
}
