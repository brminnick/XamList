using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using XamList.Shared;
using XamList.Mobile.Shared;
using XamList.Services;

using Polly;
using Refit;

namespace XamList
{
    static class APIService
    {
        #region Constant Fields
        readonly static Lazy<IXamListAPI> _xamListApiClientHolder = new Lazy<IXamListAPI>(() => RestService.For<IXamListAPI>(BackendConstants.AzureAPIUrl));
        readonly static Lazy<IXamlListFunction> _xamListFunctionsClientHolder = new Lazy<IXamlListFunction>(() => RestService.For<IXamlListFunction>(BackendConstants.AzureFunctionUrl));
        #endregion

        #region Properties
        static IXamListAPI XamListApiClient => _xamListApiClientHolder.Value;
        static IXamlListFunction XamListFunctionsClient => _xamListFunctionsClientHolder.Value;
        #endregion

        #region Methods
        public static Task<List<ContactModel>> GetAllContactModels() => ExecutePollyFunction(() => XamListApiClient.GetAllContactModels());
        public static Task<ContactModel> GetContactModel(string id) => ExecutePollyFunction(() => XamListApiClient.GetContactModel(id));
        public static Task<ContactModel> PostContactModel(ContactModel contact) => ExecutePollyFunction(() => XamListApiClient.PostContactModel(contact));
        public static Task<ContactModel> PatchContactModel(ContactModel contact) => ExecutePollyFunction(() => XamListApiClient.PatchContactModel(contact));
        public static Task<HttpResponseMessage> DeleteContactModel(string id) => ExecutePollyFunction(() => XamListApiClient.DeleteContactModel(id));
        public static Task<HttpResponseMessage> RestoreDeletedContacts() => ExecutePollyFunction(() => XamListFunctionsClient.RestoreDeletedContacts());

        static Task<T> ExecutePollyFunction<T>(Func<Task<T>> action, int numRetries = 5)
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
        #endregion
    }
}
