using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.Constants.ConstantsTemplate", Version = "1.0")]

namespace EntityFrameworkCore.CustomPrimaryConnectionString.Infrastructure
{
    public static class EntityFrameworkCoreCustomPrimaryConnectionStringConstants
    {
        public const string CustomConnectionString = "Custom Connection String";
    }
}