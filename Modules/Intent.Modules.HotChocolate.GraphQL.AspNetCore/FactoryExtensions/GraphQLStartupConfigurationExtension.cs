using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Templates.SubscriptionType;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GraphQLStartupConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.HotChocolate.GraphQL.AspNetCore.GraphQLStartupConfigurationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            template.CSharpFile.AfterBuild(_ =>
            {
                var startup = template.StartupFile;

                startup.AddServiceConfiguration(
                    context => new CSharpInvocationStatement($"{context.Services}.ConfigureGraphQL"),
                    (statement, _) => statement.AddMetadata("configure-services-graphql", true));

            }, 100);

            template.CSharpFile.AfterBuild(_ =>
            {
                var startup = template.StartupFile;

                startup.ConfigureEndpoints((statements, context) =>
                {
                    statements.AddStatement($"{context.Endpoints}.MapGraphQL();", s => s.AddMetadata("configure-endpoints-graphql", true));
                });

                if (application.FindTemplateInstances<ITemplate>(SubscriptionTypeTemplate.TemplateId).Any())
                {
                    startup.ConfigureApp((statements, context) =>
                    {
                        var useRoutingStatement = statements.FindStatement(x => x.ToString()!.Contains(".UseRouting()"));
                        if (useRoutingStatement != null)
                        {
                            useRoutingStatement.InsertBelow($"{context.App}.UseWebSockets();");
                        }
                    });
                }
            }, 10);
        }
    }
}