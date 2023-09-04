﻿using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Eventing.FactoryExtensions;

public class MessageCreationMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _sourceTemplate;

    public MessageCreationMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
    {
        _sourceTemplate = sourceTemplate;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        //if (mappingModel.MappingTypeId != "5f172141-fdba-426b-980e-163e782ff53e") // Command to Class Creation Mapping
        //{
        //    return null;
        //}

        var model = mappingModel.Model;
        if (model.SpecializationType == "Message" || model.TypeReference?.Element?.SpecializationType == "Eventing DTO")
        {
            return new ObjectInitializationMapping(mappingModel, _sourceTemplate);
        }
        if (model.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(mappingModel, _sourceTemplate);
        }

        return null;
    }
}