using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.InteractionStrategies;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MongoDbRepositoryUpdateExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.MediatR.CRUD.MongoDbRepositoryUpdateExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            // Putting here for now, but is wrong place
            InteractionStrategyProvider.Instance.Register(new QueryInteractionStrategy());
        }

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
            var templates = application.FindTemplateInstances<CSharpTemplateBase<CommandModel>>(TemplateDependency.OnTemplate("Application.Command.Handler"))
                .Where(x => x.CanRunTemplate());
            foreach (var template in templates)
            {
                var strategy = new CreateImplementationStrategy(template);
                if (!strategy.IsMatch() || strategy.GetStrategyData().FoundEntity.IsAggregateRoot())
                {
                    continue;
                }

                ((ICSharpFileBuilderTemplate)template).CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First(x => x.HasMetadata("handler"));
                    var method = priClass.FindMethod("Handle");
                    var strategyData = strategy.GetStrategyData();
                    if (strategyData.FoundEntity.InternalElement.Package.SpecializationType != "Mongo Domain Package")
                    {
                        return;
                    }
                    method.Statements.Last()
                        .InsertAbove($"{strategyData.Repository.FieldName}.Update(aggregateRoot);");
                }, 100);
            }
        }

        private void InstallOnNestedCompositeDeleteCommandHandlers(IApplication application)
        {
            var templates = application.FindTemplateInstances<CSharpTemplateBase<CommandModel>>(TemplateDependency.OnTemplate("Application.Command.Handler"))
                .Where(x => x.CanRunTemplate());
            foreach (var template in templates)
            {
                var strategy = new DeleteImplementationStrategy(template);
                if (!strategy.IsMatch() || strategy.GetStrategyData().FoundEntity.IsAggregateRoot())
                {
                    continue;
                }

                ((ICSharpFileBuilderTemplate)template).CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First(x => x.HasMetadata("handler"));
                    var method = priClass.FindMethod("Handle");
                    var strategyData = strategy.GetStrategyData();
                    if (strategyData.FoundEntity.InternalElement.Package.SpecializationType != "Mongo Domain Package")
                    {
                        return;
                    }
                    method.Statements.Last()
                        .InsertAbove($"{strategyData.Repository.FieldName}.Update( aggregateRoot);");
                }, 100);
            }
        }

        private void InstallOnNestedCompositeUpdateCommandHandlers(IApplication application)
        {
            var templates = application.FindTemplateInstances<CSharpTemplateBase<CommandModel>>(TemplateDependency.OnTemplate("Application.Command.Handler"))
                .Where(x => x.CanRunTemplate());
            foreach (var template in templates)
            {
                var strategy = new UpdateImplementationStrategy(template);
                if (!strategy.IsMatch() || strategy.GetStrategyData().FoundEntity.IsAggregateRoot())
                {
                    continue;
                }

                ((ICSharpFileBuilderTemplate)template).CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First(x => x.HasMetadata("handler"));
                    var method = priClass.FindMethod("Handle");
                    var strategyData = strategy.GetStrategyData();
                    if (strategyData.FoundEntity.InternalElement.Package.SpecializationType != "Mongo Domain Package")
                    {
                        return;
                    }
                    method.Statements.Last()
                        .InsertAbove($"{strategyData.Repository.FieldName}.Update(aggregateRoot);");
                }, 100);
            }
        }

        private void InstallOnUpdateAggregateCommandHandlers(IApplication application)
        {
            var templates = application.FindTemplateInstances<CSharpTemplateBase<CommandModel>>(TemplateDependency.OnTemplate("Application.Command.Handler"))
                .Where(x => x.CanRunTemplate());
            foreach (var template in templates)
            {
                var strategy = new UpdateImplementationStrategy(template);
                if (!strategy.IsMatch() || !strategy.GetStrategyData().FoundEntity.IsAggregateRoot())
                {
                    continue;
                }

                ((ICSharpFileBuilderTemplate)template).CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First(x => x.HasMetadata("handler"));
                    var method = priClass.FindMethod("Handle");
                    var strategyData = strategy.GetStrategyData();
                    if (strategyData.FoundEntity.InternalElement.Package.SpecializationType != "Mongo Domain Package")
                    {
                        return;
                    }
                    method.Statements.Last()
                        .InsertAbove($"{strategyData.Repository.FieldName}.Update(existing{strategyData.FoundEntity.Name});");
                }, 100);
            }
        }
    }
}