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
    builder.EntitySet<RAM>("RAMs");
    builder.EntitySet<Metrics>("Metrics"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class RAMsController : ODataController
    {
        private PrincipalAPIContext db = new PrincipalAPIContext();

        // GET: odata/RAMs
        [EnableQuery]
        public IQueryable<RAM> GetRAMs()
        {
            return db.RAMs;
        }

        // GET: odata/RAMs(5)
        [EnableQuery]
        public SingleResult<RAM> GetRAM([FromODataUri] int key)
        {
            return SingleResult.Create(db.RAMs.Where(rAM => rAM.RAMID == key));
        }

        // PUT: odata/RAMs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<RAM> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RAM rAM = await db.RAMs.FindAsync(key);
            if (rAM == null)
            {
                return NotFound();
            }

            patch.Put(rAM);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RAMExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(rAM);
        }

        // POST: odata/RAMs
        public async Task<IHttpActionResult> Post(RAM rAM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RAMs.Add(rAM);
            await db.SaveChangesAsync();

            return Created(rAM);
        }

        // PATCH: odata/RAMs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<RAM> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RAM rAM = await db.RAMs.FindAsync(key);
            if (rAM == null)
            {
                return NotFound();
            }

            patch.Patch(rAM);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RAMExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(rAM);
        }

        // DELETE: odata/RAMs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            RAM rAM = await db.RAMs.FindAsync(key);
            if (rAM == null)
            {
                return NotFound();
            }

            db.RAMs.Remove(rAM);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/RAMs(5)/Metrics
        [EnableQuery]
        public SingleResult<Metrics> GetMetrics([FromODataUri] int key)
        {
            return SingleResult.Create(db.RAMs.Where(m => m.RAMID == key).Select(m => m.Metrics));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RAMExists(int key)
        {
            return db.RAMs.Count(e => e.RAMID == key) > 0;
        }
    }
}
