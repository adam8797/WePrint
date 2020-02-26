using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using WePrint.Common.ServiceDiscovery;
using WePrint.Models;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/address")]
    public class AddressController : Controller
    {
        private readonly ILogger<AddressController> _logger;
        private readonly IServiceDiscovery _discovery;
        private readonly IAsyncDocumentSession _session;

        public AddressController(ILogger<AddressController> logger, IServiceDiscovery discovery, IAsyncDocumentSession session)
        {
            _logger = logger;
            _session = session;
            _discovery = discovery;
        }


        // GET: /api/Address/
        [HttpGet]
        public async Task<IActionResult> GetAddress()
        {
            return Json("Under Construction");
        }

        // GET" /api/Address/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressByID([FromRoute]string id)
        {
            var user = await this.GetCurrentUser(_session);
            if (user == null)
                return this.FailWith("Not logged in", HttpStatusCode.Unauthorized);

            if (id != null)
                return await this.QueryItemById<AddressModel>(_session, id);


            return this.FailWith("No ID provided", HttpStatusCode.ExpectationFailed);
        }

        // POST: /api/Address/
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody]AddressUpdateModel createUpdate)
        {
            ApplicationUser currentUser = await this.GetCurrentUser(_session);
            if(currentUser == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Json(new { err = "Not logged in" });
            }

            var newAddress = new AddressModel()
            {
                StreetAddress = createUpdate.StreetAddress,
                City = createUpdate.City,
                State = createUpdate.State,
                ZipCode = (int)createUpdate.ZipCode
            };

            try 
            {
                await _session.StoreAsync(newAddress);
                await _session.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return this.FailWith(new { err = "Uncaught Exception", details = e.ToString()}, HttpStatusCode.InternalServerError);
            }

            return Json(newAddress.Id);
        }

        // PUT: /api/Address/
        [HttpPut]
        public async Task<IActionResult> UpdateAddress([FromBody]AddressUpdateModel update)
        {
            ApplicationUser currentUser = await this.GetCurrentUser(_session);
            if(currentUser == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Json(new { err = "Not logged in" });
            }

            var addresses = await _session.Query<AddressModel>().Where(address => address.Id == update.Id).ToArrayAsync();

            if (addresses.Length == 0)
                return this.FailWith("No address with id " + update.Id, HttpStatusCode.NotFound);

            if (addresses.Length > 1)
                return this.FailWith("More than one address with ID " + update.Id, HttpStatusCode.Conflict);

            var address = addresses[0];

            address.ApplyChanges(update);

            try 
            {
                await _session.StoreAsync(address);
                await _session.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return this.FailWith(new { err = "Uncaught Exception", details = e.ToString()}, HttpStatusCode.InternalServerError);
            }

            return Json(address.Id);
        }

        // DELETE: /api/Address/
        [HttpDelete]
        public async Task<IActionResult> DeleteAddress([FromBody] string addrId)
        {
            ApplicationUser currentUser = await this.GetCurrentUser(_session);
            if(currentUser == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Json(new { err = "Not logged in" });
            }

            var addresses = await _session.Query<AddressModel>().Where(address => address.Id == addrId).ToArrayAsync();

            if (addresses.Length == 0)
                return this.FailWith("No address with id " + addrId, HttpStatusCode.NotFound);

            if (addresses.Length > 1)
                return this.FailWith("More than one address with ID " + addrId, HttpStatusCode.Conflict);

            var address = addresses[0];

            try
            {
                _session.Delete(address);
                await _session.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return this.FailWith(new { err = "Uncaught Exception", details = e.ToString()}, HttpStatusCode.InternalServerError);
            }

            return Json(address.Id);
        }
    }
}
