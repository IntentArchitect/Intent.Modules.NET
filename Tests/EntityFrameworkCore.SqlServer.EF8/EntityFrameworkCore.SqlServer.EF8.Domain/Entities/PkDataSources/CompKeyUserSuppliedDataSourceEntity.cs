using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.PkDataSources
{
    public class CompKeyUserSuppliedDataSourceEntity
    {
        public CompKeyUserSuppliedDataSourceEntity()
        {
            FieldValue = null!;
        }

        public long Id1 { get; set; }

        public long Id2 { get; set; }

        public string FieldValue { get; set; }
    }
}