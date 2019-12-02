using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XamList.Backend.Shared;
using XamList.Shared;

namespace XamList.API.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class XamListApiController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            var contactList = await XamListDatabase.GetAllContactModels().ConfigureAwait(false);
            return new OkObjectResult(contactList);
        }

        [HttpGet]
        public IActionResult GetContact(string id)
        {
            var contactModel = XamListDatabase.GetContactModel(id);
            return new OkObjectResult(contactModel);
        }

        [HttpPost]
        public async Task<IActionResult> PostContact([FromBody]ContactModel contact)
        {
            var contactFromDatabase = await XamListDatabase.InsertContactModel(contact).ConfigureAwait(false);
            return new CreatedResult("", contactFromDatabase);
        }

        [HttpPatch]
        public async Task<IActionResult> PatchContact([FromBody]ContactModel contact)
        {
            var contactFromDatabase = await XamListDatabase.PatchContactModel(contact).ConfigureAwait(false);
            return new OkObjectResult(contactFromDatabase);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteContact(string id)
        {
            var contactFromDatabase = await XamListDatabase.DeleteContactModel(id).ConfigureAwait(false);
            return new OkObjectResult(contactFromDatabase);
        }
    }
}
