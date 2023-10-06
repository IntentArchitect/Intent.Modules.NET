using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.BasicAudit
{
    public class Audit_DerivedClass : Audit_BaseClass
    {
        public string DerivedAttr { get; set; }
    }
}