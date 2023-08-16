using System;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using JetBrains.Annotations;

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates;

public static class CommandMappingExtensions
{
    public static bool IsValidCommandMapping(this CommandModel commandModel)
    {
        if (commandModel == null) throw new ArgumentNullException(nameof(commandModel));
        
        return commandModel.Mapping?.Element?.IsClassModel() == true ||
               commandModel.Mapping?.Element?.IsClassConstructorModel() == true ||
               commandModel.Mapping?.Element?.IsOperationModel() == true;
    }

    public static ClassModel GetClassModel(this CommandModel commandModel)
    {
        if (commandModel == null) throw new ArgumentNullException(nameof(commandModel));
        
        return commandModel.Mapping?.Element.AsClassModel()
               ?? commandModel.Mapping?.Element.AsClassConstructorModel()?.ParentClass
               ?? commandModel.Mapping?.Element.AsOperationModel()?.ParentClass;
    }
}