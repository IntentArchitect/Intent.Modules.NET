using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.BasicAudit
{
    public class Audit_DerivedClass : Audit_BaseClass
    {
        public string DerivedAttr { get; set; }
    }
}