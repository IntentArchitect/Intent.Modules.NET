using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.DocumentDB.Api.Extensions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.CosmosDB.Templates;
using Intent.Modules.DocumentDB.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

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

                var method = file.Classes.First().FindMethod("AddInfrastructure");
                method.AddInvocationStatement("services.AddCosmosRepository", invocation =>
                {
                    var classes = application.MetadataManager.Domain(application).GetClassModels()
                        .Where(x => CosmosDbProvider.FilterDbProvider(x) &&
                                    x.IsAggregateRoot() &&
                                    !x.IsAbstract)
                        .ToArray();

                    if (!classes.Any(x => x.TryGetContainerSettings(out _, out _)))
                    {
                        return;
                    }

                    invocation.AddArgument(new CSharpLambdaBlock("options"), a =>
                    {
                        var options = (CSharpLambdaBlock)a;

                        var hasDefaultContainerSettings = classes.Any(x => !x.TryGetContainerSettings(out _, out _));
                        if (hasDefaultContainerSettings)
                        {
                            options.AddStatement(
                                "var defaultContainerId = configuration.GetValue<string>(\"RepositoryOptions:ContainerId\");");
                            options.AddIfStatement("string.IsNullOrWhiteSpace(defaultContainerId)", @if => @if
                                .AddStatement($"throw new {template.UseType("System.Exception")}(\"\\\"RepositoryOptions:ContainerId\\\" configuration not specified\");")
                            );
                        }

                        options.AddStatement("options.ContainerPerItemType = true;", s =>
                        {
                            if (hasDefaultContainerSettings)
                            {
                                s.SeparatedFromPrevious();
                            }
                        });

                        options.AddMethodChainStatement("options.ContainerBuilder", c =>
                        {
                            foreach (var @class in classes)
                            {
                                if (!@class.TryGetContainerSettings(out var containerName, out var partitionKey))
                                {
                                    containerName = "defaultContainerId";
                                }
                                else
                                {
                                    containerName = $"\"{containerName}\"";
                                }

                                var documentTypeName = template.GetCosmosDBDocumentName(@class);
                                c.AddChainStatement(new CSharpInvocationStatement($"Configure<{documentTypeName}>"), l =>
                                {
                                    var configureContainer = (CSharpInvocationStatement)l;
                                    configureContainer.WithoutSemicolon();

                                    var cSharpMethodChainStatement = new CSharpMethodChainStatement("c => c");
                                    configureContainer.AddArgument(cSharpMethodChainStatement.WithoutSemicolon());

                                    cSharpMethodChainStatement.AddChainStatement($"WithContainer({containerName})");
                                    if (!string.IsNullOrWhiteSpace(partitionKey))
                                    {
                                        cSharpMethodChainStatement.AddChainStatement($"WithPartitionKey(\"/{partitionKey}\")");
                                    }
                                });
                            }
                        });
                    });
                });
            });
        }
    }
}