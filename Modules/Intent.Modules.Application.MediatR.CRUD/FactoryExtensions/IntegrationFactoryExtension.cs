using System.Linq;
using Intent.Engine;
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

            var templates = application.FindTemplateInstances<CommandHandlerTemplate>(TemplateDependency.OnTemplate("Application.Command.Handler"));
            foreach (var template in templates)
            {
                var strategy = new UpdateImplementationStrategy(template, application, application.MetadataManager);
                if (!strategy.IsMatch())
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
                        .InsertAbove($"{strategyData.Repository.FieldName}.Update(p => p.{strategyData.FoundEntity.GetEntityIdAttribute().IdName} == request.{strategyData.IdField.Name.ToPascalCase()}, existing{strategyData.FoundEntity.Name});");
                }, 100);
            }
        }
    }
}