using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
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

                    if (classes.All(x => !x.TryGetContainerSettings(out _)))
                    {
                        return;
                    }

                    invocation.AddArgument(new CSharpLambdaBlock("options"), options =>
                    {
                        const string defaultContainerId = "defaultContainerId";

                        var hasDefaultContainerName = classes.Any(x => !x.TryGetContainerSettings(out var containerSettings) ||
                                                                       containerSettings.Name == null);
                        if (hasDefaultContainerName)
                        {

                            options.AddStatement(
                                $"var {defaultContainerId} = configuration.GetValue<string>(\"RepositoryOptions:ContainerId\");");
                            options.AddIfStatement("string.IsNullOrWhiteSpace(defaultContainerId)", @if => @if
                                .AddStatement($"throw new {template.UseType("System.Exception")}(\"\\\"RepositoryOptions:ContainerId\\\" configuration not specified\");")
                            );
                        }

                        options.AddStatement("options.ContainerPerItemType = true;", s =>
                        {
                            if (hasDefaultContainerName)
                            {
                                s.SeparatedFromPrevious();
                            }
                        });

                        options.AddMethodChainStatement("options.ContainerBuilder", c =>
                        {
                            foreach (var @class in classes)
                            {
                                @class.TryGetContainerSettings(out var containerSettings);

                                var documentTypeName = template.GetCosmosDBDocumentName(@class);
                                c.AddChainStatement(new CSharpInvocationStatement($"Configure<{documentTypeName}>"), l =>
                                {
                                    var configureContainer = (CSharpInvocationStatement)l;
                                    configureContainer.WithoutSemicolon();

                                    var cSharpMethodChainStatement = new CSharpMethodChainStatement("c => c");
                                    configureContainer.AddArgument(cSharpMethodChainStatement.WithoutSemicolon());

                                    var containerName = containerSettings?.Name != null ? $"\"{containerSettings.Name}\"" : defaultContainerId;
                                    cSharpMethodChainStatement.AddChainStatement($"WithContainer({containerName})");

                                    if (containerSettings?.PartitionKey != null)
                                    {
                                        cSharpMethodChainStatement.AddChainStatement($"WithPartitionKey(\"/{containerSettings.PartitionKey}\")");
                                    }

                                    switch (containerSettings?.ThroughputType)
                                    {
                                        case ContainerThroughputType.Autoscale:
                                            cSharpMethodChainStatement.AddChainStatement($"WithAutoscaleThroughput({containerSettings.AutomaticThroughputMax:D})");
                                            break;
                                        case ContainerThroughputType.Manual:
                                            cSharpMethodChainStatement.AddChainStatement($"WithManualThroughput({containerSettings.ManualThroughput:D})");
                                            break;
                                        case ContainerThroughputType.Serverless:
                                            cSharpMethodChainStatement.AddChainStatement($"WithServerlessThroughput({containerSettings.ManualThroughput:D})");
                                            break;
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