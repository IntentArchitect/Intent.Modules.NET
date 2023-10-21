using System;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class P_SourceNameDiffDependent
    {
        public Guid Id { get; set; }

        public Guid SourceNameDiffId { get; set; }
    }
}