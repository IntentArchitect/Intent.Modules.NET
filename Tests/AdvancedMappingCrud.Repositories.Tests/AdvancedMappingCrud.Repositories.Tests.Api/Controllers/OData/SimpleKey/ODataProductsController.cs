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
        public IActionResult Get(Guid key)
        {
            var oDataProduct = _context.ODataProducts.FirstOrDefault(m => m.Id == key);

            return oDataProduct == null ? NotFound() : Ok(oDataProduct);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ODataProduct oDataProduct)
        {
            _context.ODataProducts.Add(oDataProduct);
            _context.SaveChanges();

            return Created(oDataProduct);
        }

        [HttpDelete]
        public ActionResult Delete(Guid key)
        {
            var oDataProduct = _context.ODataProducts.SingleOrDefault(m => m.Id == key);

            if (oDataProduct is not null)
            {
                _context.ODataProducts.Remove(oDataProduct);
            }
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch]
        public ActionResult Patch(Guid key, [FromBody] Delta<ODataProduct> delta)
        {
            var oDataProduct = _context.ODataProducts.SingleOrDefault(m => m.Id == key);

            if (oDataProduct is null)
            {
                return NotFound();
            }
            delta.Patch(oDataProduct);
            _context.SaveChanges();

            return Updated(oDataProduct);
        }

        [HttpPut]
        public ActionResult Put(Guid key, [FromBody] ODataProduct update)
        {
            var oDataProduct = _context.ODataProducts.AsNoTracking().SingleOrDefault(m => m.Id == key);

            if (oDataProduct is null)
            {
                return NotFound();
            }
            update.Id = key;
            _context.Entry(update).State = EntityState.Modified;
            _context.SaveChanges();

            return Updated(oDataProduct);
        }
    }
}