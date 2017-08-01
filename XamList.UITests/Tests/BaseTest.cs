using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using NUnit.Framework;

using Xamarin.UITest;

using XamList.Mobile.Common;

using XamList.Shared;

namespace XamList.UITests
{
    [TestFixture(Platform.iOS)]
    [TestFixture(Platform.Android)]
    public abstract class BaseTest
    {
        #region Fields
        IApp _app;
        Platform _platform;
        ContactsListPage _contactsListPage;
        ContactDetailsPage _contactsDetailsPage;
        #endregion

        #region Constructors
        protected BaseTest(Platform platform) => _platform = platform;
        #endregion

        #region Properties
        protected Platform Platform => _platform;
        protected IApp App => _app;
        protected ContactsListPage ContactsListPage => _contactsListPage ?? (_contactsListPage = new ContactsListPage(App, Platform));
        protected ContactDetailsPage ContactDetailsPage => _contactsDetailsPage ?? (_contactsDetailsPage = new ContactDetailsPage(App, Platform));
        #endregion

        #region Methods
        [SetUp]
        protected virtual async Task BeforeEachTest()
        {
            var contactList = await APIService.GetAllContactModels();
            Assert.IsNotNull(contactList, "Error Retriecing Contact List From Remote Database");

            var testContactList = contactList.Where(x =>
                                                    x.FirstName.Equals(Constants.TestFirstName) &&
                                                    x.LastName.Equals(Constants.TestLastName) &&
                                                    x.PhoneNumber.Equals(Constants.TestPhoneNumber)).ToList();

            var removedContactTaskList = new List<Task<HttpResponseMessage>>();
            foreach (var contact in testContactList)
                removedContactTaskList.Add(APIService.RemoveContactFromDatabase(contact));

            await Task.WhenAll(removedContactTaskList);

            var successfullyRemovedContactCount = removedContactTaskList.Count(x => x.Result.IsSuccessStatusCode);
            Assert.IsTrue(testContactList.Count == successfullyRemovedContactCount, 
                $"Error Removing Test Data from Remote Datase\n Found {testContactList.Count} Test Contacts and Removed {successfullyRemovedContactCount} Test Contacts");

            _app = AppInitializer.StartApp(Platform);
        }
        #endregion
    }
}

