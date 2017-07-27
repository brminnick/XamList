using System.Collections.Generic;
using System.Web.Http;
using System.Threading.Tasks;

using XamList.Shared;

namespace XamList.API.Controllers
{
    public class XamListController : ApiController
    {
        [HttpGet, Route("api/GetAllContacts")]
        public async Task<List<ContactModel>> GetAllContacts() =>
             await Database.XamListDatabase.GetAllContactModels();
    }
}
