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
    builder.EntitySet<Controller>("Controllers");
    builder.EntitySet<Host>("Hosts"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ControllersController : ODataController
    {
        private PrincipalAPIContext db = new PrincipalAPIContext();

        // GET: odata/Controllers
        [EnableQuery]
        public IQueryable<Controller> GetControllers()
        {
            return db.Controllers;
        }

        // GET: odata/Controllers(5)
        [EnableQuery]
        public SingleResult<Controller> GetController([FromODataUri] string key)
        {
            return SingleResult.Create(db.Controllers.Where(controller => controller.ControllerIP == key));
        }

        // PUT: odata/Controllers(5)
        public async Task<IHttpActionResult> Put([FromODataUri] string key, Delta<Controller> patch)
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

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ControllerExists(controller.ControllerIP))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(controller);
        }

        // PATCH: odata/Controllers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] string key, Delta<Controller> patch)
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
        public async Task<IHttpActionResult> Delete([FromODataUri] string key)
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
        public IQueryable<Host> Gethosts([FromODataUri] string key)
        {
            return db.Controllers.Where(m => m.ControllerIP == key).SelectMany(m => m.hosts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public bool ControllerExists(string key)
        {
            return db.Controllers.Count(e => e.ControllerIP == key) > 0;
        }
    }
}
