using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;

using XamList.Backend.Common;

namespace XamList.Functions
{
    public static class RemoveItemFromDatabase
    {
        [FunctionName(nameof(RemoveItemFromDatabase))]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "RemoveItemFromDatabase/{id}")]HttpRequestMessage req, string id, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

			var contactDeleted = await XamListDatabase.RemoveContactModel(id).ConfigureAwait(false);

            if(contactDeleted == null)
                return req.CreateResponse(System.Net.HttpStatusCode.BadRequest, $"Contact Id not found: Id: {id}");

            return req.CreateResponse(System.Net.HttpStatusCode.OK, contactDeleted);
        }
    }
}