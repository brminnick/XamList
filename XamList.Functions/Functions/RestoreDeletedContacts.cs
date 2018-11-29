using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using XamList.Backend.Shared;

namespace XamList.Functions
{
    public static class RestoreDeletedContacts
    {
        [FunctionName(nameof(RestoreDeletedContacts))]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "RestoreDeletedContacts/")]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var contactModelList = await XamListDatabase.GetAllContactModels().ConfigureAwait(false);
            var deletedContactModelList = contactModelList.Where(x => x.IsDeleted);

            var undeletedContactModelList = deletedContactModelList.Select(x => { x.IsDeleted = false; return x; }).ToList();

            foreach (var contact in undeletedContactModelList)
                await XamListDatabase.PatchContactModel(contact).ConfigureAwait(false);

            return req.CreateResponse(System.Net.HttpStatusCode.OK, $"Number of Deleted Contacts Restored: {undeletedContactModelList.Count}");
        }
    }
}