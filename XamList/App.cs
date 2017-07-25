using System;

using Xamarin.Forms;

namespace XamList
{
    public class App : Application
    {
        public App() => MainPage = new BaseNavigationPage(new ContactsListPage());

        protected override async void OnStart()
        {
            base.OnStart();

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    await MobileCenterHelpers.Start(MobileCenterConstants.MobileCenterAPIKey_iOS);
                    break;
                case Device.Android:
                    await MobileCenterHelpers.Start(MobileCenterConstants.MobileCenterAPIKey_Droid);
                    break;
                default:
                    throw new Exception("Runtime Platform Not Supported");
            }
        }
    }
}
