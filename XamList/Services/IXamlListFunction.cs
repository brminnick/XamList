using System.Net.Http;
using System.Threading.Tasks;

using Refit;

using XamList.Mobile.Shared;

namespace XamList.Services
{
    [Headers("Accept-Encoding", "gzip", "Accept", "application/json")]
    public interface IXamlListFunction
    {
        [Post(@"RestoreDeletedContacts/?code=" + BackendConstants.AzureFunctionKey_RestoreDeletedContacts)]
        Task<HttpResponseMessage> RestoreDeletedContacts();
    }
}
