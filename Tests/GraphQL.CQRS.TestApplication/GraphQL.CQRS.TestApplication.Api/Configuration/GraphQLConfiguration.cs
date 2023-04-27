using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.AspNetCore.ConfigureGraphQL", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.Configuration
{
    public static class GraphQLConfiguration
    {
        public static IServiceCollection ConfigureGraphQL(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGraphQLServer()
                .AddMutationType(x => x.Name("Mutation"))
                .AddQueryType(x => x.Name("Query"));
            return services;
        }
    }
}