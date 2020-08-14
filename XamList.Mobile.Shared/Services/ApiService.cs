using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Refit;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using XamList.Shared;

namespace XamList.Mobile.Shared
{
    public class ApiService
    {
        readonly IDeviceInfo _deviceInfo;
        readonly IXamListAPI _apiClient;
        readonly IXamListFunction _functionsClient;

        public ApiService(IDeviceInfo deviceInfo)
        {
            _deviceInfo = deviceInfo;
            _apiClient = RestService.For<IXamListAPI>(CreateHttpClient(BackendConstants.AzureAPIUrl));
            _functionsClient = RestService.For<IXamListFunction>(CreateHttpClient(BackendConstants.AzureFunctionUrl));
        }

        public Task<List<ContactModel>> GetAllContactModels() => AttemptAndRetry(() => _apiClient.GetAllContactModels());
        public Task<ContactModel> GetContactModel(string id) => AttemptAndRetry(() => _apiClient.GetContactModel(id));
        public Task<ContactModel> PostContactModel(ContactModel contact) => AttemptAndRetry(() => _apiClient.PostContactModel(contact));
        public Task<ContactModel> PatchContactModel(ContactModel contact) => AttemptAndRetry(() => _apiClient.PatchContactModel(contact));
        public Task<HttpResponseMessage> DeleteContactModel(string id) => AttemptAndRetry(() => _apiClient.DeleteContactModel(id));
        public Task<HttpResponseMessage> GetHttpResponseMessage() => AttemptAndRetry(() => _apiClient.GetHttpResponse());
        public Task<HttpResponseMessage> RestoreDeletedContacts() => AttemptAndRetry(() => _functionsClient.RestoreDeletedContacts());
        public Task<ContactModel> RemoveContactFromRemoteDatabase(string id) => AttemptAndRetry(() => _functionsClient.RemoveContactFromRemoteDatabase(id));

        static Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries = 2)
        {
            return Policy.Handle<Exception>().WaitAndRetryAsync(numRetries, pollyRetryAttempt).ExecuteAsync(action);

            static TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromSeconds(Math.Pow(2, attemptNumber));
        }

        HttpClient CreateHttpClient(string baseAddress)
        {
            HttpClient client;

            if (_deviceInfo.Platform == DevicePlatform.iOS || _deviceInfo.Platform == DevicePlatform.Android)
                client = new HttpClient();
            else
                client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

            client.BaseAddress = new Uri(baseAddress);

            return client;
        }
    }
}
