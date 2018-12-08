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
    static class APIService
    {
        #region Constant Fields
        readonly static Lazy<IXamListAPI> _xamListApiClientHolder = new Lazy<IXamListAPI>(() => RestService.For<IXamListAPI>(BackendConstants.AzureAPIUrl));
        readonly static Lazy<IXamListFunction> _xamListFunctionsClientHolder = new Lazy<IXamListFunction>(() => RestService.For<IXamListFunction>(BackendConstants.AzureFunctionUrl));
        #endregion

        #region Properties
        static IXamListAPI XamListApiClient => _xamListApiClientHolder.Value;
        static IXamListFunction XamListFunctionsClient => _xamListFunctionsClientHolder.Value;
        #endregion

        #region Methods
        public static Task<List<ContactModel>> GetAllContactModels() => ExecutePollyHttpFunction(() => XamListApiClient.GetAllContactModels());
        public static Task<ContactModel> GetContactModel(string id) => ExecutePollyHttpFunction(() => XamListApiClient.GetContactModel(id));
        public static Task<ContactModel> PostContactModel(ContactModel contact) => ExecutePollyHttpFunction(() => XamListApiClient.PostContactModel(contact));
        public static Task<ContactModel> PatchContactModel(ContactModel contact) => ExecutePollyHttpFunction(() => XamListApiClient.PatchContactModel(contact));
        public static Task<HttpResponseMessage> DeleteContactModel(string id) => ExecutePollyHttpFunction(() => XamListApiClient.DeleteContactModel(id));
        public static Task<HttpResponseMessage> RestoreDeletedContacts() => ExecutePollyHttpFunction(() => XamListFunctionsClient.RestoreDeletedContacts());
        public static Task<ContactModel> RemoveContactFromRemoteDatabase(string id) => ExecutePollyHttpFunction(() => XamListFunctionsClient.RemoveContactFromRemoteDatabase(id));

        static Task<T> ExecutePollyHttpFunction<T>(Func<Task<T>> action, int numRetries = 5)
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
