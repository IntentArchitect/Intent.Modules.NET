using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
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

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var commandTemplates = application.FindTemplateInstances<CommandModelsTemplate>(TemplateDependency.OnTemplate(CommandModelsTemplate.TemplateId)).ToArray();
            if (!commandTemplates.Any())
            {
                return;
            }

            var controllerTemplates = application
                .FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Distribution.WebApi.Controller)).ToArray();
            foreach (var commandTemplate in commandTemplates)
            {
                if (IsUpdateCrudCommand(commandTemplate, application))
                {
                    continue;
                }

                commandTemplate.CSharpFile.OnBuild(commandFile =>
                {
                    var controllerTemplate = controllerTemplates.FirstOrDefault(p =>
                        p.CSharpFile.Classes.First().TryGetMetadata<string>("modelId", out var classModelId) &&
                        classModelId == commandTemplate.Model.InternalElement.ParentElement?.Id);
                    if (controllerTemplate is null)
                    {
                        return;
                    }

                    var commandClass = commandTemplate.CSharpFile.Classes.First();
                    var controllerClass = controllerTemplate.CSharpFile.Classes.First();

                    controllerTemplate.CSharpFile.OnBuild(controllerFile =>
                    {
                        var controllerMethod = controllerClass.FindMethod(method =>
                            method.TryGetMetadata<string>("modelId", out var operationModelId) &&
                            operationModelId == commandTemplate.Model.Id &&
                            method.Parameters.Any(param => param.Type.Contains(commandTemplate.ClassName, StringComparison.InvariantCulture)));
                        if (controllerMethod is null)
                        {
                            return;
                        }

                        var commandFieldsLookup = commandTemplate.Model.Properties.ToDictionary(k => k.Id);
                        var matches = GetControllerParamWithCommandPropertyMatch(controllerMethod, commandClass, commandFieldsLookup);
                        var commandParameter = controllerMethod.Parameters.First(param => param.Type.Contains(commandTemplate.ClassName, StringComparison.InvariantCulture));

                        int insertIndex = -1;
                        foreach (var match in matches)
                        {
                            insertIndex++;
                            match.CommandProperty.Setter.Private();
                            commandClass.AddMethod("void", $"Set{match.CommandProperty.Name}", method =>
                            {
                                method.AddParameter(commandTemplate.GetTypeName(match.DtoFieldModel.TypeReference), match.CommandProperty.Name.ToParameterName());

                                method.AddStatement($"{match.CommandProperty.Name} = {match.CommandProperty.Name.ToParameterName()};");
                            });
                            controllerMethod.InsertStatement(insertIndex, new CSharpIfStatement($"{commandParameter.Name}.{match.CommandProperty.Name} == default")
                                .AddStatement($"{commandParameter.Name}.Set{match.CommandProperty.Name}({match.MethodParameter.Name});"));
                        }
                    }, 10);
                }, 10);
            }
        }

        private bool IsUpdateCrudCommand(CommandModelsTemplate commandTemplate, IApplication application)
        {
            var commandHandler = application.FindTemplateInstance<CommandHandlerTemplate>(CommandHandlerTemplate.TemplateId, commandTemplate.Model);
            return commandHandler is null || StrategyFactory.GetMatchedCommandStrategy(commandHandler) is not UpdateImplementationStrategy strategy;
        }
        
        private static List<MatchEntry> GetControllerParamWithCommandPropertyMatch(CSharpClassMethod method, CSharpClass commandClass, Dictionary<string, DTOFieldModel> commandFieldsLookup)
        {
            var matches = new List<MatchEntry>();
            foreach (var methodParameter in method.Parameters)
            {
                if (!methodParameter.TryGetMetadata<string>("modelId", out var paramModelId))
                {
                    continue;
                }

                var commandProp = commandClass.Properties.FirstOrDefault(prop =>
                    prop.TryGetMetadata<DTOFieldModel>("model", out var propModel) &&
                    propModel?.Id == paramModelId);
                if (commandProp is null)
                {
                    continue;
                }

                matches.Add(new MatchEntry(methodParameter, commandProp, commandFieldsLookup[paramModelId]));
            }

            return matches;
        }

        private record MatchEntry(CSharpParameter MethodParameter, CSharpProperty CommandProperty, DTOFieldModel DtoFieldModel);
    }
}