using System;

using Xamarin.Forms;

namespace XamList
{
    public class App : Application
    {
        public App() => MainPage = new BaseNavigationPage(new ContactsListPage());

        protected override void OnStart()
        {
            base.OnStart();

            MobileCenterHelpers.Start();

            ConnectivityService.SubscribeEventHandlers();
        }

        protected override void OnResume()
        {
            base.OnResume();

            ConnectivityService.SubscribeEventHandlers();
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            ConnectivityService.UnsubscribeEventHandlers();
        }
    }
}
