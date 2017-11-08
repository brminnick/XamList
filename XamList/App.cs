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
    }
}
