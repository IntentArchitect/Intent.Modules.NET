using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.BasicAudit
{
    public class Audit_DerivedClass : Audit_BaseClass
    {
        public string DerivedAttr { get; set; }
    }
}