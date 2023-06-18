using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.DocumentDB.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common.CSharp.DependencyInjection;
using System.Threading;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.CosmosDB.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.CosmosDB.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Modules.CosmosDB.EntityFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            EntityFactoryExtensionHelper.Execute(application);
            RegisterServices(application);
        }

        private void RegisterServices(IApplication application)
        {
            var dependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Infrastructure.DependencyInjection);
            if (dependencyInjection == null)
            {
                return;
            }

            dependencyInjection.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Microsoft.Extensions.DependencyInjection");

                var method = file.Classes.First().FindMethod("AddInfrastructure");
                method.AddInvocationStatement("services.AddCosmosRepository", invocation =>
                {
                    var classes = application.MetadataManager.Domain(application).GetClassModels()
                        .Where(x => x.InternalElement.Package.HasStereotype("Document Database") &&
                                    x.IsAggregateRoot() && !x.IsAbstract)
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
                                .AddStatement("throw new Exception(\"\\\"RepositoryOptions:ContainerId\\\" configuration not specified\");")
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

                                var documentTypeName = dependencyInjection.GetCosmosDBDocumentName(@class);
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