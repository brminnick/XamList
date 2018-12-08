using System.Net.Http;
using System.Threading.Tasks;

using Refit;

using XamList.Shared;

namespace XamList.Mobile.Shared
{
    [Headers("Accept-Encoding", "gzip", "Accept", "application/json")]
    public interface IXamListFunction
    {
        [Post(@"/RestoreDeletedContacts")]
        Task<HttpResponseMessage> RestoreDeletedContacts();

        [Post(@"/RemoveItemFromDatabase/{id}")]
        Task<ContactModel> RemoveContactFromRemoteDatabase(string id);
    }
}
