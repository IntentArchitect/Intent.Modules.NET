using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.DomainInteractions.Extensions;

public static class DomainServiceInjectionExtensions
{
    private const string DomainServiceSpecializationId = "07f936ea-3756-48c8-babd-24ac7271daac";

    public static void WireupDomainServicesForConstructors(this ICSharpClass handlerClass, CreateEntityActionTargetEndModel createAction, CSharpStatement constructionStatement)
    {
        var constructor = createAction.Element.AsClassConstructorModel();
        if (constructor != null)
        {
            WireupDomainService(handlerClass.File.Template, constructionStatement as CSharpInvocationStatement, constructor.Parameters, handlerClass);
        }
    }

    public static void WireupDomainServicesForOperations(this ICSharpClass handlerClass, UpdateEntityActionTargetEndModel updateAction, IList<CSharpStatement> updateStatements)
    {
        Func<CSharpInvocationStatement, OperationModel> getOperation;
        if (updateAction.Element.IsOperationModel())
        {
            var operation = updateAction.Element.AsOperationModel();
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
            var mappedOperations = updateMappings.MappedEnds.Where(me => me.TargetElement.IsOperationModel()).Select(me => me.TargetElement.AsOperationModel()).ToList();

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

    public static void WireupDomainServicesForProcessingAction(this ICSharpClass handlerClass, IElementToElementMapping mapping, IList<CSharpStatement> processingActions)
    {
        var mappedOperations = mapping.MappedEnds.Where(me => me.TargetElement.IsOperationModel()).Select(me => me.TargetElement.AsOperationModel()).ToList();

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

    private static void WireupDomainServicesForOperations(ICSharpClass handlerClass, IAssociationEnd callServiceOperation, List<CSharpStatement> statements)
    {
        var operation = callServiceOperation.TypeReference.Element.AsOperationModel();
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

    private static void WireupDomainService(ICSharpTemplate _template, CSharpInvocationStatement invocation, IList<ParameterModel> parameters, ICSharpClass handlerClass)
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