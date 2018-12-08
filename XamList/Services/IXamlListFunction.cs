using System.Net.Http;
using System.Threading.Tasks;

using Refit;

namespace XamList
{
    [Headers("Accept-Encoding", "gzip", "Accept", "application/json")]
    public interface IXamlListFunction
    {
        [Post(@"/RestoreDeletedContacts")]
        Task<HttpResponseMessage> RestoreDeletedContacts();
    }
}
