using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Api;

public class BindingManager
{
    private readonly IRazorComponentTemplate _componentTemplate;

    public BindingManager(IRazorComponentTemplate template, IElementToElementMapping? viewBinding)
    {
        _componentTemplate = template;
        ViewBinding = viewBinding;
    }

    public IElementToElementMapping ViewBinding { get; }

    public CSharpStatement? GetEventEmitterBinding(IElementToElementMappedEnd mappedEnd, IRazorFileNode razorNode = null)
    {
        return GetBinding(mappedEnd, razorNode).ToLambda();
    }

    public CSharpStatement? GetBinding(IElementToElementMappedEnd? mappedEnd, IRazorFileNode? razorNode = null, bool? isTargetNullable = default)
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

        return mappingManager.GenerateSourceStatementForMapping(ViewBinding, mappedEnd, isTargetNullable)?.ToString();
    }

    public CSharpStatement? GetBinding(IMetadataModel model, string mappableNameOrId, IRazorFileNode razorNode = null, bool? isTargetNullable = default)
    {
        var mappedEnd = GetMappedEndFor(model, mappableNameOrId);
        return GetBinding(mappedEnd, razorNode, isTargetNullable);
    }

    public IElementToElementMappedEnd? GetMappedEndFor(IMetadataModel model)
    {
        return ViewBinding?.MappedEnds.SingleOrDefault(x => x.TargetElement?.Id == model.Id);
    }

    public IElementToElementMappedEnd? GetMappedEndFor(IMetadataModel model, string mappableNameOrId)
    {
        return ViewBinding?.MappedEnds.SingleOrDefault(x => x.TargetPath.Any(x => x.Id == model.Id) && (x.TargetPath.Last().Name == mappableNameOrId || x.TargetPath.Last().Id == mappableNameOrId));
    }

    public IList<IElementToElementMappedEnd> GetMappedEndsFor(IMetadataModel model, string mappableNameOrId)
    {
        return ViewBinding?.MappedEnds.Where(x => x.TargetPath.Any(x => x.Id == model.Id) && x.TargetPath.Any(x => x.Name == mappableNameOrId || x.Id == mappableNameOrId))
            .ToList() ?? [];
    }

    public CSharpStatement? GetElementBinding(IMetadataModel model, IRazorFileNode razorNode = null, bool? isTargetNullable = default)
    {
        var mappedEnd = GetMappedEndFor(model);
        return GetBinding(mappedEnd, razorNode, isTargetNullable);
    }

    public CSharpStatement? GetElementBinding(IMetadataModel model, string mappableNameOrId, IRazorFileNode razorNode = null, bool? isTargetNullable = default)
    {
        var mappedEnd = GetMappedEndFor(model, mappableNameOrId);
        return GetBinding(mappedEnd, razorNode, isTargetNullable);
    }
}