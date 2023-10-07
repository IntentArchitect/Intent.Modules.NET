using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCommandIdExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.MediatR.CRUD.UpdateCommandIdExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var commandTemplates = application.FindTemplateInstances<CommandModelsTemplate>(TemplateDependency.OnTemplate(CommandModelsTemplate.TemplateId)).ToArray();
            if (!commandTemplates.Any())
            {
                return;
            }

            var controllerTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Distribution.WebApi.Controller)).ToArray();
            foreach (var commandTemplate in commandTemplates)
            {
                var commandHandler = application.FindTemplateInstance<CommandHandlerTemplate>(CommandHandlerTemplate.TemplateId, commandTemplate.Model);
                if (commandHandler is null ||
                    StrategyFactory.GetMatchedCommandStrategy(commandHandler) is not UpdateImplementationStrategy strategy)
                {
                    continue;
                }

                var entity = strategy.GetStrategyData().FoundEntity;
                var ids = commandTemplate.Model.Properties.GetEntityIdFields(entity, commandTemplate.ExecutionContext);
                
                commandTemplate.CSharpFile.OnBuild(commandFile =>
                {
                    var commandClass = commandFile.Classes.First();
                    foreach (var field in commandClass.Properties.Where(p => ids.Any(q => q.Name == p.Name)))
                    {
                        field.Setter.Private();
                    }

                    commandClass.AddMethod("void", "SetId", method =>
                    {
                        foreach (var id in ids)
                        {
                            method.AddParameter(commandTemplate.GetTypeName(id.TypeReference), id.Name.ToParameterName());
                            
                            method.AddIfStatement($"{id.Name.ToPropertyName()} == default", stmt =>
                            {
                                stmt.AddStatement($"{id.Name.ToPropertyName()} = {id.Name.ToParameterName()};");
                            });
                        }
                    });
                    
                    var controllerTemplate = controllerTemplates.FirstOrDefault(p => p.CSharpFile.Classes.First().TryGetMetadata<string>("modelId", out var classModelId) && classModelId == commandTemplate.Model.InternalElement.ParentElement?.Id);
                    if (controllerTemplate is null)
                    {
                        return;
                    }

                    controllerTemplate.CSharpFile.OnBuild(controllerFile =>
                    {
                        var controllerClass = controllerFile.Classes.First();
                        controllerClass.FindMethod(p => p.TryGetMetadata("modelId", out var operationModelId) && operationModelId == commandTemplate.Model.Id)
                            ?.InsertStatement(0, new CSharpInvocationStatement("command.SetId"), stmt =>
                            {
                                var invokeStmt = (CSharpInvocationStatement)stmt;
                                foreach (var id in ids)
                                {
                                    invokeStmt.AddArgument(id.Name.ToParameterName());
                                }
                            });
                    }, 10);
                }, 10);
            }
        }
    }
}