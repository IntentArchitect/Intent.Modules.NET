using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping.Resolvers;

public class EntityUpdateMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _sourceTemplate;

    public EntityUpdateMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
    {
        _sourceTemplate = sourceTemplate;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.MappingTypeId != "01721b1a-a85d-4320-a5cd-8bd39247196a")
        {
            return null;
        }
        var model = mappingModel.Model;
        if (model.SpecializationType == "Class" || model.TypeReference?.Element?.SpecializationType == "Class")
        {
            return new ObjectUpdateMapping(mappingModel.Model, mappingModel.Mapping, mappingModel.Children.Where(x => !x.Model.HasStereotype("Primary Key")).ToList(), _sourceTemplate);
        }
        if (model.SpecializationType == "Class Constructor")
        {
            //return new ImplicitConstructorMapping(((IElement)mappingModel.Model).ParentElement, mappingModel.Mapping, mappingModel.Children, _sourceTemplate);
            return new ImplicitConstructorMapping(mappingModel, _sourceTemplate);
        }
        if (model.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(mappingModel, _sourceTemplate);
        }

        return null;
    }
}