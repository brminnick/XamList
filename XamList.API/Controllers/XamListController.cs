using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace XamList.API.Controllers
{
    public class XamListController : ApiController
    {
        [HttpGet, Route("api/GetAllContacts")]
        public async Task<List<ContactModel>> GetAllContacts() =>
             await Database.XamListDatabase.GetAllContactModels();
    }
}
