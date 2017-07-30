using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;

using XamList.Backend.Common;

namespace XamList.Functions
{
    public static class RestoreDeletedContacts
    {
        [FunctionName("RestoreDeletedContacts")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "RestoreDeletedContacts/")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var contactModelList = await XamListDatabase.GetAllContactModels();
            var deletedContactModelList = contactModelList.Where(x => x.IsDeleted);

            var undeletedContactModelList = deletedContactModelList.Select(x => { x.IsDeleted = true; return x; }).ToList();

            foreach (var contact in undeletedContactModelList)
                await XamListDatabase.PatchContactModel(contact);

            return req.CreateResponse(System.Net.HttpStatusCode.OK, $"Number of Deleted Contacts Restored: {undeletedContactModelList.Count}");
        }
    }
}