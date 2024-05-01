using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Blazor.Api;

public class BindingManager
{
    private readonly IRazorComponentTemplate _componentTemplate;

    public BindingManager(IRazorComponentTemplate template, IElementToElementMapping viewBinding)
    {
        _componentTemplate = template;
        ViewBinding = viewBinding;
    }

    public IElementToElementMapping ViewBinding { get; }

    public string GetBinding(IElementToElementMappedEnd mappedEnd, CSharpClassMappingManager mappingManager = null)
    {
        if (mappedEnd == null)
        {
            return null;
        }

        return (mappingManager ?? _componentTemplate.CreateMappingManager()).GenerateSourceStatementForMapping(ViewBinding, mappedEnd)?.ToString();
    }

    public IElementToElementMappedEnd GetMappedEndFor(IMetadataModel model)
    {
        return ViewBinding?.MappedEnds.SingleOrDefault(x => x.TargetElement?.Id == model.Id);
    }

    public IElementToElementMappedEnd GetMappedEndFor(IMetadataModel model, string stereotypePropertyName)
    {
        return ViewBinding?.MappedEnds.SingleOrDefault(x => x.TargetPath.Any(x => x.Id == model.Id) && x.TargetPath.Last().Name == stereotypePropertyName);
    }

    public string GetElementBinding(IMetadataModel model, CSharpClassMappingManager mappingManager = null)
    {
        var mappedEnd = GetMappedEndFor(model);
        return GetBinding(mappedEnd, mappingManager);
    }

    public string GetStereotypePropertyBinding(IMetadataModel model, string propertyName, CSharpClassMappingManager mappingManager = null)
    {
        var mappedEnd = GetMappedEndFor(model, propertyName);
        return GetBinding(mappedEnd, mappingManager);
    }
}