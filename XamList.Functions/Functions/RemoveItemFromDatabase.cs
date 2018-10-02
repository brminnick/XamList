using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using XamList.Backend.Shared;

namespace XamList.Functions
{
    public static class RemoveItemFromDatabase
    {
        [FunctionName(nameof(RemoveItemFromDatabase))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "RemoveItemFromDatabase/{id}")]HttpRequest req, string id, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

			var contactDeleted = await XamListDatabase.RemoveContactModel(id).ConfigureAwait(false);

            if (contactDeleted is null)
                return new BadRequestResult();

            return new OkObjectResult(contactDeleted);
        }
    }
}