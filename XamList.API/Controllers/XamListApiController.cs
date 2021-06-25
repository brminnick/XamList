using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XamList.Backend.Shared;
using XamList.Shared;

namespace XamList.API.Controllers
{
    [ApiController, Route("api")]
    class XamListApiController : ControllerBase
    {
        readonly XamListDatabaseService _database;

        public XamListApiController(XamListDatabaseService database) => _database = database;

        [HttpGet, Route(nameof(GetAllContacts))]
        public IActionResult GetAllContacts() => new OkObjectResult(_database.GetAllContactModels());

        [HttpGet, Route(nameof(GetContact))]
        public IActionResult GetContact(string id)
        {
            var contactModel = _database.GetContactModel(id);
            return new OkObjectResult(contactModel);
        }

        [HttpPost, Route(nameof(PostContact))]
        public async Task<IActionResult> PostContact([FromBody] ContactModel contact)
        {
            var contactFromDatabase = await _database.InsertContactModel(contact).ConfigureAwait(false);
            return new CreatedResult("", contactFromDatabase);
        }

        [HttpPatch, Route(nameof(PatchContact))]
        public async Task<IActionResult> PatchContact([FromBody] ContactModel contact)
        {
            var contactFromDatabase = await _database.PatchContactModel(contact).ConfigureAwait(false);
            return new OkObjectResult(contactFromDatabase);
        }

        [HttpDelete, Route(nameof(DeleteContact))]
        public async Task<IActionResult> DeleteContact(string id)
        {
            var contactFromDatabase = await _database.DeleteContactModel(id).ConfigureAwait(false);
            return new OkObjectResult(contactFromDatabase);
        }
    }
}
