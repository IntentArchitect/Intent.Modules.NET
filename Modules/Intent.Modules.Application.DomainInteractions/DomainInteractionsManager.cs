using System;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.InteractionStrategies;
using Intent.Templates;
using Intent.Utils;
using JetBrains.Annotations;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;
using AttributeModel = Intent.Modelers.Domain.Api.AttributeModel;

namespace Intent.Modules.Application.DomainInteractions;
// Disambiguation
using Intent.Modelers.Domain.Api;
using System.ComponentModel.Design;
using System.Drawing;

public static class DomainInteractionExtensions
{

    public static bool HasDomainInteractions(this IProcessingHandlerModel model)
    {
        return model.CreateEntityActions().Any()
               || model.QueryEntityActions().Any()
               || model.UpdateEntityActions().Any()
               || model.DeleteEntityActions().Any()
               || model.ServiceInvocationActions().Any()
               || model.CallServiceOperationActions().Any();
    }
}

public class DomainInteractionsManager
{
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly CSharpClassMappingManager _csharpMapping;

    public DomainInteractionsManager(ICSharpFileBuilderTemplate template, CSharpClassMappingManager csharpMapping)
    {
        _template = template;
        _csharpMapping = csharpMapping;
    }

    //public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();

    private const string ApplicationServiceSpecializationId = "b16578a5-27b1-4047-a8df-f0b783d706bd";
    private const string EntitySpecializationId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10";
    private const string RepositorySpecializationId = "96ffceb2-a70a-4b69-869b-0df436c470c3";
    private const string ServiceProxySpecializationId = "07d8d1a9-6b9f-4676-b7d3-8db06299e35c";

    public IEnumerable<CSharpStatement> CreateInteractionStatements(CSharpClassMethod method, IProcessingHandlerModel model)
    {
        var handlerClass = method.Class;
        var domainInteractionManager = this;
        var statements = new List<CSharpStatement>();
        try
        {
            //foreach (var queryAction in model.QueryEntityActions())
            //{
            //    if (queryAction.Mappings.GetQueryEntityMapping() == null)
            //    {
            //        continue;
            //    }

            //    if (!queryAction.Element.IsClassModel())
            //    {
            //        continue;
            //    }

            //    var foundEntity = queryAction.Element.AsClassModel();
            //    statements.AddRange(domainInteractionManager.QueryEntity(new QueryActionContext(method, ActionType.Query, queryAction.InternalAssociationEnd)));
            //}

            //foreach (var createAction in model.CreateEntityActions())
            //{
            //    statements.AddRange(domainInteractionManager.CreateEntity(method, createAction));
            //}

            //foreach (var updateAction in model.UpdateEntityActions())
            //{
            //    statements.AddRange(domainInteractionManager.QueryEntity(new QueryActionContext(method, ActionType.Update, updateAction.InternalAssociationEnd)));

            //    statements.Add(string.Empty);
            //    statements.AddRange(domainInteractionManager.UpdateEntity(method, updateAction));
            //}

            //foreach (var callAction in model.ServiceInvocationActions())
            //{
            //    statements.AddRange(domainInteractionManager.InvokeService(handlerClass, callAction.InternalAssociationEnd));
            //}


            //foreach (var callAction in model.CallServiceOperationActions())
            //{
            //    statements.AddRange(domainInteractionManager.CallServiceOperation(handlerClass, callAction.InternalAssociationEnd));
            //}

            //foreach (var deleteAction in model.DeleteEntityActions())
            //{
            //    statements.AddRange(domainInteractionManager.QueryEntity(new QueryActionContext(method, ActionType.Delete, deleteAction.InternalAssociationEnd)));
            //    statements.AddRange(domainInteractionManager.DeleteEntity(method, deleteAction));
            //}

            foreach (var actions in model.ProcessingActions().Where(x => x.InternalElement.Mappings.Count() == 1))
            {
                try
                {
                    var processingStatements = _csharpMapping.GenerateUpdateStatements(actions.InternalElement.Mappings.Single())
                    .Select(x =>
                    {
                        if (x is CSharpAssignmentStatement)
                        {
                            x.WithSemicolon();
                        }

                        return x;
                    }).ToList();

                    handlerClass.WireupDomainServicesForProcessingAction(actions.InternalElement.Mappings.Single(), processingStatements);
                    processingStatements.FirstOrDefault()?.SeparatedFromPrevious();
                    statements.AddRange(processingStatements);

                }
                catch (Exception ex)
                {
                    throw new ElementException(actions.InternalElement, "An error occurred while generating processing action logic", ex);
                }
            }

            //foreach (var entity in method.TrackedEntities().Values.Where(x => x.IsNew))
            //{
            //    statements.Add(entity.DataAccessProvider.AddEntity(entity.VariableName).SeparatedFromPrevious());
            //}

            return statements;
        }
        catch (ElementException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ElementException(model.InternalElement, "An error occurred while generating the domain interactions logic. See inner exception for more details", ex);
        }
    }


    public IEnumerable<CSharpStatement> GetReturnStatements(CSharpClassMethod method, ITypeReference returnType)
    {
        if (returnType.Element == null)
        {
            throw new Exception("No return type specified");
        }
        var statements = new List<CSharpStatement>();

        var entitiesReturningPk = GetEntitiesReturningPK(method, returnType);
        var nonUserSuppliedEntitiesReturningPks = GetEntitiesReturningPK(method, returnType, isUserSupplied: false);

        foreach (var entity in nonUserSuppliedEntitiesReturningPks.Where(x => x.IsNew).GroupBy(x => x.ElementModel.Id).Select(x => x.First()))
        {
            if (entity.ElementModel.IsClassModel())
            {
                var primaryKeys = entity.ElementModel.AsClassModel().GetTypesInHierarchy().SelectMany(c => c.Attributes.Where(a => a.IsPrimaryKey()));
                if (primaryKeys.Any(p => HasDBGeneratedPk(p)))
                {
                    statements.Add($"{entity.DataAccessProvider.SaveChangesAsync()}");
                }
            }
            else
            {
                statements.Add($"{entity.DataAccessProvider.SaveChangesAsync()}");
            }
        }

        if (returnType.Element.AsDTOModel()?.IsMapped == true &&
            method.TrackedEntities().Values.Any(x => x.ElementModel?.Id == returnType.Element.AsDTOModel().Mapping.ElementId) &&
            _template.TryGetTypeName("Application.Contract.Dto", returnType.Element, out var returnDto))
        {
            var entityDetails = method.TrackedEntities().Values.First(x => x.ElementModel?.Id == returnType.Element.AsDTOModel().Mapping.ElementId);
            if (entityDetails.ProjectedType == returnDto)
            {
                statements.Add($"return {entityDetails.VariableName};");
            }
            else
            {
                //Adding Using Clause for Extension Methods
                _template.TryGetTypeName("Intent.Application.Dtos.EntityDtoMappingExtensions", returnType.Element, out var _);
                var autoMapperFieldName = method.Class.InjectService(_template.UseType("AutoMapper.IMapper"));
                string nullable = returnType.IsNullable ? "?" : "";
                statements.Add($"return {entityDetails.VariableName}{nullable}.MapTo{returnDto}{(returnType.IsCollection ? "List" : "")}({autoMapperFieldName});");
            }
        }
        else if (returnType.IsResultPaginated() &&
                 returnType.GenericTypeParameters.FirstOrDefault()?.Element.AsDTOModel()?.IsMapped == true &&
                 method.TrackedEntities().Values.Any(x => x.ElementModel?.Id == returnType.GenericTypeParameters.First().Element.AsDTOModel().Mapping.ElementId) &&
                 _template.TryGetTypeName("Application.Contract.Dto", returnType.GenericTypeParameters.First().Element, out returnDto))
        {
            var entityDetails = method.TrackedEntities().Values.First(x => x.ElementModel.Id == returnType.GenericTypeParameters.First().Element.AsDTOModel().Mapping.ElementId);
            if (entityDetails.ProjectedType == returnDto)
            {
                statements.Add($"return {entityDetails.VariableName}.MapToPagedResult();");
            }
            else
            {
                var autoMapperFieldName = method.Class.InjectService(_template.UseType("AutoMapper.IMapper"));
                statements.Add($"return {entityDetails.VariableName}.MapToPagedResult(x => x.MapTo{returnDto}({autoMapperFieldName}));");
            }
        }
        else if (returnType.Element.IsTypeDefinitionModel() && (nonUserSuppliedEntitiesReturningPks.Count == 1 || entitiesReturningPk.Count == 1)) // No need for TrackedEntities thus no check for it
        {
            var entityDetails = nonUserSuppliedEntitiesReturningPks.Count == 1
                ? nonUserSuppliedEntitiesReturningPks[0]
                : entitiesReturningPk[0];
            var entity = entityDetails.ElementModel.AsClassModel();
            statements.Add($"return {entityDetails.VariableName}.{entity.GetTypesInHierarchy().SelectMany(x => x.Attributes).FirstOrDefault(x => x.IsPrimaryKey(isUserSupplied: false))?.Name ?? "Id"};");
        }
        else if (method.TrackedEntities().Values.Any(x => returnType.Element.Id == x.ElementModel.Id))
        {
            var entityDetails = method.TrackedEntities().Values.First(x => returnType.Element.Id == x.ElementModel.Id);
            statements.Add($"return {entityDetails.VariableName};");
        }
        else
        {
            statements.Add(new CSharpStatement($"// TODO: Implement return type mapping...").SeparatedFromPrevious());
            statements.Add("throw new NotImplementedException(\"Implement return type mapping...\");");
        }

        return statements;
    }

    private List<EntityDetails> GetEntitiesReturningPK(CSharpClassMethod method, ITypeReference returnType, bool? isUserSupplied = null)
    {
        if (returnType.Element.IsDTOModel())
        {
            var dto = returnType.Element.AsDTOModel();

            var mappedPks = dto.Fields
                .Where(x => x.Mapping != null && Intent.Modelers.Domain.Api.AttributeModelExtensions.IsAttributeModel(x.Mapping.Element) && Intent.Modelers.Domain.Api.AttributeModelExtensions.AsAttributeModel(x.Mapping.Element).IsPrimaryKey(isUserSupplied))
                .Select(x => Intent.Modelers.Domain.Api.AttributeModelExtensions.AsAttributeModel(x.Mapping.Element).InternalElement.ParentElement.Id)
                .Distinct()
                .ToList();
            if (mappedPks.Any())
            {
                return method.TrackedEntities().Values
                .Where(x => x.ElementModel.IsClassModel() && mappedPks.Contains(x.ElementModel.Id))
                .ToList();
            }
            return new List<EntityDetails>();
        }
        return method.TrackedEntities().Values
            .Where(x => x.ElementModel.AsClassModel()?.GetTypesInHierarchy()
                .SelectMany(c => c.Attributes)
                .Count(a => a.IsPrimaryKey(isUserSupplied) && a.TypeReference.Element.Id == returnType.Element.Id) == 1)
            .ToList();
    }

    private bool HasDBGeneratedPk(AttributeModel attribute)
    {
        return attribute.IsPrimaryKey() && attribute.GetStereotypeProperty("Primary Key", "Data source", "Default") == "Default";
    }
}

public static class CSharpClassMethodExtensions
{
    public static Dictionary<string, EntityDetails> TrackedEntities(this CSharpClassMethod method)
    {
        if (!method.TryGetMetadata<Dictionary<string, EntityDetails>>("tracked-entities", out var trackedEntities))
        {
            trackedEntities = new Dictionary<string, EntityDetails>();
            if (method.HasMetadata("tracked-entities"))
            {
                method.Metadata.Remove("tracked-entities");
            }
            method.AddMetadata("tracked-entities", trackedEntities);
        }

        return trackedEntities;
    }
}


internal static class AttributeModelExtensions
{
    public static bool IsPrimaryKey(this AttributeModel attribute, bool? isUserSupplied = null)
    {
        if (!attribute.HasStereotype("Primary Key"))
        {
            return false;
        }

        if (!isUserSupplied.HasValue)
        {
            return true;
        }

        if (!attribute.GetStereotype("Primary Key").TryGetProperty("Data source", out var property))
        {
            return isUserSupplied == false;
        }

        return property.Value == "User supplied" == isUserSupplied.Value;
    }

    public static bool IsForeignKey(this AttributeModel attribute)
    {
        return attribute.HasStereotype("Foreign Key");
    }

    public static AssociationTargetEndModel GetForeignKeyAssociation(this AttributeModel attribute)
    {
        return attribute.GetStereotype("Foreign Key")?.GetProperty<IElement>("Association")?.AsAssociationTargetEndModel();
    }

    public static string AsSingleOrTuple(this IEnumerable<CSharpStatement> idFields)
    {
        if (idFields.Count() <= 1)
            return $"{idFields.Single()}";
        return $"({string.Join(", ", idFields.Select(idField => $"{idField}"))})";
    }
}

internal record AggregateKeyMapping(AttributeModel Key, string Match);