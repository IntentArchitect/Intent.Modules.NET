using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IntegrationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.MediatR.CRUD.IntegrationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (application.InstalledModules.All(p => p.ModuleId != "Intent.MongoDb.Repositories"))
            {
                return;
            }

            InstallOnUpdateAggregateCommandHandlers(application);
            InstallOnNestedCompositeCreateCommandHandlers(application);
            InstallOnNestedCompositeDeleteCommandHandlers(application);
            InstallOnNestedCompositeUpdateCommandHandlers(application);
        }

        private void InstallOnNestedCompositeCreateCommandHandlers(IApplication application)
        {
            var templates = application.FindTemplateInstances<CommandHandlerTemplate>(TemplateDependency.OnTemplate("Application.Command.Handler"));
            foreach (var template in templates)
            {
                var strategy = new CreateImplementationStrategy(template);
                if (!strategy.IsMatch() || strategy.GetStrategyData().FoundEntity.IsAggregateRoot())
                {
                    continue;
                }

                template.CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var method = priClass.FindMethod("Handle");
                    var strategyData = strategy.GetStrategyData();
                    if (strategyData.FoundEntity.InternalElement.Package.SpecializationType != "Mongo Domain Package")
                    {
                        return;
                    }
                    method.Statements.Last()
                        .InsertAbove($"{strategyData.Repository.FieldName}.Update(p => p.{strategyData.FoundEntity.GetEntityIdAttribute(template.ExecutionContext).IdName} == request.{strategyData.FoundEntity.GetNestedCompositionalOwnerIdAttribute(strategyData.FoundEntity.GetNestedCompositionalOwner(), template.ExecutionContext).IdName.ToPascalCase()}, aggregateRoot);");
                }, 100);
            }
        }

        private void InstallOnNestedCompositeDeleteCommandHandlers(IApplication application)
        {
            var templates = application.FindTemplateInstances<CommandHandlerTemplate>(TemplateDependency.OnTemplate("Application.Command.Handler"));
            foreach (var template in templates)
            {
                var strategy = new DeleteImplementationStrategy(template);
                if (!strategy.IsMatch() || strategy.GetStrategyData().FoundEntity.IsAggregateRoot())
                {
                    continue;
                }

                template.CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var method = priClass.FindMethod("Handle");
                    var strategyData = strategy.GetStrategyData();
                    if (strategyData.FoundEntity.InternalElement.Package.SpecializationType != "Mongo Domain Package")
                    {
                        return;
                    }
                    method.Statements.Last()
                        .InsertAbove($"{strategyData.Repository.FieldName}.Update(p => p.{strategyData.FoundEntity.GetEntityIdAttribute(template.ExecutionContext).IdName} == request.{strategyData.FoundEntity.GetNestedCompositionalOwnerIdAttribute(strategyData.FoundEntity.GetNestedCompositionalOwner(), template.ExecutionContext).IdName.ToPascalCase()}, aggregateRoot);");
                }, 100);
            }
        }

        private void InstallOnNestedCompositeUpdateCommandHandlers(IApplication application)
        {
            var templates = application.FindTemplateInstances<CommandHandlerTemplate>(TemplateDependency.OnTemplate("Application.Command.Handler"));
            foreach (var template in templates)
            {
                var strategy = new UpdateImplementationStrategy(template);
                if (!strategy.IsMatch() || strategy.GetStrategyData().FoundEntity.IsAggregateRoot())
                {
                    continue;
                }

                template.CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var method = priClass.FindMethod("Handle");
                    var strategyData = strategy.GetStrategyData();
                    if (strategyData.FoundEntity.InternalElement.Package.SpecializationType != "Mongo Domain Package")
                    {
                        return;
                    }
                    method.Statements.Last()
                        .InsertAbove($"{strategyData.Repository.FieldName}.Update(p => p.{strategyData.FoundEntity.GetEntityIdAttribute(template.ExecutionContext).IdName} == request.{strategyData.FoundEntity.GetNestedCompositionalOwnerIdAttribute(strategyData.FoundEntity.GetNestedCompositionalOwner(), template.ExecutionContext).IdName.ToPascalCase()}, aggregateRoot);");
                }, 100);
            }
        }

        private void InstallOnUpdateAggregateCommandHandlers(IApplication application)
        {
            var templates = application.FindTemplateInstances<CommandHandlerTemplate>(TemplateDependency.OnTemplate("Application.Command.Handler"));
            foreach (var template in templates)
            {
                var strategy = new UpdateImplementationStrategy(template);
                if (!strategy.IsMatch() || !strategy.GetStrategyData().FoundEntity.IsAggregateRoot())
                {
                    continue;
                }

                template.CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var method = priClass.FindMethod("Handle");
                    var strategyData = strategy.GetStrategyData();
                    if (strategyData.FoundEntity.InternalElement.Package.SpecializationType != "Mongo Domain Package")
                    {
                        return;
                    }
                    method.Statements.Last()
                        .InsertAbove($"{strategyData.Repository.FieldName}.Update(p => p.{strategyData.FoundEntity.GetEntityIdAttribute(template.ExecutionContext).IdName} == request.{strategyData.IdField.Name.ToPascalCase()}, existing{strategyData.FoundEntity.Name});");
                }, 100);
            }
        }
    }
}