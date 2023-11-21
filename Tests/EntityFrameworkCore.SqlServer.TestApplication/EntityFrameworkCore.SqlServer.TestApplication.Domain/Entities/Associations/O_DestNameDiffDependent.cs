using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    public class O_DestNameDiffDependent
    {
        public Guid Id { get; set; }

        public Guid ODestnamediffId { get; set; }
    }
}