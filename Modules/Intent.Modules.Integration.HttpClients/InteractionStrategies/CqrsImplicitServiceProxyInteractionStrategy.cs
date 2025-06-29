using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Clients.Templates;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Integration.HttpClients.InteractionStrategies;

internal class CqrsImplicitServiceProxyInteractionStrategy : IInteractionStrategy
{
    private readonly IApplication _application;

    public CqrsImplicitServiceProxyInteractionStrategy(IApplication application)
    {
        _application = application;
    }

    public bool IsMatch(IElement interaction)
    {
        if (!IsPerformInvocationTargetEndModel(interaction))
        {
            return false;
        }

        var element = interaction.TypeReference.Element as IElement;
        var isMatch = element != null &&
                      element.HasHttpSettings() &&
                      element.Package.ApplicationId != _application.Id; // There seems to be a bug in v4.5.0-beta.2 where the element's application ID is wrong

        return isMatch;

        // TODO: JL Copied for now so no reference needs to be added 
        static bool IsPerformInvocationTargetEndModel(ICanBeReferencedType type)
        {
            return type is IAssociationEnd { SpecializationTypeId: "093e5909-ffe4-4510-b3ea-532f30212f3c" };
        }
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
            template.AddTypeSource(TemplateRoles.Application.Query);
            template.AddTypeSource(TemplateRoles.Application.Command);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Dto);

            var serviceField = @class.InjectService(template.GetTypeName(serviceInterfaceTemplate));
            var invocation = new CSharpInvocationStatement($"await {serviceField}.{methodToInvoke.Name}");

            if (cqrsFieldCount > 0)
            {
                var csharpMapping = method.GetMappingManager();

                // We don't want to generate construction a command/query when only a single parameter
                if (cqrsFieldCount == 1) 
                {
                    // TODO JL: This isn't working
                    //csharpMapping.ClearMappingResolvers();
                    //csharpMapping.AddMappingResolver(new SingleParameterMappingResolver(template));
                }

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

    private class SingleParameterMappingResolver : IMappingTypeResolver
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public SingleParameterMappingResolver(ICSharpFileBuilderTemplate template)
        {
            _template = template;
        }

        public ICSharpMapping? ResolveMappings(MappingModel mappingModel)
        {
            return new Mapping(mappingModel, _template);
        }
    }

    private class Mapping(MappingModel _mappingMappingModel, ICSharpTemplate _template) : CSharpMappingBase(_mappingMappingModel, _template)
    {
        public override CSharpStatement GetSourceStatement(bool? targetIsNullable = null)
        {
            // TODO JL: Test case for this
            if (Model.TypeReference?.IsCollection == true)
            {
                var mapping = new SelectToListMapping(_mappingMappingModel, _template)
                {
                    Parent = this.Parent
                };

                return mapping.GetSourceStatement();
            }

            return base.GetSourceStatement(targetIsNullable);

            //return GetAllChildren().First().GetSourceStatement(targetIsNullable);
        }
    }
}