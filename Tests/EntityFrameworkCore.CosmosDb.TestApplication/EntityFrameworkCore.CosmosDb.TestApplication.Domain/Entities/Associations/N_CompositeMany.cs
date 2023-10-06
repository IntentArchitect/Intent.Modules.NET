using System;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class N_CompositeMany
    {
        public Guid Id { get; set; }

        public string ManyAttr { get; set; }

        public Guid NComplexrootId { get; set; }
    }
}