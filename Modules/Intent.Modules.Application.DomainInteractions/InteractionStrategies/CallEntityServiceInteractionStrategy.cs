using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.DomainInteractions.InteractionStrategies;

[Obsolete("This is pure evil and should be removed. Should replace with Processing Actions in the designer.")]
public class CallEntityServiceInteractionStrategy : IInteractionStrategy
{
    private ICSharpFileBuilderTemplate _template;
    private CSharpClassMappingManager _csharpMapping;

    public bool IsMatch(IElement interaction)
    {
        return interaction.IsPerformInvocationTargetEndModel() && interaction.TypeReference.Element.SpecializationType == "Operation"
                                                               && ((IElement)interaction.TypeReference.Element).ParentElement.SpecializationType == "Class";
    }

    public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
    {
        var interaction = (IAssociationEnd)interactionElement;
        var @class = method.Class;
        _template = (ICSharpFileBuilderTemplate)@class.File.Template;
        _csharpMapping = method.GetMappingManager();
        //_csharpMapping.AddMappingResolver(new CallServiceOperationMappingResolver(_template));
        try
        {
            var statements = new List<CSharpStatement>();
            var serviceModel = ((IElement)interaction.TypeReference.Element).ParentElement;
            if (interaction.Mappings.Any() is false || !HasServiceDependency(serviceModel, out var serviceInterfaceTemplate))
            {
                return;
            }

            // So that the mapping system can resolve the name of the operation from the interface itself:
            _template.AddTypeSource(serviceInterfaceTemplate.Id);

            //if (serviceInterfaceTemplate.CSharpFile.Interfaces.Count == 1) {
            var serviceField = method.TrackedEntities().Values.LastOrDefault()?.VariableName;
            if (serviceField is null)
            {
                throw new ElementException(interactionElement, @"Call Service Operation performed without a prior call to ""Create"" or ""Query"" an Entity.");
            }
            var methodInvocation = _csharpMapping.GenerateCreationStatement(interaction.Mappings.First());
            CSharpStatement invoke = new CSharpAccessMemberStatement(serviceField, methodInvocation);

            var invStatement = methodInvocation as CSharpInvocationStatement;
            if (invStatement?.IsAsyncInvocation() == true)
            {
                invStatement.AddArgument("cancellationToken");
                invoke = new CSharpAwaitExpression(invoke);
            }

            var operationModel = interaction.TypeReference.Element;
            if (operationModel.TypeReference.Element != null)
            {
                var variableName = interaction.Name.ToLocalVariableName();
                _csharpMapping.SetFromReplacement(interaction, variableName);
                _csharpMapping.SetToReplacement(interaction, variableName);

                statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(variableName), invoke));

                //TrackedEntities.Add(interaction.Id, new EntityDetails((IElement)operationModel.TypeReference.Element, variableName, null, false, null, operationModel.TypeReference.IsCollection));
            }
            else if (invStatement?.Expression.Reference is ICSharpMethodDeclaration methodDeclaration &&
                     (methodDeclaration.ReturnTypeInfo.GetTaskGenericType() is CSharpTypeTuple || methodDeclaration.ReturnTypeInfo is CSharpTypeTuple))
            {
                var tuple = (CSharpTypeTuple)methodDeclaration.ReturnTypeInfo.GetTaskGenericType() ?? (CSharpTypeTuple)methodDeclaration.ReturnTypeInfo;
                var declaration = new CSharpDeclarationExpression(tuple.Elements.Select(s => s.Name.ToLocalVariableName()).ToList());
                statements.Add(new CSharpAssignmentStatement(declaration, invoke));
            }
            else
            {
                statements.Add(invoke);
            }

            //WireupDomainServicesForOperations(handlerClass, callServiceOperation, statements);

            method.AddStatements(ExecutionPhases.BusinessLogic, statements);
        }
        catch (Exception ex)
        {
            throw new ElementException(interaction, $"An error occurred while generating the interaction logic: {ex.Message}\nSee inner exception for more details.", ex);
        }
    }

    private const string DomainServiceSpecializationId = "07f936ea-3756-48c8-babd-24ac7271daac";
    private const string ApplicationServiceSpecializationId = "b16578a5-27b1-4047-a8df-f0b783d706bd";
    private const string EntitySpecializationId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10";
    private const string RepositorySpecializationId = "96ffceb2-a70a-4b69-869b-0df436c470c3";
    private const string ServiceProxySpecializationId = "07d8d1a9-6b9f-4676-b7d3-8db06299e35c";

    bool HasServiceDependency(IElement serviceModel, out ICSharpFileBuilderTemplate dependencyInfo)
    {
        switch (serviceModel.SpecializationTypeId)
        {
            case DomainServiceSpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.DomainServices.Interface, serviceModel, out var domainServiceTemplate):
                dependencyInfo = domainServiceTemplate;
                return true;
            case ApplicationServiceSpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.Interface, serviceModel, out var applicationServiceTemplate):
                dependencyInfo = applicationServiceTemplate;
                return true;
            case RepositorySpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Repository.Interface.Entity, serviceModel, out var repositoryTemplate):
                dependencyInfo = repositoryTemplate;
                return true;
            case EntitySpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, serviceModel, out var entityTemplate):
                dependencyInfo = entityTemplate;
                return true;
            case ServiceProxySpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.ClientInterface, serviceModel, out var clientInterfaceTemplate):
                dependencyInfo = clientInterfaceTemplate;
                return true;
            default:
                dependencyInfo = default;
                return false;
        }
    }
}