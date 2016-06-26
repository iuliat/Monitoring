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
    builder.EntitySet<CPU>("CPUs");
    builder.EntitySet<Metrics>("Metrics"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CPUsController : ODataController
    {
        private PrincipalAPIContext db = new PrincipalAPIContext();

        // GET: odata/CPUs
        [EnableQuery]
        public IQueryable<CPU> GetCPUs()
        {
            return db.CPUs;
        }



        // GET: odata/CPUs(5)
        [EnableQuery]
        public SingleResult<CPU> GetCPU([FromODataUri] int key)
        {
            return SingleResult.Create(db.CPUs.Where(cPU => cPU.CPUID == key));
        }

        // PUT: odata/CPUs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<CPU> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CPU cPU = await db.CPUs.FindAsync(key);
            if (cPU == null)
            {
                return NotFound();
            }

            patch.Put(cPU);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CPUExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(cPU);
        }

        // POST: odata/CPUs
        public async Task<IHttpActionResult> Post(CPU cPU)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CPUs.Add(cPU);
            await db.SaveChangesAsync();

            return Created(cPU);
        }

        // PATCH: odata/CPUs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<CPU> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CPU cPU = await db.CPUs.FindAsync(key);
            if (cPU == null)
            {
                return NotFound();
            }

            patch.Patch(cPU);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CPUExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(cPU);
        }

        // DELETE: odata/CPUs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            CPU cPU = await db.CPUs.FindAsync(key);
            if (cPU == null)
            {
                return NotFound();
            }

            db.CPUs.Remove(cPU);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/CPUs(5)/Metrics
        [EnableQuery]
        public SingleResult<Metrics> GetMetrics([FromODataUri] int key)
        {
            return SingleResult.Create(db.CPUs.Where(m => m.CPUID == key).Select(m => m.Metrics));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CPUExists(int key)
        {
            return db.CPUs.Count(e => e.CPUID == key) > 0;
        }
    }
}
