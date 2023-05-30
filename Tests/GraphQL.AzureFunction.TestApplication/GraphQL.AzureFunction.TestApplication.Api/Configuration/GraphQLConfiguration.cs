using System;
using GraphQL.AzureFunction.TestApplication.Api.GraphQL.Mutations;
using GraphQL.AzureFunction.TestApplication.Api.GraphQL.Queries;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.AzureFunctions.GraphQLConfigTemplate", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Api.Configuration
{
    public static class GraphQLConfiguration
    {
        public static IServiceCollection ConfigureGraphQL(this IServiceCollection services)
        {
            services
                .AddGraphQLFunction()
                .AddGraphQLQueries()
                .AddGraphQLMutations()
                .AddGraphQLSubscriptions()
                .BindRuntimeType<string, StringType>()
                .BindRuntimeType<Guid, IdType>();
            return services;
        }

        private static IRequestExecutorBuilder AddGraphQLQueries(this IRequestExecutorBuilder builder)
        {
            return builder
                .AddQueryType()
                .AddTypeExtension<ProductQueries>()
                .AddTypeExtension<CustomersQueries>();
        }

        private static IRequestExecutorBuilder AddGraphQLMutations(this IRequestExecutorBuilder builder)
        {
            return builder
                .AddMutationConventions(applyToAllMutations: false)
                .AddMutationType()
                .AddTypeExtension<ProductMutations>()
                .AddTypeExtension<CustomersMutations>();
        }

        private static IRequestExecutorBuilder AddGraphQLSubscriptions(this IRequestExecutorBuilder builder)
        {
            // Subscriptions will be configured here
            return builder;
        }
    }
}