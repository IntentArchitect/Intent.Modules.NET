using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.Constants.ConstantsTemplate", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure
{
    public static class EntityFrameworkCoreMultiDbContextDbContextInterfaceConstants
    {
        public const string ConnStr = "ConnStr";
        public const string DefaultConnection = "DefaultConnection";
    }
}