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

			Task.Run(async () => await RemoveTestContactsFromDatabases()).Wait();

            ContactsListPage.PullToRefresh();
        }

		[TearDown]
		protected virtual void AfterEachTest()
		{
			Task.Run(async () => await RemoveTestContactsFromDatabases()).Wait();
		}

        static HttpClient CreateHttpClient()
        {
            var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip })
            {
                Timeout = System.TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            return client;
        }

        async Task RemoveTestContactsFromDatabases()
        {
            BackdoorMethodHelpers.RemoveTestContactsFromLocalDatabase(App);
			await RemoveTestContactsFromRemoteDatabase();
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
                $"Error Removing Test Data from Remote Datase\n Found {testContactList.Count} Test Contacts and Removed {successfullyRemovedContactCount} Test Contacts");

        }

        async Task<List<ContactModel>> GetContactsFromRemoteDatabase()
        {
            var jsonString = await Client.GetStringAsync("https://xamlistapi.azurewebsites.net/api/GetAllContacts");
            return JsonConvert.DeserializeObject<List<ContactModel>>(jsonString);
        }

        async Task<HttpResponseMessage> RemoveContactFromRemoteDatabase(ContactModel contact)
        {
            var apiUrl = $"https://xamlistfunctions.azurewebsites.net/api/RemoveItemFromDatabase/{contact.Id}?code=qZGcFbpqxBTpdz4K0f45m81qS9eHzSOMBWjGH5o1SfH8cycnYbaf3Q==";

            return await Client.PostAsync(apiUrl, null).ConfigureAwait(false);
        }


        #endregion
    }
}

