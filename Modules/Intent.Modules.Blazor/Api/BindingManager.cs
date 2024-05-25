using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
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

    public CSharpStatement GetEventEmitterBinding(IElementToElementMappedEnd mappedEnd, IRazorFileNode razorNode = null)
    {
        return GetBinding(mappedEnd, razorNode).ToLambda();
    }

    public CSharpStatement GetBinding(IElementToElementMappedEnd mappedEnd, IRazorFileNode razorNode = null)
    {
        if (mappedEnd == null)
        {
            return null;
        }

        var mappingManager = _componentTemplate.CreateMappingManager();
        if (razorNode != null)
        {
            foreach (var mappingReplacement in razorNode.GetMappingReplacements())
            {
                mappingManager.SetFromReplacement(mappingReplacement.Key, mappingReplacement.Value);
                mappingManager.SetToReplacement(mappingReplacement.Key, mappingReplacement.Value);
            }
        }

        return mappingManager.GenerateSourceStatementForMapping(ViewBinding, mappedEnd)?.ToString();
    }

    public IElementToElementMappedEnd GetMappedEndFor(IMetadataModel model)
    {
        return ViewBinding?.MappedEnds.SingleOrDefault(x => x.TargetElement?.Id == model.Id);
    }

    public IElementToElementMappedEnd GetMappedEndFor(IMetadataModel model, string stereotypePropertyName)
    {
        return ViewBinding?.MappedEnds.SingleOrDefault(x => x.TargetPath.Any(x => x.Id == model.Id) && x.TargetPath.Last().Name == stereotypePropertyName);
    }

    public CSharpStatement GetElementBinding(IMetadataModel model, IRazorFileNode razorNode = null)
    {
        var mappedEnd = GetMappedEndFor(model);
        return GetBinding(mappedEnd, razorNode);
    }

    public CSharpStatement GetStereotypePropertyBinding(IMetadataModel model, string propertyName, IRazorFileNode razorNode = null)
    {
        var mappedEnd = GetMappedEndFor(model, propertyName);
        return GetBinding(mappedEnd, razorNode);
    }
}