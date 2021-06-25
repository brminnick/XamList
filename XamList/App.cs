using Xamarin.Forms;

namespace XamList
{
    public class App : Application
    {
        readonly AppCenterService _appCenterService;

        public App(AppCenterService appCenterService, ContactsListPage contactsListPage)
        {
            _appCenterService = appCenterService;
            MainPage = new BaseNavigationPage(contactsListPage);
        }

        protected override void OnStart()
        {
            base.OnStart();

            _appCenterService.Start();
        }
    }
}
