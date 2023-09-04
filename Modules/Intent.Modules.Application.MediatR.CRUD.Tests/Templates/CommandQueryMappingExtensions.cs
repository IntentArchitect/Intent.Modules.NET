using System;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates;

public static class CommandQueryMappingExtensions
{
    public static bool IsValidCommandMapping(this CommandModel commandModel)
    {
        if (commandModel == null) throw new ArgumentNullException(nameof(commandModel));
        
        return commandModel.Mapping?.Element?.IsClassModel() == true ||
               commandModel.Mapping?.Element?.IsClassConstructorModel() == true ||
               OperationModelExtensions.IsOperationModel(commandModel.Mapping?.Element);
    }

    public static ClassModel GetClassModel(this CommandModel commandModel)
    {
        if (commandModel == null) throw new ArgumentNullException(nameof(commandModel));
        
        return commandModel.Mapping?.Element.AsClassModel()
               ?? commandModel.Mapping?.Element.AsClassConstructorModel()?.ParentClass
               ?? OperationModelExtensions.AsOperationModel(commandModel.Mapping?.Element)?.ParentClass;
    }

    public static ClassModel GetClassModel(this QueryModel queryModel)
    {
        if (queryModel == null) throw new ArgumentNullException(nameof(queryModel));
        
        return queryModel.Mapping?.Element.AsClassModel()
               ?? queryModel.TypeReference?.Element?.AsDTOModel()?.Mapping?.Element?.AsClassModel()
               ?? queryModel.GetPaginatedClassModel();
    }
    
    const string pagedResultTypeDefinitionId = "9204e067-bdc8-45e7-8970-8a833fdc5253";

    public static bool IsPaginationQueryMapping(this QueryModel queryModel)
    {
        return queryModel.TypeReference?.Element?.Id == pagedResultTypeDefinitionId &&
               queryModel.TypeReference?.GenericTypeParameters
                   ?.FirstOrDefault()?.Element?.AsDTOModel()?.Mapping?.Element?.IsClassModel() == true;
    }

    public static ClassModel GetPaginatedClassModel(this QueryModel queryModel)
    {
        if (queryModel.TypeReference?.Element?.Id != pagedResultTypeDefinitionId)
        {
            return null;
        }
        return queryModel.TypeReference?.GenericTypeParameters
                   ?.FirstOrDefault()?.Element?.AsDTOModel()?.Mapping?.Element?.AsClassModel();
    }

    public static bool HasIdentityKeys(this QueryModel queryModel)
    {
        var classModel = queryModel.GetClassModel();
        return classModel is not null && queryModel.Properties.GetEntityIdFields(classModel).Any();
    }
    
    public static bool HasIdentityKeys(this CommandModel commandModel)
    {
        var classModel = commandModel.GetClassModel();
        return classModel is not null && commandModel.Properties.GetEntityIdFields(classModel).Any();
    }
}