using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.PkDataSources
{
    public class DefaultDataSourceEntity
    {
        public DefaultDataSourceEntity()
        {
            FieldValue = null!;
        }

        public long Id { get; set; }

        public string FieldValue { get; set; }
    }
}