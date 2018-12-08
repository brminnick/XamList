using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Refit;

using XamList.Shared;

namespace XamList.Services
{
    [Headers("Accept-Encoding", "gzip", "Accept", "application/json")]
    public interface IXamListAPI
    {
        [Get(@"/GetAllContacts")]
        Task<List<ContactModel>> GetAllContactModels();

        [Get(@"/GetContact/{id}")]
        Task<ContactModel> GetContactModel(string id);

        [Post(@"/PostContact")]
        Task<ContactModel> PostContactModel([Body]ContactModel contact);

        [Patch(@"/PatchContact")]
        Task<ContactModel> PatchContactModel([Body]ContactModel contact);

        [Delete(@"/DeleteContact/{id}")]
        Task<HttpResponseMessage> DeleteContactModel(string id);
    }
}
