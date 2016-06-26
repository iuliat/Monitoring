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
    builder.EntitySet<Notifications>("Notifications");
    builder.EntitySet<Host>("Hosts"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class NotificationsController : ODataController
    {
        private PrincipalAPIContext db = new PrincipalAPIContext();

        // GET: odata/Notifications
        [EnableQuery]
        public IQueryable<Notifications> GetNotifications()
        {
            return db.Notifications;
        }

        // GET: odata/Notifications(5)
        [EnableQuery]
        public SingleResult<Notifications> GetNotifications([FromODataUri] int key)
        {
            return SingleResult.Create(db.Notifications.Where(notifications => notifications.NotificationID == key));
        }

        // PUT: odata/Notifications(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Notifications> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Notifications notifications = await db.Notifications.FindAsync(key);
            if (notifications == null)
            {
                return NotFound();
            }

            patch.Put(notifications);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationsExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(notifications);
        }

        // POST: odata/Notifications
        public async Task<IHttpActionResult> Post(Notifications notifications)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Notifications.Add(notifications);
            await db.SaveChangesAsync();

            return Created(notifications);
        }

        // PATCH: odata/Notifications(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Notifications> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Notifications notifications = await db.Notifications.FindAsync(key);
            if (notifications == null)
            {
                return NotFound();
            }

            patch.Patch(notifications);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationsExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(notifications);
        }

        // DELETE: odata/Notifications(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Notifications notifications = await db.Notifications.FindAsync(key);
            if (notifications == null)
            {
                return NotFound();
            }

            db.Notifications.Remove(notifications);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Notifications(5)/Host
        [EnableQuery]
        public SingleResult<Host> GetHost([FromODataUri] int key)
        {
            return SingleResult.Create(db.Notifications.Where(m => m.NotificationID == key).Select(m => m.Host));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NotificationsExists(int key)
        {
            return db.Notifications.Count(e => e.NotificationID == key) > 0;
        }
    }
}
