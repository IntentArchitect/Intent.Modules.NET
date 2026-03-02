using System;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.Contracts.DomainMapping.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.CRUD.Eventing.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class EventBusPublisherInstaller : FactoryExtensionBase
{
    public override string Id => "Intent.Application.ServiceImplementations.CRUD.Eventing.EventBusPublisherInstaller";

    [IntentManaged(Mode.Ignore)] public override int Order => 10;

    protected override void OnBeforeTemplateExecution(IApplication application)
    {
        var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Application.Implementation.Custom")).ToList();
        if (!templates.Any())
        {
            templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(ServiceImplementationTemplate.TemplateId)).ToList();
        }

        foreach (var template in templates)
        {
            template.CSharpFile.AfterBuild(file => { ProcessTemplate(application, template, file); });
        }
    }

    private void ProcessTemplate(IApplication application, ICSharpFileBuilderTemplate template, CSharpFile file)
    {
        var createStrategy = new CreateImplementationStrategy(template, application);
        var updateStrategy = new UpdateImplementationStrategy(template, application);
        var deleteStrategy = new DeleteWithReturnDtoImplementationStrategy(template, application);
        var legacyDeleteStrategy = new LegacyDeleteImplementationStrategy(template, application);

        var hasAtLeastOneMatch = false;

        var @class = file.Classes.First();
        foreach (var method in @class.Methods.Where(p => p.TryGetMetadata("model", out OperationModel _)))
        {
            var operation = method.GetMetadata<OperationModel>("model");
            if (createStrategy.IsMatch(operation))
            {
                var (_, domainModel) = operation.GetCreateModelPair();
                var messageModel = GetMessageModel(application, domainModel, "create");
                if (messageModel == null)
                {
                    continue;
                }

                AddMessageExtensionsUsings(application, file, messageModel);
                AddPublishStatement(
                    method: method,
                    publishStatement: $"_eventBus.Publish({domainModel.GetNewVariableName()}.MapTo{messageModel.Name.ToPascalCase().Replace("Event", "")}Event());");

                hasAtLeastOneMatch = true;
            }
            else if (updateStrategy.IsMatch(operation))
            {
                var (_, domainModel) = operation.GetUpdateModelPair();
                var messageModel = GetMessageModel(application, domainModel, "update");
                if (messageModel == null)
                {
                    continue;
                }

                AddMessageExtensionsUsings(application, file, messageModel);
                AddPublishStatement(
                    method: method,
                    publishStatement: $"_eventBus.Publish({domainModel.GetExistingVariableName()}.MapTo{messageModel.Name.ToPascalCase().Replace("Event", "")}Event());");

                hasAtLeastOneMatch = true;
            }
            else if (deleteStrategy.IsMatch(operation))
            {
                var (_, domainModel) = operation.GetDeleteModelPair();
                var messageModel = GetMessageModel(application, domainModel, "delete");
                if (messageModel == null)
                {
                    continue;
                }

                AddMessageExtensionsUsings(application, file, messageModel);
                AddPublishStatement(
                    method: method,
                    publishStatement: $"_eventBus.Publish({domainModel.GetExistingVariableName()}.MapTo{messageModel.Name.ToPascalCase().Replace("Event", "")}Event());");

                hasAtLeastOneMatch = true;
            }
            else if (legacyDeleteStrategy.IsMatch(operation))
            {
                var domainModel = operation.GetLegacyDeleteDomainModel(application);
                var messageModel = GetMessageModel(application, domainModel, "delete");
                if (messageModel == null)
                {
                    continue;
                }

                AddMessageExtensionsUsings(application, file, messageModel);
                AddPublishStatement(
                    method: method,
                    publishStatement: $"_eventBus.Publish({domainModel.GetExistingVariableName()}.MapTo{messageModel.Name.ToPascalCase().Replace("Event", "")}Event());");

                hasAtLeastOneMatch = true;
            }
        }

        if (!hasAtLeastOneMatch)
        {
            return;
        }

        var ctor = @class.Constructors.First();
        if (ctor.Parameters.All(p => p.Name != "eventBus"))
        {
            ctor.AddParameter(template.GetTypeName(EventBusInterfaceTemplate.TemplateId), "eventBus", parm => parm.IntroduceReadonlyField());
        }
    }

    private static void AddPublishStatement(CSharpClassMethod method, string publishStatement)
    {
        var returnClause = method.Statements.FirstOrDefault(p => p.GetText("").Trim().StartsWith("return"));

        if (returnClause != null)
        {
            returnClause.InsertAbove(publishStatement);
        }
        else
        {
            method.Statements.Add(publishStatement);
        }
    }

    private static void AddMessageExtensionsUsings(IApplication application, CSharpFile cSharpFile, MessageModel message)
    {
        var mapToMethodTemplate = application.FindTemplateInstance<IClassProvider>("Application.Eventing.MessageExtensions", message);
        cSharpFile.AddUsing(mapToMethodTemplate.Namespace);
    }

    private MessageModel GetMessageModel(IApplication application, ClassModel domainModel, string convention)
    {
        var app = application.MetadataManager.Eventing(application).GetApplicationModels().SingleOrDefault();
        var messageModel = application.MetadataManager.Eventing(application).GetMessageModels()
            .FirstOrDefault(x => HasMappedDomainEntityPresent(app, x, domainModel) && GetConventionName(x.Name) == convention);
        return messageModel;
    }

    private static string GetConventionName(string name)
    {
        return name.ToLower() switch
        {
            var x when x.Contains("create", StringComparison.OrdinalIgnoreCase) || x.StartsWith("new", StringComparison.OrdinalIgnoreCase) || x.Contains("add", StringComparison.OrdinalIgnoreCase) => "create",
            var x when x.Contains("update", StringComparison.OrdinalIgnoreCase) || x.Contains("edit", StringComparison.OrdinalIgnoreCase) => "update",
            var x when x.Contains("delete", StringComparison.OrdinalIgnoreCase) || x.Contains("remove", StringComparison.OrdinalIgnoreCase) => "delete",
            _ => null
        };
    }

    private bool HasMappedDomainEntityPresent(ApplicationModel applicationModel, MessageModel messageModel, ClassModel domainModel)
    {
        if (applicationModel.PublishedMessages().All(p => p.Element.AsMessageModel().Id != messageModel.Id))
        {
            return false;
        }

        var domainMapping = messageModel.GetMapFromDomainMapping();
        if (domainMapping == null)
        {
            return false;
        }

        return domainMapping.ElementId == domainModel.Id;
    }
}