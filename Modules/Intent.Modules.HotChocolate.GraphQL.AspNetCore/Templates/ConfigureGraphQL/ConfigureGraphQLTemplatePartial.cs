using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Templates.MutationResolver;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore.Templates.ConfigureGraphQL
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ConfigureGraphQLTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.HotChocolate.GraphQL.AspNetCore.ConfigureGraphQL";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ConfigureGraphQLTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.HotChocolateAspNetCore);

        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"GraphQLConfiguration",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetGraphQLRegistrations()
        {
            var queryResolvers = ExecutionContext.FindTemplateInstances(QueryResolverTemplate.TemplateId, (t) => true);
            var mutationResolvers = ExecutionContext.FindTemplateInstances(MutationResolverTemplate.TemplateId, (t) => true);
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