using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using NUnit.Framework;

using Xamarin.UITest;

using XamList.Shared;
using XamList.Mobile.Shared;

namespace XamList.UITests
{
    [TestFixture(Platform.iOS)]
    [TestFixture(Platform.Android)]
    public abstract class BaseUITest
    {
        #region Constant Fields
        readonly Platform _platform;
        #endregion

        #region Constructors
        protected BaseUITest(Platform platform) => _platform = platform;
        #endregion

        #region Properties
        protected ContactsListPage ContactsListPage { get; private set; }
        protected ContactDetailsPage ContactDetailsPage { get; private set; }
        protected IApp App { get; private set; }
        #endregion

        #region Methods
        [SetUp]
        protected virtual async Task BeforeEachTest()
        {
            App = AppInitializer.StartApp(_platform);
            ContactsListPage = new ContactsListPage(App);
            ContactDetailsPage = new ContactDetailsPage(App);

            ContactsListPage.WaitForPageToLoad();
            await ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync().ConfigureAwait(false);

            await RemoveTestContactsFromDatabases().ConfigureAwait(false);

            ContactsListPage.PullToRefresh();

            App.Screenshot("App Started");
        }

        [TearDown]
        protected virtual Task AfterEachTest() => RemoveTestContactsFromDatabases();

        Task RemoveTestContactsFromDatabases()
        {
            BackdoorMethodHelpers.RemoveTestContactsFromLocalDatabase(App);
            return RemoveTestContactsFromRemoteDatabase();
        }

        async Task RemoveTestContactsFromRemoteDatabase()
        {
            var contactList = await ApiService.GetAllContactModels().ConfigureAwait(false);

            Assert.IsNotNull(contactList, "Error Retrieving Contact List From Remote Database");

            var testContactList = contactList.Where(x => x.FirstName.Equals(TestConstants.TestFirstName)
                                                            && x.LastName.Equals(TestConstants.TestLastName)
                                                            && x.PhoneNumber.Equals(TestConstants.TestPhoneNumber)).ToList();

            var removedContactTaskList = new List<Task<ContactModel>>();
            foreach (var contact in testContactList)
                removedContactTaskList.Add(ApiService.RemoveContactFromRemoteDatabase(contact.Id));

            await Task.WhenAll(removedContactTaskList).ConfigureAwait(false);

            var successfullyRemovedContactCount = removedContactTaskList.Count(x => x != null);
            Assert.IsTrue(testContactList.Count == successfullyRemovedContactCount,
                $"Error Removing Test Data from Remote Database\n Found {testContactList.Count} Test Contacts and Removed {successfullyRemovedContactCount} Test Contacts");
        }
        #endregion
    }
}

