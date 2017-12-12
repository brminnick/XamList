using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Newtonsoft.Json;

using NUnit.Framework;

using XamList.Shared;

namespace XamList.Tests.Shared
{
    public abstract class BaseTest
    {
        #region constant Fields
        readonly Lazy<HttpClient> _clientHolder = new Lazy<HttpClient>(CreateHttpClient);
        #endregion

        #region Properties
        HttpClient Client => _clientHolder.Value;
        #endregion

        #region Methods
        [SetUp]
        protected virtual void BeforeEachTest() =>
            Task.Run(async () => await RemoveTestContactsFromRemoteDatabase()).GetAwaiter().GetResult();

        [TearDown]
        protected virtual void AfterEachTest() =>
            Task.Run(async () => await RemoveTestContactsFromRemoteDatabase()).GetAwaiter().GetResult();

        static HttpClient CreateHttpClient()
        {
            var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip })
            {
                Timeout = System.TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            return client;
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
