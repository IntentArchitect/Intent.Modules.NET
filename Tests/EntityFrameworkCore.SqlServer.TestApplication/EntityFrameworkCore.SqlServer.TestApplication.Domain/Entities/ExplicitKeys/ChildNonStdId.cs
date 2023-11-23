using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ExplicitKeys
{
    public class ChildNonStdId
    {
        public Guid DiffId { get; set; }

        public string Name { get; set; }
    }
}