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
    builder.EntitySet<MasterVM>("MasterVMs");
    builder.EntitySet<Host>("Hosts"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class MasterVMsController : ODataController
    {
        private PrincipalAPIContext db = new PrincipalAPIContext();

        // GET: odata/MasterVMs
        [EnableQuery]
        public IQueryable<MasterVM> GetMasterVMs()
        {
            return db.MasterVMs;
        }

        // GET: odata/MasterVMs(5)
        [EnableQuery]
        public SingleResult<MasterVM> GetMasterVM([FromODataUri] int key)
        {
            return SingleResult.Create(db.MasterVMs.Where(masterVM => masterVM.MasterVMID == key));
        }

        // PUT: odata/MasterVMs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<MasterVM> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MasterVM masterVM = await db.MasterVMs.FindAsync(key);
            if (masterVM == null)
            {
                return NotFound();
            }

            patch.Put(masterVM);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MasterVMExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(masterVM);
        }

        // POST: odata/MasterVMs
        public async Task<IHttpActionResult> Post(MasterVM masterVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MasterVMs.Add(masterVM);
            await db.SaveChangesAsync();

            return Created(masterVM);
        }

        // PATCH: odata/MasterVMs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<MasterVM> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MasterVM masterVM = await db.MasterVMs.FindAsync(key);
            if (masterVM == null)
            {
                return NotFound();
            }

            patch.Patch(masterVM);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MasterVMExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(masterVM);
        }

        // DELETE: odata/MasterVMs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            MasterVM masterVM = await db.MasterVMs.FindAsync(key);
            if (masterVM == null)
            {
                return NotFound();
            }

            db.MasterVMs.Remove(masterVM);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/MasterVMs(5)/hosts
        [EnableQuery]
        public IQueryable<Host> Gethosts([FromODataUri] int key)
        {
            return db.MasterVMs.Where(m => m.MasterVMID == key).SelectMany(m => m.hosts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MasterVMExists(int key)
        {
            return db.MasterVMs.Count(e => e.MasterVMID == key) > 0;
        }

        public bool MasterVMExistsIP(String MasterVMIP)
        {
            return db.MasterVMs.Count(e => e.MasterVMIP == MasterVMIP) > 0;
        }

        
    }
}
