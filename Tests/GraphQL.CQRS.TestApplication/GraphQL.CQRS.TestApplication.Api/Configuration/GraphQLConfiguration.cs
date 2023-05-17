using System;
using GraphQL.CQRS.TestApplication.Api.GraphQL.Mutations;
using GraphQL.CQRS.TestApplication.Api.GraphQL.Queries;
using GraphQL.CQRS.TestApplication.Api.GraphQL.Subscriptions;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.AspNetCore.GraphQLConfiguration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace GraphQL.CQRS.TestApplication.Api.Configuration
{
    public static class GraphQLConfiguration
    {
        public static IServiceCollection ConfigureGraphQL(this IServiceCollection services)
        {
            services
                .AddGraphQLServer()
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
                .AddTypeExtension<Query>()
                .AddTypeExtension<CustomerQueries>()
                .AddTypeExtension<InvoiceQueries>()
                .AddTypeExtension<ProductsQueries>();
        }

        private static IRequestExecutorBuilder AddGraphQLMutations(this IRequestExecutorBuilder builder)
        {
            return builder
                .AddMutationConventions()
                .AddMutationType()
                .AddTypeExtension<Mutation>()
                .AddTypeExtension<CustomerMutations>()
                .AddTypeExtension<InvoiceMutations>()
                .AddTypeExtension<ProductsMutations>();
        }

        private static IRequestExecutorBuilder AddGraphQLSubscriptions(this IRequestExecutorBuilder builder)
        {
            return builder
                .AddInMemorySubscriptions()
                .AddSubscriptionType()
                .AddTypeExtension<Subscription>();
        }

    }
}
