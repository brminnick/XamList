using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XamList.Backend.Shared;
using XamList.Shared;

namespace XamList.API.Controllers
{
    [ApiController, Route("api")]
    public class XamListApiController : ControllerBase
    {
        [HttpGet, Route(nameof(GetAllContacts))]
        public IActionResult GetAllContacts() => new OkObjectResult(XamListDatabase.GetAllContactModels());

        [HttpGet, Route(nameof(GetContact))]
        public IActionResult GetContact(string id)
        {
            var contactModel = XamListDatabase.GetContactModel(id);
            return new OkObjectResult(contactModel);
        }

        [HttpPost, Route(nameof(PostContact))]
        public async Task<IActionResult> PostContact([FromBody]ContactModel contact)
        {
            var contactFromDatabase = await XamListDatabase.InsertContactModel(contact).ConfigureAwait(false);
            return new CreatedResult("", contactFromDatabase);
        }

        [HttpPatch, Route(nameof(PatchContact))]
        public async Task<IActionResult> PatchContact([FromBody]ContactModel contact)
        {
            var contactFromDatabase = await XamListDatabase.PatchContactModel(contact).ConfigureAwait(false);
            return new OkObjectResult(contactFromDatabase);
        }

        [HttpDelete, Route(nameof(DeleteContact))]
        public async Task<IActionResult> DeleteContact(string id)
        {
            var contactFromDatabase = await XamListDatabase.DeleteContactModel(id).ConfigureAwait(false);
            return new OkObjectResult(contactFromDatabase);
        }
    }
}
