using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Eventing.Contracts.InteractionStrategies;

public static class DomainServiceInjectionExtensions
{
    private const string DomainServiceSpecializationId = "07f936ea-3756-48c8-babd-24ac7271daac";

    public static void WireupDomainServicesForConstructors(this CSharpClass handlerClass, CreateEntityActionTargetEndModel createAction, CSharpStatement constructionStatement)
    {
        var constructor = createAction.Element.AsClassConstructorModel();
        if (constructor != null)
        {
            WireupDomainService(handlerClass.File.Template, constructionStatement as CSharpInvocationStatement, constructor.Parameters, handlerClass);
        }
    }

    public static void WireupDomainServicesForOperations(this CSharpClass handlerClass, UpdateEntityActionTargetEndModel updateAction, IList<CSharpStatement> updateStatements)
    {
        Func<CSharpInvocationStatement, Intent.Modelers.Domain.Api.OperationModel> getOperation;
        if (OperationModelExtensions.IsOperationModel(updateAction.Element))
        {
            var operation = OperationModelExtensions.AsOperationModel(updateAction.Element);
            if (operation == null)
            {
                return;
            }
            if (operation.Parameters.All(p => p.TypeReference.Element.SpecializationTypeId != DomainServiceSpecializationId))
            {
                return;
            }
            getOperation = (x) => operation;
        }
        else
        {
            var updateMappings = updateAction.Mappings.GetUpdateEntityMapping();
            var mappedOperations = updateMappings.MappedEnds.Where(me => OperationModelExtensions.IsOperationModel(me.TargetElement)).Select(me => OperationModelExtensions.AsOperationModel(me.TargetElement)).ToList();

            if (!mappedOperations.Any())
            {
                return;
            }
            if (!mappedOperations.Any(o => o.Parameters.Any(p => p.TypeReference.Element.SpecializationTypeId == DomainServiceSpecializationId)))
            {
                return;
            }

            getOperation = (invocation) =>
            {
                string operationName = invocation.Expression.Reference is ICSharpMethodDeclaration iCSharpMethodDeclaration ? iCSharpMethodDeclaration.Name.ToCSharpIdentifier() : null;
                return mappedOperations.FirstOrDefault(operation => operation.Name == operationName);
            };
        }

        foreach (var updateStatement in updateStatements)
        {
            if (updateStatement is not CSharpInvocationStatement invocation)
            {
                continue;
            }
            var operation = getOperation(invocation);

            if (operation == null)
            {
                continue;
            }
            WireupDomainService(handlerClass.File.Template, invocation, operation.Parameters, handlerClass);
        }
    }

    public static void WireupDomainServicesForProcessingAction(this CSharpClass handlerClass, IElementToElementMapping mapping, IList<CSharpStatement> processingActions)
    {
        var mappedOperations = mapping.MappedEnds.Where(me => OperationModelExtensions.IsOperationModel(me.TargetElement)).Select(me => OperationModelExtensions.AsOperationModel(me.TargetElement)).ToList();

        if (!mappedOperations.Any())
        {
            return;
        }
        if (!mappedOperations.Any(o => o.Parameters.Any(p => p.TypeReference.Element.SpecializationTypeId == DomainServiceSpecializationId)))
        {
            return;
        }


        foreach (var updateStatement in processingActions)
        {
            if (updateStatement is not CSharpInvocationStatement invocation)
            {
                continue;
            }
            string operationName = invocation.Expression.Reference is ICSharpMethodDeclaration iCSharpMethodDeclaration ? iCSharpMethodDeclaration.Name.ToCSharpIdentifier() : null;
            var operation = mappedOperations.FirstOrDefault(operation => operation.Name == operationName);

            if (operation == null)
            {
                continue;
            }
            WireupDomainService(handlerClass.File.Template, invocation, operation.Parameters, handlerClass);
        }
    }

    private static void WireupDomainServicesForOperations(CSharpClass handlerClass, IAssociationEnd callServiceOperation, List<CSharpStatement> statements)
    {
        var operation = OperationModelExtensions.AsOperationModel(callServiceOperation.TypeReference.Element);
        if (operation == null)
        {
            return;
        }
        if (operation.Parameters.All(p => p.TypeReference.Element.SpecializationTypeId != DomainServiceSpecializationId))
        {
            return;
        }

        foreach (var statement in statements)
        {
            SubstituteServiceParameters(statement);
        }

        return;

        void SubstituteServiceParameters(CSharpStatement statement)
        {
            switch (statement)
            {
                case CSharpAssignmentStatement assign:
                    SubstituteServiceParameters(assign.Rhs);
                    return;
                case CSharpAccessMemberStatement access:
                {
                    SubstituteServiceParameters(access.Member);
                    return;
                }
                default:
                    break;
            }

            var invocation = statement as CSharpInvocationStatement;
            if (invocation is null)
            {
                return;
            }

            WireupDomainService(handlerClass.File.Template, invocation, operation.Parameters, handlerClass);
        }
    }

    private static void WireupDomainService(ICSharpTemplate _template, CSharpInvocationStatement invocation, IList<Intent.Modelers.Domain.Api.ParameterModel> parameters, CSharpClass handlerClass)
    {
        if (invocation is null)
        {
            return;
        }

        for (var i = 0; i < parameters.Count; i++)
        {
            var arg = parameters[i];
            if (arg.TypeReference.Element.SpecializationTypeId != DomainServiceSpecializationId)
            {
                continue;
            }

            if (!_template.TryGetTypeName(TemplateRoles.Domain.DomainServices.Interface, arg.TypeReference.Element.Id, out var domainServiceInterface))
            {
                continue;
            }
            var fieldName = handlerClass.InjectService(domainServiceInterface, domainServiceInterface.Substring(1).ToParameterName());
            //Change `default` or `parameterName: default` into `_domainService` (fieldName)
            invocation.Statements[i].Replace(invocation.Statements[i].GetText("").Replace("default", fieldName));
        }
    }
}