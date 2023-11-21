using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Indexes
{
    public class SingleChildIndex
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}