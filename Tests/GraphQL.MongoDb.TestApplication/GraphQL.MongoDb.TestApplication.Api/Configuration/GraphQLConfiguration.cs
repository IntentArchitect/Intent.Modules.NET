using System;
using GraphQL.MongoDb.TestApplication.Api.GraphQL.Mutations;
using GraphQL.MongoDb.TestApplication.Api.GraphQL.Queries;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.AspNetCore.GraphQLConfiguration", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Api.Configuration
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
                .AddTypeExtension<PrivilegeQueries>()
                .AddTypeExtension<UserQueries>();
        }

        private static IRequestExecutorBuilder AddGraphQLMutations(this IRequestExecutorBuilder builder)
        {
            return builder
                .AddMutationType()
                .AddTypeExtension<PrivilegeMutations>()
                .AddTypeExtension<UserMutations>();
        }

        private static IRequestExecutorBuilder AddGraphQLSubscriptions(this IRequestExecutorBuilder builder)
        {
            // Subscriptions will be configured here
            return builder;
        }
    }
}