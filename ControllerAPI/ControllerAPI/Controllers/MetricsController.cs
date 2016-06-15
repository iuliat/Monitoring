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
using ControllerAPI.Models;

namespace ControllerAPI.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using ControllerAPI.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Metrics>("Metrics");
    builder.EntitySet<CPU>("CPUs"); 
    builder.EntitySet<Host>("Hosts"); 
    builder.EntitySet<RAM>("RAMs"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class MetricsController : ODataController
    {
        private ControllerAPIContext db = new ControllerAPIContext();

        // GET: odata/Metrics
        [EnableQuery]
        public IQueryable<Metrics> GetMetrics()
        {
            return db.Metrics;
        }

        // GET: odata/Metrics(5)
        [EnableQuery]
        public SingleResult<Metrics> GetMetrics([FromODataUri] int key)
        {
            return SingleResult.Create(db.Metrics.Where(metrics => metrics.MetricID == key));
        }

        // PUT: odata/Metrics(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Metrics> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Metrics metrics = await db.Metrics.FindAsync(key);
            if (metrics == null)
            {
                return NotFound();
            }

            patch.Put(metrics);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MetricsExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(metrics);
        }

        // POST: odata/Metrics
        public async Task<IHttpActionResult> Post(Metrics metrics)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Metrics.Add(metrics);
            await db.SaveChangesAsync();

            return Created(metrics);
        }

        // PATCH: odata/Metrics(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Metrics> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Metrics metrics = await db.Metrics.FindAsync(key);
            if (metrics == null)
            {
                return NotFound();
            }

            patch.Patch(metrics);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MetricsExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(metrics);
        }

        // DELETE: odata/Metrics(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Metrics metrics = await db.Metrics.FindAsync(key);
            if (metrics == null)
            {
                return NotFound();
            }

            db.Metrics.Remove(metrics);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Metrics(5)/CPUs
        [EnableQuery]
        public IQueryable<CPU> GetCPUs([FromODataUri] int key)
        {
            return db.Metrics.Where(m => m.MetricID == key).SelectMany(m => m.CPUs);
        }

        // GET: odata/Metrics(5)/hosts
        [EnableQuery]
        public IQueryable<Host> Gethosts([FromODataUri] int key)
        {
            return db.Metrics.Where(m => m.MetricID == key).SelectMany(m => m.hosts);
        }

        // GET: odata/Metrics(5)/RAMs
        [EnableQuery]
        public IQueryable<RAM> GetRAMs([FromODataUri] int key)
        {
            return db.Metrics.Where(m => m.MetricID == key).SelectMany(m => m.RAMs);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MetricsExists(int key)
        {
            return db.Metrics.Count(e => e.MetricID == key) > 0;
        }
    }
}
