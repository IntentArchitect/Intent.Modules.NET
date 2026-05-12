using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using System.Linq;

namespace Intent.Modules.Application.MediatR.CRUD.Eventing.MappingTypeResolvers;

public class InvocationMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public InvocationMappingTypeResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping? ResolveMappings(MappingModel mappingModel)
    {
        var model = mappingModel.Model;

        if (model.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(mappingModel, (ICSharpTemplate)_template);
        }

        return null;
    }
}
