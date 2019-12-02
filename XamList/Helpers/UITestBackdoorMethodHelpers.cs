#if DEBUG
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using XamList.Mobile.Shared;
using Xamarin.Forms;

namespace XamList
{
    public static class UITestBackdoorMethodHelpers
    {
        public static async Task RemoveTestContactsFromLocalDatabase()
        {
            var contactsList = await ContactDatabase.GetAllContacts().ConfigureAwait(false);

            var testContactsList = contactsList.Where(x =>
                                                    x.FirstName.Equals(TestConstants.TestFirstName) &&
                                                    x.LastName.Equals(TestConstants.TestLastName) &&
                                                    x.PhoneNumber.Equals(TestConstants.TestPhoneNumber));

            var removedContactTaskList = new List<Task>();
            foreach (var contact in testContactsList)
                removedContactTaskList.Add(ContactDatabase.RemoveContact(contact));

            await Task.WhenAll(removedContactTaskList).ConfigureAwait(false);
        }

        public static void TriggerPullToRegresh()
        {
            var navigationPage = (NavigationPage)Application.Current.MainPage;
            var listPage = (ContactsListPage)navigationPage.Navigation.NavigationStack.First();

            var listPageLayout = (Layout<View>)listPage.Content;
            var listView = listPageLayout.Children.OfType<ListView>().First();

            Device.BeginInvokeOnMainThread(listView.BeginRefresh);
        }
    }
}
#endif
