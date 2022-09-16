using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DependencyInjection.EntityFrameworkCore.DbContextConfiguration", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.DependencyInjection
{
    public class DbContextConfiguration
    {
        public string DefaultContainerName { get; set; }
        public bool? EnsureDbCreated { get; set; }
    }
}