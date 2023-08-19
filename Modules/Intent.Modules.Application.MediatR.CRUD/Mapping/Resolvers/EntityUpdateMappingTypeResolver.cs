using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
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
        if (mappingModel.MappingType != "update-mapping")
        {
            return null;
        }
        var model = mappingModel.Model;
        if (model.SpecializationType == "Class Constructor")
        {
            //return new ImplicitConstructorMapping(((IElement)mappingModel.Model).ParentElement, mappingModel.Mapping, mappingModel.Children, _sourceTemplate);
            return new ImplicitConstructorMapping(mappingModel, _sourceTemplate);
        }
        if (model.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(mappingModel, _sourceTemplate);
        }
        return new ObjectUpdateMapping(mappingModel.Model, mappingModel.Mapping, mappingModel.Children.Where(x => !x.Model.HasStereotype("Primary Key")).ToList(), _sourceTemplate);
    }
}