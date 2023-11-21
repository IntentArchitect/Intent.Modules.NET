using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities
{
    public class SchemaInLineChild
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}