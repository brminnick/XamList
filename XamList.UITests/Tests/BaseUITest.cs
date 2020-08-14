using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.Essentials.Implementation;
using Xamarin.UITest;
using XamList.Mobile.Shared;
using XamList.Shared;

namespace XamList.UITests
{
    [TestFixture(Platform.iOS)]
    [TestFixture(Platform.Android)]
    public abstract class BaseUITest
    {
        readonly Platform _platform;

        ContactsListPage? _contactsListPage;
        ContactDetailsPage? _contactDetailsPage;
        IApp? _app;

        protected BaseUITest(Platform platform) => _platform = platform;

        protected ContactsListPage ContactsListPage => _contactsListPage ?? throw new NullReferenceException();
        protected ContactDetailsPage ContactDetailsPage => _contactDetailsPage ?? throw new NullReferenceException();
        protected IApp App => _app ?? throw new NullReferenceException();

        [SetUp]
        protected virtual async Task BeforeEachTest()
        {
            _app = AppInitializer.StartApp(_platform);
            _contactsListPage = new ContactsListPage(App);
            _contactDetailsPage = new ContactDetailsPage(App);

            try
            {
                ContactsListPage.WaitForPageToLoad();
            }
            catch
            {
                ContactsListPage.PullToRefresh();
            }
            await ContactsListPage.WaitForNoPullToRefreshActivityIndicator().ConfigureAwait(false);

            await RemoveTestContactsFromDatabases().ConfigureAwait(false);


            App.Screenshot("App Started");
        }

        [TearDown]
        protected virtual Task AfterEachTest() => RemoveTestContactsFromDatabases();

        Task RemoveTestContactsFromDatabases()
        {
            App.InvokeBackdoorMethod(BackdoorMethodConstants.RemoveTestContactsFromLocalDatabase);
            return RemoveTestContactsFromRemoteDatabase();
        }

        async Task RemoveTestContactsFromRemoteDatabase()
        {
            var apiService = new ApiService(new DeviceInfoImplementation());
            var contactList = await apiService.GetAllContactModels().ConfigureAwait(false);

            Assert.IsNotNull(contactList, "Error Retrieving Contact List From Remote Database");

            var testContactList = contactList.Where(x => x.FirstName.Equals(TestConstants.TestFirstName)
                                                            && x.LastName.Equals(TestConstants.TestLastName)
                                                            && x.PhoneNumber.Equals(TestConstants.TestPhoneNumber)).ToList();

            var removedContactTaskList = new List<Task<ContactModel>>();
            foreach (var contact in testContactList)
                removedContactTaskList.Add(apiService.RemoveContactFromRemoteDatabase(contact.Id));

            await Task.WhenAll(removedContactTaskList).ConfigureAwait(false);

            var successfullyRemovedContactCount = removedContactTaskList.Count(x => x != null);
            Assert.IsTrue(testContactList.Count == successfullyRemovedContactCount,
                $"Error Removing Test Data from Remote Database\n Found {testContactList.Count} Test Contacts and Removed {successfullyRemovedContactCount} Test Contacts");
        }
    }
}

