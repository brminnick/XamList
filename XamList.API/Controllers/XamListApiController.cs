using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using XamList.Shared;
using System.Collections.Generic;

namespace XamList.API.Controllers
{
    public class XamListApiController : ApiController
    {
        [HttpGet, Route("api/GetAllContacts")]
        public async Task<IList<ContactModel>> GetAllContacts() =>
             await Database.XamListDatabase.GetAllContactModels();

        [HttpGet, Route("api/GetContact")]
        public async Task<ContactModel> GetContact(string id) =>
             await Database.XamListDatabase.GetContactModel(id);

        [HttpPost, Route("api/PostContact")]
        public async Task<HttpResponseMessage> PostContact(ContactModel contact)
        {
            try
            {
                await Database.XamListDatabase.InsertContactModel(contact);
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = e.Message
                };
            }
        }

        [HttpPatch, Route("api/PatchContact")]
        public async Task<HttpResponseMessage> PatchContact(ContactModel contact)
        {
            try
            {
                await Database.XamListDatabase.PatchContactModel(contact);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = e.Message
                };
            }
        }

        [HttpDelete, Route("api/DeleteContact")]
        public async Task<HttpResponseMessage> DeleteContact(string id)
        {
            try
            {
                await Database.XamListDatabase.DeleteContactModel(id);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = e.Message
                };
            }
        }
    }
}
