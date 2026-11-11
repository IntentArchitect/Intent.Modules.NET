using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.Constants.ConstantsTemplate", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Infrastructure
{
    public static class EntityFrameworkCoreSqlServerEF11Constants
    {
        public const string DefaultConnection = "DefaultConnection";
    }
}