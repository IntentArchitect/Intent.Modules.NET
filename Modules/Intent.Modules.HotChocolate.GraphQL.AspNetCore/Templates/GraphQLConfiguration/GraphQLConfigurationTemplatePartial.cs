using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Templates.MutationType;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryType;
using Intent.Modules.HotChocolate.GraphQL.Templates.SubscriptionType;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore.Templates.GraphQLConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GraphQLConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.HotChocolate.GraphQL.AspNetCore.GraphQLConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GraphQLConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.HotChocolateAspNetCore(OutputTarget));
            AddNugetDependency(NugetPackages.HotChocolateAspNetCoreAuthorization(OutputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("HotChocolate.Execution.Configuration")
                .AddUsing("HotChocolate.Types")
                .AddClass("GraphQLConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureGraphQL", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        method.AddStatement(new CSharpMethodChainStatement("services")
                            .AddChainStatement("AddGraphQLServer()")
                            .AddChainStatement("AddAuthorization()")
                            .AddChainStatement("AddGraphQLQueries()")
                            .AddChainStatement("AddGraphQLMutations()")
                            .AddChainStatement("AddGraphQLSubscriptions()")
                            .AddChainStatement("BindRuntimeType<string, StringType>()")
                            .AddChainStatement("BindRuntimeType<Guid, IdType>()")
                        );
                        method.AddStatement("return services;");
                    });

                    @class.AddMethod("IRequestExecutorBuilder", "AddGraphQLQueries", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("IRequestExecutorBuilder", "builder", param => param.WithThisModifier());
                        var queryResolvers = ExecutionContext.FindTemplateInstances(QueryTypeTemplate.TemplateId, (t) => true).ToList();
                        if (!queryResolvers.Any())
                        {
                            method.AddStatement("// Queries will be configured here");
                            method.AddStatement("return builder;");
                            return;
                        }
                        method.AddStatement(new CSharpMethodChainStatement("return builder")
                            .AddChainStatement("AddQueryType()"), statement =>
                            {
                                var chain = (CSharpMethodChainStatement)statement;
                                foreach (var queryResolver in queryResolvers)
                                {
                                    chain.AddChainStatement($"AddTypeExtension<{GetTypeName(queryResolver)}>()");
                                }
                            });
                    });

                    @class.AddMethod("IRequestExecutorBuilder", "AddGraphQLMutations", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("IRequestExecutorBuilder", "builder", param => param.WithThisModifier());
                        var mutationResolvers = ExecutionContext.FindTemplateInstances(MutationTypeTemplate.TemplateId, (t) => true).ToList();
                        if (!mutationResolvers.Any())
                        {
                            method.AddStatement("// Mutations will be configured here");
                            method.AddStatement("return builder;");
                            return;
                        }
                        method.AddStatement(new CSharpMethodChainStatement("return builder")
                            .AddChainStatement("AddMutationConventions(applyToAllMutations: false)")
                            .AddChainStatement("AddMutationType()"), statement =>
                            {
                                var chain = (CSharpMethodChainStatement)statement;
                                foreach (var mutationResolver in mutationResolvers)
                                {
                                    chain.AddChainStatement($"AddTypeExtension<{GetTypeName(mutationResolver)}>()");
                                }
                            });
                    });

                    @class.AddMethod("IRequestExecutorBuilder", "AddGraphQLSubscriptions", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("IRequestExecutorBuilder", "builder", param => param.WithThisModifier());
                        var subscriptionTypes = ExecutionContext.FindTemplateInstances(SubscriptionTypeTemplate.TemplateId, (t) => true).ToList();
                        if (!subscriptionTypes.Any())
                        {
                            method.AddStatement("// Subscriptions will be configured here");
                            method.AddStatement("return builder;");
                            return;
                        }
                        method.AddStatement(new CSharpMethodChainStatement("return builder")
                            .AddChainStatement("AddInMemorySubscriptions()") // make configurable
                            .AddChainStatement("AddSubscriptionType()"), statement =>
                            {
                                var chain = (CSharpMethodChainStatement)statement;
                                foreach (var mutationResolver in subscriptionTypes)
                                {
                                    chain.AddChainStatement($"AddTypeExtension<{GetTypeName(mutationResolver)}>()");
                                }
                            });
                    });

                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private string GetGraphQLRegistrations()
        {
            var queryResolvers = ExecutionContext.FindTemplateInstances(QueryTypeTemplate.TemplateId, (t) => true);
            var mutationResolvers = ExecutionContext.FindTemplateInstances(MutationTypeTemplate.TemplateId, (t) => true);
            return $@"
                .AddMutationType(x => x.Name(""Mutation"")){(mutationResolvers.Any() ? $@"
                    {string.Join(@"
                    ", mutationResolvers.Select(x => $".AddType<{GetTypeName(x)}>()"))}" : string.Empty)}
                .AddQueryType(x => x.Name(""Query"")){(queryResolvers.Any() ? $@"
                    {string.Join(@"
                    ", queryResolvers.Select(x => $".AddType<{GetTypeName(x)}>()"))}" : string.Empty)}";
        }
    }
}