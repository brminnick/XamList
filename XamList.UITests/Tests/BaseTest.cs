using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using NUnit.Framework;

using Newtonsoft.Json;

using Xamarin.UITest;

using XamList.Shared;
using XamList.Mobile.Common;

namespace XamList.UITests
{
    [TestFixture(Platform.iOS)]
    [TestFixture(Platform.Android)]
    public abstract class BaseTest
    {
        #region Fields
        IApp _app;
        HttpClient _client;
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

        HttpClient Client => _client ?? (_client = CreateHttpClient());
        #endregion

        #region Methods
        [SetUp]
        protected virtual void BeforeEachTest()
        {
            _app = AppInitializer.StartApp(Platform);

            Task.Run(async () => await RemoveTestContactsFromDatabases()).GetAwaiter().GetResult();

            ContactsListPage.WaitForPageToLoad();
            ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync().GetAwaiter().GetResult();
        }

        [TearDown]
        protected virtual void AfterEachTest() =>
            Task.Run(async () => await RemoveTestContactsFromDatabases()).GetAwaiter().GetResult();

        HttpClient CreateHttpClient()
        {
            var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip })
            {
                Timeout = System.TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            return client;
        }

        async Task RemoveTestContactsFromDatabases()
        {
            await RemoveTestContactsFromRemoteDatabase();
            BackdoorMethodHelpers.RemoveTestContactsFromLocalDatabase(App);
        }

        async Task RemoveTestContactsFromRemoteDatabase()
        {
            var contactList = await GetContactsFromRemoteDatabase();

            Assert.IsNotNull(contactList, "Error Retriecing Contact List From Remote Database");

            var testContactList = contactList.Where(x =>
                                                    x.FirstName.Equals(TestConstants.TestFirstName) &&
                                                    x.LastName.Equals(TestConstants.TestLastName) &&
                                                    x.PhoneNumber.Equals(TestConstants.TestPhoneNumber)).ToList();

            var removedContactTaskList = new List<Task<HttpResponseMessage>>();
            foreach (var contact in testContactList)
                removedContactTaskList.Add(RemoveContactFromRemoteDatabase(contact));

            await Task.WhenAll(removedContactTaskList);

            var successfullyRemovedContactCount = removedContactTaskList.Count(x => x.Result.IsSuccessStatusCode);
            Assert.IsTrue(testContactList.Count == successfullyRemovedContactCount,
                $"Error Removing Test Data from Remote Database\n Found {testContactList.Count} Test Contacts and Removed {successfullyRemovedContactCount} Test Contacts");
        }

        async Task<List<ContactModel>> GetContactsFromRemoteDatabase()
        {
            var jsonString = await Client.GetStringAsync($"{BackendConstants.AzureAPIUrl}GetAllContacts");
            return JsonConvert.DeserializeObject<List<ContactModel>>(jsonString);
        }

        Task<HttpResponseMessage> RemoveContactFromRemoteDatabase(ContactModel contact)
        {
            var apiUrl = $"{BackendConstants.AzureFunctionUrl}RemoveItemFromDatabase/{contact.Id}?code={BackendConstants.AzureFunctionKey_RemoveItemFromDatabase}";

            return Client.PostAsync(apiUrl, null);
        }


        #endregion
    }
}

