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
    builder.EntitySet<Controller>("Controllers");
    builder.EntitySet<Host>("Hosts"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ControllersController : ODataController
    {
        private ControllerAPIContext db = new ControllerAPIContext();

        // GET: odata/Controllers
        [EnableQuery]
        public IQueryable<Controller> GetControllers()
        {
            return db.Controllers;
        }

        // GET: odata/Controllers(5)
        [EnableQuery]
        public SingleResult<Controller> GetController([FromODataUri] int key)
        {
            return SingleResult.Create(db.Controllers.Where(controller => controller.ControllerID == key));
        }

        // PUT: odata/Controllers(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Controller> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Controller controller = await db.Controllers.FindAsync(key);
            if (controller == null)
            {
                return NotFound();
            }

            patch.Put(controller);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ControllerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(controller);
        }

        // POST: odata/Controllers
        public async Task<IHttpActionResult> Post(Controller controller)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Controllers.Add(controller);
            await db.SaveChangesAsync();

            return Created(controller);
        }

        // PATCH: odata/Controllers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Controller> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Controller controller = await db.Controllers.FindAsync(key);
            if (controller == null)
            {
                return NotFound();
            }

            patch.Patch(controller);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ControllerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(controller);
        }

        // DELETE: odata/Controllers(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Controller controller = await db.Controllers.FindAsync(key);
            if (controller == null)
            {
                return NotFound();
            }

            db.Controllers.Remove(controller);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Controllers(5)/hosts
        [EnableQuery]
        public IQueryable<Host> Gethosts([FromODataUri] int key)
        {
            return db.Controllers.Where(m => m.ControllerID == key).SelectMany(m => m.hosts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ControllerExists(int key)
        {
            return db.Controllers.Count(e => e.ControllerID == key) > 0;
        }
    }
}
