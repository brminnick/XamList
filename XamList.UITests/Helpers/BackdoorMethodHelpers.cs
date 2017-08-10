using System;

using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Android;

namespace XamList.UITests
{
    public static class BackdoorMethodHelpers
    {
		internal static void RemoveTestContactsFromLocalDatabase(IApp app)
		{
            switch(app)
            {
                case iOSApp app_iOS:
                   app_iOS.Invoke("removeTestContactsFromLocalDatabase:");
                    break;
                case AndroidApp app_Android:
					app.Invoke("RemoveTestContactsFromLocalDatabase");
                    break;
                default:
                    throw new Exception("App Type Not Supported");
            }
		}
    }
}
