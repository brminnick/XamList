using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Threading.Tasks;
using System.Collections.Generic;

using XamList.Shared;

namespace XamList.API.Controllers
{
    public class XamListApiController : ApiController
    {
        [HttpGet, Route("api/GetAllContacts"), EnableQuery]
        public async Task<IList<ContactModel>> GetAllContacts() =>
             await XamListDatabase.GetAllContactModels();

        [HttpGet, Route("api/GetContact")]
        public async Task<ContactModel> GetContact(string id) =>
             await XamListDatabase.GetContactModel(id);

        [HttpPost, Route("api/PostContact")]
        public async Task<HttpResponseMessage> PostContact(ContactModel contact)
        {
            try
            {
                var contactFromDatabase = await XamListDatabase.InsertContactModel(contact);
                return Request.CreateResponse(HttpStatusCode.Created, contactFromDatabase);
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = e.Message };
            }
        }

        [HttpPatch, Route("api/PatchContact/{id}")]
        public async Task<HttpResponseMessage> PatchContact(string id, Delta<ContactModel> contact)
        {
            try
            {
                var contactFromDatabase = await XamListDatabase.PatchContactModel(id, contact);
                return Request.CreateResponse(HttpStatusCode.OK, contactFromDatabase);
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = e.Message };
            }
        }

        [HttpDelete, Route("api/DeleteContact/{id}")]
        public async Task<HttpResponseMessage> DeleteContact(string id)
        {
            try
            {
                var contactFromDatabase = await XamListDatabase.DeleteContactModel(id);
                return Request.CreateResponse(HttpStatusCode.OK, contactFromDatabase);
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = e.Message };
            }
        }
    }
}
