using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations
{
    public class J_RequiredDependent
    {
        public Guid Id { get; set; }

        public string ReqDepAttr { get; set; }
    }
}