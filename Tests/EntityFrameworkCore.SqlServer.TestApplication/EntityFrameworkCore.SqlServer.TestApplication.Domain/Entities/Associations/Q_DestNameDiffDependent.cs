using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    public class Q_DestNameDiffDependent
    {
        public Guid Id { get; set; }
    }
}