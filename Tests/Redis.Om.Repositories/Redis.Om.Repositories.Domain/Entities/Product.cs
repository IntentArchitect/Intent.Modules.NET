using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Redis.Om.Repositories.Domain.Entities
{
    public class Product
    {
        public string Name { get; set; }

        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}