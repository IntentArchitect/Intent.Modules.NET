using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.PkDataSources
{
    public class UserSuppliedDataSourceEntity
    {
        public UserSuppliedDataSourceEntity()
        {
            FieldValue = null!;
        }

        public long Id { get; set; }

        public string FieldValue { get; set; }
    }
}