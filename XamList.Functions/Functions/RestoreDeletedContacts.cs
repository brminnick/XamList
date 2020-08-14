using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using XamList.Backend.Shared;

namespace XamList.Functions
{
    class RestoreDeletedContacts
    {
        readonly XamListDatabase _database;

        public RestoreDeletedContacts(XamListDatabase database) => _database = database;

        [FunctionName(nameof(RestoreDeletedContacts))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "RestoreDeletedContacts/")] HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var deletedContactModelList = _database.GetAllContactModels(x => x.IsDeleted);

                var undeletedContactModelList = deletedContactModelList.Select(x =>
                {
                    x.IsDeleted = false;
                    return x;
                }).ToList();

                foreach (var contact in undeletedContactModelList)
                    await _database.PatchContactModel(contact).ConfigureAwait(false);

                return new OkObjectResult($"Number of Deleted Contacts Restored: {undeletedContactModelList.Count}");
            }
            catch (System.Exception e)
            {
                log.LogError(e, e.Message);
                throw;
            }
        }
    }
}