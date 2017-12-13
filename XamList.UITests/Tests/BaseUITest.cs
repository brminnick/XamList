using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Newtonsoft.Json;

using NUnit.Framework;

using Xamarin.UITest;
using XamList.Shared;

namespace XamList.UITests
{
    [TestFixture(Platform.iOS)]
    [TestFixture(Platform.Android)]
    public abstract class BaseUITest
    {
        #region Constant Fields
        readonly Lazy<HttpClient> _clientHolder = new Lazy<HttpClient>(CreateHttpClient);
        #endregion

        #region Constructors
        protected BaseUITest(Platform platform) => Platform = platform;
        #endregion

        #region Properties
        protected Platform Platform { get; }

        protected ContactsListPage ContactsListPage { get; private set; }
        protected ContactDetailsPage ContactDetailsPage { get; private set; }
        protected IApp App { get; private set; }

        HttpClient Client => _clientHolder.Value;
        #endregion

        #region Methods
        [SetUp]
        protected virtual void BeforeEachTest()
        {
            App = AppInitializer.StartApp(Platform);
            ContactsListPage = new ContactsListPage(App, Platform);
            ContactDetailsPage = new ContactDetailsPage(App, Platform);

            RevoveTestContactsFromDatabases(App).GetAwaiter().GetResult();

            ContactsListPage.WaitForPageToLoad();
            ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync().GetAwaiter().GetResult();
        }

        [TearDown]
        protected virtual void AfterEachTest() => RevoveTestContactsFromDatabases(App).GetAwaiter().GetResult();

        static HttpClient CreateHttpClient()
        {
            var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip })
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            return client;
        }

        Task RevoveTestContactsFromDatabases(IApp app)
        {
            var removeTestContactsFromRemoteDatabaseTask = RemoveTestContactsFromRemoteDatabase();
            var removeTestContactsFromLocalDatabaseTask = Task.Run(() => BackdoorMethodHelpers.RemoveTestContactsFromLocalDatabase(app));

            return Task.WhenAll(removeTestContactsFromLocalDatabaseTask, removeTestContactsFromRemoteDatabaseTask);
        }

        async Task RemoveTestContactsFromRemoteDatabase()
        {
            var contactList = await GetContactsFromRemoteDatabase().ConfigureAwait(false);

            Assert.IsNotNull(contactList, "Error Retriecing Contact List From Remote Database");

            var testContactList = contactList.Where(x =>
                                                    x.FirstName.Equals(TestConstants.TestFirstName) &&
                                                    x.LastName.Equals(TestConstants.TestLastName) &&
                                                    x.PhoneNumber.Equals(TestConstants.TestPhoneNumber)).ToList();

            var removedContactTaskList = new List<Task<HttpResponseMessage>>();
            foreach (var contact in testContactList)
                removedContactTaskList.Add(RemoveContactFromRemoteDatabase(contact));

            await Task.WhenAll(removedContactTaskList).ConfigureAwait(false);

            var successfullyRemovedContactCount = removedContactTaskList.Count(x => x.Result.IsSuccessStatusCode);
            Assert.IsTrue(testContactList.Count == successfullyRemovedContactCount,
                $"Error Removing Test Data from Remote Database\n Found {testContactList.Count} Test Contacts and Removed {successfullyRemovedContactCount} Test Contacts");
        }

        async Task<List<ContactModel>> GetContactsFromRemoteDatabase()
        {
            var jsonString = await Client.GetStringAsync($"{BackendConstants.AzureAPIUrl}GetAllContacts").ConfigureAwait(false);
            return await Task.Run(() => JsonConvert.DeserializeObject<List<ContactModel>>(jsonString)).ConfigureAwait(false);
        }

        Task<HttpResponseMessage> RemoveContactFromRemoteDatabase(ContactModel contact)
        {
            var apiUrl = $"{BackendConstants.AzureFunctionUrl}RemoveItemFromDatabase/{contact.Id}?code={BackendConstants.AzureFunctionKey_RemoveItemFromDatabase}";

            return Client.PostAsync(apiUrl, null);
        }
        #endregion
    }
}

