#if DEBUG
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using XamList.Mobile.Shared;
using Xamarin.Forms;
using Xamarin.Essentials.Interfaces;

namespace XamList
{
    public class UITestBackdoorMethodService
    {
        readonly IMainThread _mainThread;
        readonly ContactDatabase _contactDatabase;

        public UITestBackdoorMethodService(ContactDatabase contactDatabase, IMainThread mainThread) =>
            (_contactDatabase, _mainThread) = (contactDatabase, mainThread);

        public async Task RemoveTestContactsFromLocalDatabase()
        {
            var contactsList = await _contactDatabase.GetAllContacts().ConfigureAwait(false);

            var testContactsList = contactsList.Where(x =>
                                                    x.FirstName.Equals(TestConstants.TestFirstName) &&
                                                    x.LastName.Equals(TestConstants.TestLastName) &&
                                                    x.PhoneNumber.Equals(TestConstants.TestPhoneNumber));

            var removedContactTaskList = new List<Task>();
            foreach (var contact in testContactsList)
                removedContactTaskList.Add(_contactDatabase.RemoveContact(contact));

            await Task.WhenAll(removedContactTaskList).ConfigureAwait(false);
        }

        public void TriggerPullToRegresh()
        {
            var navigationPage = (NavigationPage)Application.Current.MainPage;
            var listPage = (ContactsListPage)navigationPage.Navigation.NavigationStack.First();

            var listPageLayout = (Layout<View>)listPage.Content;
            var listView = listPageLayout.Children.OfType<ListView>().First();

            _mainThread.BeginInvokeOnMainThread(listView.BeginRefresh);
        }
    }
}
#endif
