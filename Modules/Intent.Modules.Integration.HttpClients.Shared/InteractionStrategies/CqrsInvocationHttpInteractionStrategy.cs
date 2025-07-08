using System;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Contracts.Clients.Templates;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Integration.HttpClients.Shared.InteractionStrategies;

internal class CqrsInvocationHttpInteractionStrategy : IInteractionStrategy
{
    private const string PerformInvocationTypeId = "093e5909-ffe4-4510-b3ea-532f30212f3c";
    private readonly IApplication _application;

    public CqrsInvocationHttpInteractionStrategy(IApplication application)
    {
        _application = application;
    }

    public bool IsMatch(IElement interaction)
    {
        return interaction is IAssociationEnd { SpecializationTypeId: PerformInvocationTypeId, TypeReference.Element: IElement element } &&
               element.HasHttpSettings() &&
               element.Package.ApplicationId != _application.Id; // There seems to be a bug in v4.5.0-beta.2 where the element's application ID is wrong
    }

    public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interaction)
    {
        var cqrsElement = (IElement)interaction.TypeReference.Element;
        var cqrsFieldCount = cqrsElement.ChildElements.Count(x => x.IsDTOFieldModel());

        var @class = method.Class;
        var template = (ICSharpFileBuilderTemplate)@class.File.Template;

        try
        {
            if (!template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.ClientInterface, cqrsElement.ParentId, out var serviceInterfaceTemplate))
            {
                return;
            }

            var methodToInvoke = serviceInterfaceTemplate.CSharpFile.Interfaces
                .SelectMany(x => x.Methods)
                .Single(x => x.RepresentedModel.Id == cqrsElement.Id);

            // So that the mapping system can resolve the name of the operation from the interface itself:
            template.AddTypeSource(serviceInterfaceTemplate.Id);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            var serviceField = @class.InjectService(template.GetTypeName(serviceInterfaceTemplate));
            var invocation = new CSharpInvocationStatement($"await {serviceField}.{methodToInvoke.Name}");

            if (cqrsFieldCount > 0)
            {
                var csharpMapping = method.GetMappingManager();
                csharpMapping.AddMappingResolver(new SingleParameterRequestMappingTypeResolver(template), priority: -10);
                var creationExpression = csharpMapping.GenerateCreationStatement(interaction.Mappings.First());

                invocation.AddArgument(creationExpression);
            }

            if (methodToInvoke.IsAsync && method.IsAsync)
            {
                invocation.AddArgument("cancellationToken");
            }

            CSharpStatement statement = cqrsElement.TypeReference?.Element != null
                ? new CSharpAssignmentStatement($"var {interaction.Name.ToLocalVariableName()}", invocation)
                : invocation;

            method.AddStatement(ExecutionPhases.BusinessLogic, statement);
        }
        catch (Exception ex)
        {
            throw new ElementException(interaction, $"An error occurred while generating the interaction logic: {ex.Message}\nSee inner exception for more details.", ex);
        }
    }

    private class SingleParameterRequestMappingTypeResolver(ICSharpFileBuilderTemplate template) : IMappingTypeResolver
    {
        public ICSharpMapping? ResolveMappings(MappingModel mappingModel)
        {
            if (mappingModel.Model.SpecializationTypeId is not (CommandModel.SpecializationTypeId or QueryModel.SpecializationTypeId) ||
                ((IElement)mappingModel.Model).ChildElements.Count(x => x.IsDTOFieldModel()) != 1)
            {
                return null;
            }

            return new SingleFieldMapping(mappingModel, template);
        }
    }

    private class SingleFieldMapping(MappingModel model, ICSharpTemplate template) : CSharpMappingBase(model, template)
    {
        public override CSharpStatement GetSourceStatement(bool? targetIsNullable = null)
        {
            var child = Children.First();
            return child.GetSourceStatement();
        }
    }
}