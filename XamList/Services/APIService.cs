using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using XamList.Shared;

namespace XamList
{
    abstract class APIService : BaseHttpClientService
    {
        #region Methods
        public static Task<List<ContactModel>> GetAllContactModels() =>
            GetDataObjectFromAPI<List<ContactModel>>($"{BackendConstants.AzureAPIUrl}GetAllContacts");

        public static Task<ContactModel> GetContactModel(ContactModel contact) =>
            GetDataObjectFromAPI<ContactModel, string>($"{BackendConstants.AzureAPIUrl}GetContact", contact.Id);

        public static Task<HttpResponseMessage> PostContactModel(ContactModel contact) =>
            PostObjectToAPI($"{BackendConstants.AzureAPIUrl}PostContact", contact);

        public static Task<HttpResponseMessage> PatchContactModel(ContactModel contact) =>
            PatchObjectToAPI($"{BackendConstants.AzureAPIUrl}PatchContact/{contact.Id}", contact);

        public static Task<HttpResponseMessage> DeleteContactModel(ContactModel contact) =>
            DeleteObjectFromAPI($"{BackendConstants.AzureAPIUrl}DeleteContact/{contact.Id}");

        public static Task<HttpResponseMessage> RestoreDeletedContacts() =>
            PostObjectToAPI($"{BackendConstants.AzureFunctionUrl}RestoreDeletedContacts/?code={BackendConstants.AzureFunctionKey_RestoreDeletedContacts}", new object());
        #endregion
    }
}
