using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using XamList.Backend.Shared;
using XamList.Shared;

namespace XamList.API.Controllers
{
    public class XamListApiController : Controller
    {
        [HttpGet, Route("api/GetAllContacts")]
        public async Task<IActionResult> GetAllContacts()
        {
            var contactList = await XamListDatabase.GetAllContactModels().ConfigureAwait(false);
            return new OkObjectResult(contactList);
        }

        [HttpGet, Route("api/GetContact/{id}")]
        public IActionResult GetContact(string id)
        {
            var contactModel = XamListDatabase.GetContactModel(id);
            return new OkObjectResult(contactModel);
        }

        [HttpPost, Route("api/PostContact")]
        public async Task<IActionResult> PostContact([FromBody]ContactModel contact)
        {
            var contactFromDatabase = await XamListDatabase.InsertContactModel(contact).ConfigureAwait(false);
            return new CreatedResult("", contactFromDatabase);
        }

        [HttpPatch, Route("api/PatchContact")]
        public async Task<IActionResult> PatchContact([FromBody]ContactModel contact)
        {
            var contactFromDatabase = await XamListDatabase.PatchContactModel(contact).ConfigureAwait(false);
            return new OkObjectResult(contactFromDatabase);
        }

        [HttpDelete, Route("api/DeleteContact/{id}")]
        public async Task<IActionResult> DeleteContact(string id)
        {
            var contactFromDatabase = await XamListDatabase.DeleteContactModel(id).ConfigureAwait(false);
            return new OkObjectResult(contactFromDatabase);
        }
    }
}
