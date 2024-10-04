using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Api.Mappings;

public class PropertyCollectionMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpTemplate _template;

    public PropertyCollectionMappingResolver(ICSharpTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.Model.SpecializationType is "Add()" or "Remove()")
        {
            return new ListMethodInvocationMapping(mappingModel, _template);
        }

        return null;
    }
}

public class ListMethodInvocationMapping : MethodInvocationMapping
{
    public ListMethodInvocationMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
    }

    protected override bool TryGetReferenceName(IElementMappingPathTarget mappingPath, out string reference)
    {
        if (mappingPath.Id == "36d22f9e-3984-477b-af04-b33b94401f72" || mappingPath.Name == "Add()")
        {
            reference = "Add";
            return true;
        }

        if (mappingPath.Id == "ff4ca36e-bb24-4f42-afac-c676d586c812" || mappingPath.Name == "Remove()")
        {
            reference = "Remove";
            return true;
        }

        return base.TryGetReferenceName(mappingPath, out reference);
    }
}