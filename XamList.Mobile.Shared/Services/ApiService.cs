using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Refit;
using Xamarin.Essentials;
using XamList.Shared;

namespace XamList.Mobile.Shared
{
    static class ApiService
    {
        readonly static Lazy<IXamListAPI> _xamListApiClientHolder = new Lazy<IXamListAPI>(() => RestService.For<IXamListAPI>(CreateHttpClient(BackendConstants.AzureAPIUrl)));
        readonly static Lazy<IXamListFunction> _xamListFunctionsClientHolder = new Lazy<IXamListFunction>(() => RestService.For<IXamListFunction>(CreateHttpClient(BackendConstants.AzureFunctionUrl)));

        static IXamListAPI XamListApiClient => _xamListApiClientHolder.Value;
        static IXamListFunction XamListFunctionsClient => _xamListFunctionsClientHolder.Value;

        public static Task<List<ContactModel>> GetAllContactModels() => AttemptAndRetry(() => XamListApiClient.GetAllContactModels());
        public static Task<ContactModel> GetContactModel(string id) => AttemptAndRetry(() => XamListApiClient.GetContactModel(id));
        public static Task<ContactModel> PostContactModel(ContactModel contact) => AttemptAndRetry(() => XamListApiClient.PostContactModel(contact));
        public static Task<ContactModel> PatchContactModel(ContactModel contact) => AttemptAndRetry(() => XamListApiClient.PatchContactModel(contact));
        public static Task<HttpResponseMessage> DeleteContactModel(string id) => AttemptAndRetry(() => XamListApiClient.DeleteContactModel(id));
        public static Task<HttpResponseMessage> GetHttpResponseMessage() => AttemptAndRetry(() => XamListApiClient.GetHttpResponse());
        public static Task<HttpResponseMessage> RestoreDeletedContacts() => AttemptAndRetry(() => XamListFunctionsClient.RestoreDeletedContacts());
        public static Task<ContactModel> RemoveContactFromRemoteDatabase(string id) => AttemptAndRetry(() => XamListFunctionsClient.RemoveContactFromRemoteDatabase(id));

        static Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries = 2)
        {
            return Policy.Handle<Exception>().WaitAndRetryAsync(numRetries, pollyRetryAttempt).ExecuteAsync(action);

            static TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromSeconds(Math.Pow(2, attemptNumber));
        }

        static HttpClient CreateHttpClient(string baseAddress)
        {
            HttpClient client;

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.Android)
                client = new HttpClient();
            else
                client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

            client.BaseAddress = new Uri(baseAddress);

            return client;
        }
    }
}
