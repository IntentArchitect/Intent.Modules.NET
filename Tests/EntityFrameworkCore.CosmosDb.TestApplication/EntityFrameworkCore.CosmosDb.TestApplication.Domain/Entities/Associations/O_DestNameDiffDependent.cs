using System;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class O_DestNameDiffDependent
    {
        public Guid Id { get; set; }

        public Guid ODestnamediffId { get; set; }
    }
}