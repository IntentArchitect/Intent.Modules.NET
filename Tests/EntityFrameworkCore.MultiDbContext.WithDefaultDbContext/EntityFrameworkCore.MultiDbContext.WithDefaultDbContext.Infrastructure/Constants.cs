using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.Constants.ConstantsTemplate", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure
{
    public static class Constants
    {
        public const string AlternateConnStrDefaultDb = "AlternateConnStrDefaultDb";
        public const string DefaultConnection = "DefaultConnection";
    }
}