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
        #region Constructors
        protected BaseUITest(Platform platform) => Platform = platform;
        #endregion

        #region Properties
        protected Platform Platform { get; }

        protected ContactsListPage ContactsListPage { get; private set; }
        protected ContactDetailsPage ContactDetailsPage { get; private set; }
        protected IApp App { get; private set; }
        #endregion

        #region Methods
        [SetUp]
        protected virtual void BeforeEachTest()
        {
            App = AppInitializer.StartApp(Platform);
            ContactsListPage = new ContactsListPage(App, Platform);
            ContactDetailsPage = new ContactDetailsPage(App, Platform);

            RevoveTestContactsFromDatabases(App).GetAwaiter();

            ContactsListPage.WaitForPageToLoad();
            ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync().GetAwaiter();
        }

        [TearDown]
        protected virtual void AfterEachTest() => RevoveTestContactsFromDatabases(App).GetAwaiter();

        Task RevoveTestContactsFromDatabases(IApp app)
        {
            var removeTestContactsFromRemoteDatabaseTask = RemoveTestContactsFromRemoteDatabase();
            var removeTestContactsFromLocalDatabaseTask = Task.Run(() => BackdoorMethodHelpers.RemoveTestContactsFromLocalDatabase(app));

            return Task.WhenAll(removeTestContactsFromLocalDatabaseTask, removeTestContactsFromRemoteDatabaseTask);
        }

        async Task RemoveTestContactsFromRemoteDatabase()
        {
            var contactList = await APIService.GetAllContactModels().ConfigureAwait(false);

            Assert.IsNotNull(contactList, "Error Retrieving Contact List From Remote Database");

            var testContactList = contactList.Where(x =>
                                                    x.FirstName.Equals(TestConstants.TestFirstName) &&
                                                    x.LastName.Equals(TestConstants.TestLastName) &&
                                                    x.PhoneNumber.Equals(TestConstants.TestPhoneNumber)).ToList();

            var removedContactTaskList = new List<Task<ContactModel>>();
            foreach (var contact in testContactList)
                removedContactTaskList.Add(APIService.RemoveContactFromRemoteDatabase(contact.Id));

            await Task.WhenAll(removedContactTaskList).ConfigureAwait(false);

            var successfullyRemovedContactCount = removedContactTaskList.Count(x => x != null);
            Assert.IsTrue(testContactList.Count == successfullyRemovedContactCount,
                $"Error Removing Test Data from Remote Database\n Found {testContactList.Count} Test Contacts and Removed {successfullyRemovedContactCount} Test Contacts");
        }
        #endregion
    }
}

