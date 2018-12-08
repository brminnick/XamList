using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using XamList.Backend.Shared;

namespace XamList.Functions
{
    public static class RemoveItemFromDatabase
    {
        [FunctionName(nameof(RemoveItemFromDatabase))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "RemoveItemFromDatabase/{id}")]HttpRequest req, string id, ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                var contactDeleted = await XamListDatabase.RemoveContactModel(id).ConfigureAwait(false);

                if (contactDeleted is null)
                    return new BadRequestResult();

                return new OkObjectResult(contactDeleted);
            }
            catch(System.Exception e)
            {
                log.LogError(e, e.Message);
                throw;
            }
        }
    }
}