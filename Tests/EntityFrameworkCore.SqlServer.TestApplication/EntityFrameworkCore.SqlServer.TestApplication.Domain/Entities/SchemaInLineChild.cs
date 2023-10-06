using System;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities
{
    public class SchemaInLineChild
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}