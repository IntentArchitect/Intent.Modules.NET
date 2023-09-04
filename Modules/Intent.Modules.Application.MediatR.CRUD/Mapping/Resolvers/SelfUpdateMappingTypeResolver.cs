using System.Linq;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping.Resolvers;

public class SelfUpdateMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly CommandModel _command;

    public SelfUpdateMappingTypeResolver(ICSharpFileBuilderTemplate template, CommandModel command)
    {
        _template = template;
        _command = command;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        //if (mappingModel.MappingTypeId != "01721b1a-a85d-4320-a5cd-8bd39247196a" || 
        //    !mappingModel.Model.Equals(_command.InternalElement))
        //{
        //    return null;
        //}
        if (mappingModel.Model.SpecializationType == "Create Entity Action Target End")
        {
            return new MapChildrenMapping(mappingModel, _template);
        }

        if (mappingModel.Mapping?.FromPath.Last().Element.TypeReference?.IsCollection == true && mappingModel.Model.SpecializationType == "Operation")
        {
            return new ForLoopMethodInvocationMapping(mappingModel, _template);
        }

        return null;
    }
}