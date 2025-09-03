using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.PkDataSources
{
    public class CompKeyDefaultDataSourceEntity
    {
        public CompKeyDefaultDataSourceEntity()
        {
            FieldValue = null!;
        }

        public long Id1 { get; set; }

        public long Id2 { get; set; }

        public string FieldValue { get; set; }
    }
}