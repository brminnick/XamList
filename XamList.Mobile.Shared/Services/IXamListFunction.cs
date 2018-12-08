using System.Net.Http;
using System.Threading.Tasks;

using Refit;

namespace XamList.Mobile.Shared
{
    [Headers("Accept-Encoding", "gzip", "Accept", "application/json")]
    public interface IXamListFunction
    {
        [Post(@"/RestoreDeletedContacts")]
        Task<HttpResponseMessage> RestoreDeletedContacts();

        [Post(@"/RemoveItemFromDatabase/{id}")]
        Task<HttpResponseMessage> RemoveContactFromRemoteDatabase(string id);
    }
}
