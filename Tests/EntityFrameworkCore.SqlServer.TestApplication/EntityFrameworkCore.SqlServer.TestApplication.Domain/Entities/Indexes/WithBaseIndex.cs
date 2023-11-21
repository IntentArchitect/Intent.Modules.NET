using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Indexes
{
    public class WithBaseIndex : WithBaseIndexBase
    {
        public string Name { get; set; }
    }
}