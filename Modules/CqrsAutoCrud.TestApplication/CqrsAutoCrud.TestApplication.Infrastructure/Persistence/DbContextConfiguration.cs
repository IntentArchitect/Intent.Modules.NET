using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DependencyInjection.EntityFrameworkCore.DbContextConfiguration", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Infrastructure.Persistence
{
    public class DbContextConfiguration
    {
        public string? DefaultSchemaName { get; set; }
        public bool? EnsureDbCreated { get; set; }
    }
}