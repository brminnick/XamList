using Autofac;
using Xamarin.Forms;

namespace XamList
{
    public class App : Application
    {
        readonly AppCenterService _appCenterService;

        public App()
        {
            _appCenterService = ServiceCollection.Container.Resolve<AppCenterService>();

            var contactsListPage = ServiceCollection.Container.Resolve<ContactsListPage>();
            MainPage = new BaseNavigationPage(contactsListPage);
        }

        protected override void OnStart()
        {
            base.OnStart();

            _appCenterService.Start();
        }
    }
}
