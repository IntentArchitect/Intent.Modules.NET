using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.CosmosDB.Helpers;
using Intent.Modules.CosmosDB.Settings;
using Intent.Modules.CosmosDB.Templates;
using Intent.Modules.CosmosDB.Templates.CosmosDBConfiguration;
using Intent.Modules.CosmosDB.Templates.CosmosDBDocument;
using Intent.Modules.DocumentDB.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.CosmosDB.Settings.CosmosDBSettings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.CosmosDB.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.CosmosDB.EntityFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            EntityFactoryExtensionHelper.Execute(
                application: application,
                dbProviderApplies: CosmosDbProvider.FilterDbProvider,
                primaryKeyInitStrategy: new CosmosDbPrimaryKeyInitStrategy(),
                makeNonPersistentPropertiesVirtual: false);
            RegisterServices(application);
        }

        private static void RegisterServices(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (template is null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Microsoft.Extensions.DependencyInjection");

                file.Template.AddTypeSource(CosmosDBDocumentTemplate.TemplateId);

                var method = file.Classes.First().FindMethod("AddInfrastructure");

                var authenticationMethods = application.Settings.GetCosmosDBSettings().AuthenticationMethods();
                if (authenticationMethods != null && authenticationMethods.Any(a => !a.IsKeyBased()))
                {
                    var cosmosConfigTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(CosmosDBConfigurationTemplate.TemplateId);
                    if (cosmosConfigTemplate is not null)
                    {
                        template.CSharpFile.AddUsing(cosmosConfigTemplate.Namespace);
                    }
                    
                    // add the extended setup with managed identity or multiple authentication methods
                    method.AddInvocationStatement("services.ConfigureCosmosRepository", invocation =>
                    {
                        invocation.AddArgument("configuration");
                    });
                    
                    return;
                }

                // add the default setup
                AddCosmosRepository(application, template, method);
            });
        }

        private static void AddCosmosRepository(IApplication application, ICSharpFileBuilderTemplate template, CSharpClassMethod? method)
        {
            method.AddInvocationStatement("services.AddCosmosRepository", invocation =>
            {
                ConfigurationHelper.BuildConfigurationLambda(application.MetadataManager.Domain(application), template, invocation);
            });
        }
    }
}