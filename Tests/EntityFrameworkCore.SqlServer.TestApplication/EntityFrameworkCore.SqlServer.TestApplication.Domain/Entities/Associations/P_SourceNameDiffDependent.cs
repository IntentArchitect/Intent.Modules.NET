using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    public class P_SourceNameDiffDependent
    {
        public Guid Id { get; set; }

        public Guid SourceNameDiffId { get; set; }
    }
}