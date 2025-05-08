using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Get(Guid key)
        {
            var oDataCustomer = await _context.ODataCustomers.FirstOrDefaultAsync(m => m.Id == key);

            return oDataCustomer == null ? NotFound() : Ok(oDataCustomer);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ODataCustomer oDataCustomer)
        {
            await _context.ODataCustomers.AddAsync(oDataCustomer);
            await _context.SaveChangesAsync();

            return Created(oDataCustomer);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(Guid key)
        {
            var oDataCustomer = await _context.ODataCustomers.SingleOrDefaultAsync(m => m.Id == key);

            if (oDataCustomer is not null)
            {
                _context.ODataCustomers.Remove(oDataCustomer);
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(Guid key, [FromBody] Delta<ODataCustomer> delta)
        {
            var oDataCustomer = await _context.ODataCustomers.SingleOrDefaultAsync(m => m.Id == key);

            if (oDataCustomer is null)
            {
                return NotFound();
            }
            delta.Patch(oDataCustomer);
            await _context.SaveChangesAsync();

            return Updated(oDataCustomer);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Guid key, [FromBody] ODataCustomer update)
        {
            var oDataCustomer = await _context.ODataCustomers.AsNoTracking().SingleOrDefaultAsync(m => m.Id == key);

            if (oDataCustomer is null)
            {
                return NotFound();
            }
            update.Id = key;
            _context.Entry(update).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Updated(oDataCustomer);
        }
    }
}