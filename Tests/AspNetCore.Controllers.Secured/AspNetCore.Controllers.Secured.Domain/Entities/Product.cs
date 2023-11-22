using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace AspNetCore.Controllers.Secured.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}