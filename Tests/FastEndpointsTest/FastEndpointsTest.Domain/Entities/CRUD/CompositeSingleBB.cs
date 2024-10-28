using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.CRUD
{
    public class CompositeSingleBB
    {
        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }
    }
}