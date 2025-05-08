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
    public class ODataProductsController : ODataController
    {
        private readonly ApplicationDbContext _context;

        public ODataProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IQueryable<ODataProduct> Get()
        {
            return _context.ODataProducts;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid key)
        {
            var oDataProduct = await _context.ODataProducts.FirstOrDefaultAsync(m => m.Id == key);

            return oDataProduct == null ? NotFound() : Ok(oDataProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ODataProduct oDataProduct)
        {
            await _context.ODataProducts.AddAsync(oDataProduct);
            await _context.SaveChangesAsync();

            return Created(oDataProduct);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(Guid key)
        {
            var oDataProduct = await _context.ODataProducts.SingleOrDefaultAsync(m => m.Id == key);

            if (oDataProduct is not null)
            {
                _context.ODataProducts.Remove(oDataProduct);
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(Guid key, [FromBody] Delta<ODataProduct> delta)
        {
            var oDataProduct = await _context.ODataProducts.SingleOrDefaultAsync(m => m.Id == key);

            if (oDataProduct is null)
            {
                return NotFound();
            }
            delta.Patch(oDataProduct);
            await _context.SaveChangesAsync();

            return Updated(oDataProduct);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Guid key, [FromBody] ODataProduct update)
        {
            var oDataProduct = await _context.ODataProducts.AsNoTracking().SingleOrDefaultAsync(m => m.Id == key);

            if (oDataProduct is null)
            {
                return NotFound();
            }
            update.Id = key;
            _context.Entry(update).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Updated(oDataProduct);
        }
    }
}