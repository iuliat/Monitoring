using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using PrincipalAPI.Models;

namespace PrincipalAPI.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using PrincipalAPI.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Host>("Hosts");
    builder.EntitySet<VMMon.PrincipalAPI>("Controllers"); 
    builder.EntitySet<Metrics>("Metrics"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class HostsController : ODataController
    {
        private PrincipalAPIContext db = new PrincipalAPIContext();

        // GET: odata/Hosts
        [EnableQuery]
        public IQueryable<Host> GetHosts()
        {
            return db.Hosts;
        }

        // GET: odata/Hosts(5)
        [EnableQuery]
        public SingleResult<Host> GetHost([FromODataUri] int key)
        {
            return SingleResult.Create(db.Hosts.Where(host => host.HostID == key));
        }

        // PUT: odata/Hosts(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Host> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Host host = await db.Hosts.FindAsync(key);
            if (host == null)
            {
                return NotFound();
            }

            patch.Put(host);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HostExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(host);
        }

        // POST: odata/Hosts
        public async Task<IHttpActionResult> Post(Host host)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Hosts.Add(host);
            await db.SaveChangesAsync();

            return Created(host);
        }

        // PATCH: odata/Hosts(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Host> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Host host = await db.Hosts.FindAsync(key);
            if (host == null)
            {
                return NotFound();
            }

            patch.Patch(host);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HostExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(host);
        }

        // DELETE: odata/Hosts(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Host host = await db.Hosts.FindAsync(key);
            if (host == null)
            {
                return NotFound();
            }

            db.Hosts.Remove(host);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Hosts(5)/VMMon.PrincipalAPI
        [EnableQuery]
        public SingleResult<Controller> GetController([FromODataUri] int key)
        {
            return SingleResult.Create(db.Hosts.Where(m => m.HostID == key).Select(m => m.Controller));
        }

        // GET: odata/Hosts(5)/Metrics
        [EnableQuery]
        public SingleResult<Metrics> GetMetrics([FromODataUri] int key)
        {
            return SingleResult.Create(db.Hosts.Where(m => m.HostID == key).Select(m => m.Metrics));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HostExists(int key)
        {
            return db.Hosts.Count(e => e.HostID == key) > 0;
        }
    }
}
