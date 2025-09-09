using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepositoryOptions", Version = "1.0")]

namespace CosmosDB.Authentication.Infrastructure.Options
{
    public class CosmosRepositoryOptions : RepositoryOptions
    {
        public string? AuthenticationMethod { get; set; }
        public string? ManagedIdentityClientId { get; set; }
    }
}