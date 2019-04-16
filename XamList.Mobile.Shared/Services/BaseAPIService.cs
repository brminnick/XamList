using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Polly;
using Refit;

using XamList.Shared;

namespace XamList.Mobile.Shared
{
    abstract class BaseApiService<TApiService> where TApiService : BaseApiService<TApiService>
    {
        #region Constant Fields
        readonly static Lazy<TApiService> _instanceHolder = new Lazy<TApiService>();
        #endregion

        #region Constructors
        protected BaseApiService(string device)
        {
            XamListApiClient = RestService.For<IXamListAPI>(CreateHttpClient(BackendConstants.AzureAPIUrl, device));
            XamListFunctionsClient = RestService.For<IXamListFunction>(CreateHttpClient(BackendConstants.AzureFunctionUrl, device));
        }
        #endregion 

        #region Properties
        public static TApiService Instance => _instanceHolder.Value;

        protected IXamListAPI XamListApiClient { get; }
        protected IXamListFunction XamListFunctionsClient { get; }
        #endregion

        #region Methods
        public Task<List<ContactModel>> GetAllContactModels() => ExecutePollyHttpFunction(() => XamListApiClient.GetAllContactModels());
        public Task<ContactModel> GetContactModel(string id) => ExecutePollyHttpFunction(() => XamListApiClient.GetContactModel(id));
        public Task<ContactModel> PostContactModel(ContactModel contact) => ExecutePollyHttpFunction(() => XamListApiClient.PostContactModel(contact));
        public Task<ContactModel> PatchContactModel(ContactModel contact) => ExecutePollyHttpFunction(() => XamListApiClient.PatchContactModel(contact));
        public Task<HttpResponseMessage> DeleteContactModel(string id) => ExecutePollyHttpFunction(() => XamListApiClient.DeleteContactModel(id));
        public Task<HttpResponseMessage> GetHttpResponseMessage() => ExecutePollyHttpFunction(() => XamListApiClient.GetHttpResponse());
        public Task<HttpResponseMessage> RestoreDeletedContacts() => ExecutePollyHttpFunction(() => XamListFunctionsClient.RestoreDeletedContacts());
        public Task<ContactModel> RemoveContactFromRemoteDatabase(string id) => ExecutePollyHttpFunction(() => XamListFunctionsClient.RemoveContactFromRemoteDatabase(id));

        Task<T> ExecutePollyHttpFunction<T>(Func<Task<T>> action, int numRetries = 5)
        {
            return Policy
                    .Handle<WebException>()
                    .Or<HttpRequestException>()
                    .Or<TimeoutException>()
                    .WaitAndRetryAsync
                    (
                        numRetries,
                        pollyRetryAttempt
                    ).ExecuteAsync(action);

            TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromSeconds(Math.Pow(2, attemptNumber));
        }

        HttpClient CreateHttpClient(string baseAddress, string device)
        {
            HttpClient client;

            switch (device)
            {
                case "iOS":
                case "Android":
                    client = new HttpClient();
                    break;
                default:
                    client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
                    break;
            }

            client.BaseAddress = new Uri(baseAddress);

            return client;
        }
    }
    #endregion
}
