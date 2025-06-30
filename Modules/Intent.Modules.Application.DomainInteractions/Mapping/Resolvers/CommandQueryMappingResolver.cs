using System.Linq;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

public class CommandQueryMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CommandQueryMappingResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping? ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.Mapping == null ||
            mappingModel.Model.SpecializationTypeId is not (CommandModel.SpecializationTypeId or QueryModel.SpecializationTypeId))
        {
            return null;
        }

        if ((!_template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Command, mappingModel.Model.Id, out var templateInstance) &&
             !_template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Query, mappingModel.Model.Id, out templateInstance)) ||
            templateInstance.CSharpFile.Classes.FirstOrDefault(x => x.RepresentedModel?.Id == mappingModel.Model.Id)?.Constructors.Any(x => x.Parameters.Count > 0) != true)
        {
            return new ObjectInitializationMapping(mappingModel, _template);
        }

        return new ConstructorMapping(mappingModel, _template);
    }
}