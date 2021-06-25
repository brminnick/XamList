using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using XamList.Shared;

namespace XamList.Mobile.Shared
{
    public class ApiService
    {
        readonly IXamListAPI _apiClient;
        readonly IXamListFunction _functionsClient;

        public ApiService(IXamListAPI xamListAPI, IXamListFunction xamListFunction)
        {
            _apiClient = xamListAPI;
            _functionsClient = xamListFunction;
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
    }
}
