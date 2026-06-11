using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.Constants.ConstantsTemplate", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure
{
    public static class EntityFrameworkCoreMultiDbContextNoDefaultDbContextConstants
    {
        public const string Db1ConnectionString = "Db1ConnectionString";
        public const string Db2ConnectionString = "Db2ConnectionString";
        public const string Db3ConnectionString = "Db3ConnectionString";
    }
}