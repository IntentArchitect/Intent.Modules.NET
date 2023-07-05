using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api.Extensions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
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
            EntityFactoryExtensionHelper.Execute(application);
            AddDocumentIdentityAssignment(application);
            RegisterServices(application);
        }

        private void AddDocumentIdentityAssignment(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Primary));
            foreach (var template in templates)
            {
                var templateModel = ((CSharpTemplateBase<ClassModel>)template).Model;
                if (!templateModel.InternalElement.Package.HasStereotype("Document Database"))
                {
                    continue;
                }

                template.CSharpFile.OnBuild((Action<CSharpFile>)(file =>
                {
                    file.AddUsing("System");
                    var @class = file.Classes.First();
                    var model = @class.GetMetadata<ClassModel>("model");

                    var pks = model.GetPrimaryKeys();
                    if (!pks.Any())
                    {
                        return;
                    }

                    var primaryKeyProperties = new List<CSharpProperty>();
                    foreach (var attribute in pks)
                    {
                        var existingPk = @class
                            .GetAllProperties()
                            .First(x => x.Name.Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase));
                        var fieldName = $"_{attribute.Name.ToCamelCase()}";
                        if (model.IsAggregateRoot())
                        {
                            EntityFactoryExtensionHelper.InitializePrimaryKey(template, @class, attribute, existingPk, fieldName);
                        }

                    }
                }));
            }
        }

        private void RegisterServices(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Infrastructure.DependencyInjection);
            if (template == null)
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