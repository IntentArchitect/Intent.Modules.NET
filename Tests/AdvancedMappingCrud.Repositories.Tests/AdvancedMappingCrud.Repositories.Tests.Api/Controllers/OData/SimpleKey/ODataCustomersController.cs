using System;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OData.SimpleKey;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.OData.EntityFramework.ODataAggregateController", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Controllers.OData.SimpleKey
{
    [EnableQuery]
    public class ODataCustomersController : ODataController
    {
        private readonly ApplicationDbContext _context;

        public ODataCustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IQueryable<ODataCustomer> Get()
        {
            return _context.ODataCustomers;
        }

        [HttpGet]
        public IActionResult Get(Guid key)
        {
            var oDataCustomer = _context.ODataCustomers.FirstOrDefault(m => m.Id == key);

            return oDataCustomer == null ? NotFound() : Ok(oDataCustomer);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ODataCustomer oDataCustomer)
        {
            _context.ODataCustomers.Add(oDataCustomer);
            _context.SaveChanges();

            return Created(oDataCustomer);
        }

        [HttpDelete]
        public ActionResult Delete(Guid key)
        {
            var oDataCustomer = _context.ODataCustomers.SingleOrDefault(m => m.Id == key);

            if (oDataCustomer is not null)
            {
                _context.ODataCustomers.Remove(oDataCustomer);
            }
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch]
        public ActionResult Patch(Guid key, [FromBody] Delta<ODataCustomer> delta)
        {
            var oDataCustomer = _context.ODataCustomers.SingleOrDefault(m => m.Id == key);

            if (oDataCustomer is null)
            {
                return NotFound();
            }
            delta.Patch(oDataCustomer);
            _context.SaveChanges();

            return Updated(oDataCustomer);
        }

        [HttpPut]
        public ActionResult Put(Guid key, [FromBody] ODataCustomer update)
        {
            var oDataCustomer = _context.ODataCustomers.AsNoTracking().SingleOrDefault(m => m.Id == key);

            if (oDataCustomer is null)
            {
                return NotFound();
            }
            update.Id = key;
            _context.Entry(update).State = EntityState.Modified;
            _context.SaveChanges();

            return Updated(oDataCustomer);
        }
    }
}