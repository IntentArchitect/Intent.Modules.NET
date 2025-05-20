using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Exceptions;
using Intent.IArchitect.CrossPlatform.IO;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Common.Templates;
using static System.Collections.Specialized.BitVector32;

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

        return mappingManager.GenerateSourceStatementForMapping(ViewBinding, mappedEnd, isTargetNullable);
    }

    public CSharpStatement? GetBinding(IMetadataModel model, string mappableNameOrId, IRazorFileNode razorNode = null, bool? isTargetNullable = default)
    {
        var mappedEnd = GetMappedEndFor(model, mappableNameOrId);
        return GetBinding(mappedEnd, razorNode, isTargetNullable);
    }

    public IElementToElementMappedEnd? GetMappedEndForWithParent(IMetadataModel model, string? parentId)
    {
        var result = ViewBinding?.MappedEnds.Where(x => x.TargetElement?.Id == model.Id).ToList();
        if (parentId is not null && result?.Count() > 1)
        {
            return result?.SingleOrDefault(x => x.TargetPath.Any(tp => tp.Id == parentId));
        }
        return result?.SingleOrDefault();
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

    public CSharpStatement? GetElementBindingWithParent(IMetadataModel model, string? parentId = null, IRazorFileNode razorNode = null, bool? isTargetNullable = default)
    {
        var mappedEnd = GetMappedEndForWithParent(model, parentId);
        try
        {
            return GetBinding(mappedEnd, razorNode, isTargetNullable);
        }
        catch (Exception e)
        {

            if (mappedEnd is not null)
            {
                TryThrowElementException(mappedEnd, e);
            }
            throw;
        }
    }
    
    public CSharpStatement? GetElementBinding(IMetadataModel model, IRazorFileNode razorNode = null, bool? isTargetNullable = default)
    {
        var mappedEnd = GetMappedEndFor(model);
        try
        {
            return GetBinding(mappedEnd, razorNode, isTargetNullable);
        }
        catch(Exception e)
        {

            if (mappedEnd is not null)
            {
                TryThrowElementException(mappedEnd, e);
            }
            throw;
        }
    }

    private void TryThrowElementException(IElementToElementMappedEnd mappedEnd, Exception e)
    {
        var path = _componentTemplate.GetMetadata().GetRelativeFilePath();
        if (Path.DirectorySeparatorChar is '\\')
        {
            path = path.Replace('/', '\\');
        }

        if (_componentTemplate.TryGetModel<IElementWrapper>(out var element))
        {
            throw new ElementException(element.InternalElement, $"Unable to find binding for : '{string.Join(".", mappedEnd.TargetPath.Select(p => p.Name))}' [{path}]. If the underlying model has changed, try re-saving the mappings in `Manage View Bindings`", e);
        }
    }

    public CSharpStatement? GetElementBinding(IMetadataModel model, string mappableNameOrId, IRazorFileNode razorNode = null, bool? isTargetNullable = default)
    {
        var mappedEnd = GetMappedEndFor(model, mappableNameOrId);
        try
        {
            return GetBinding(mappedEnd, razorNode, isTargetNullable);
        }
        catch (Exception e)
        {

            if (mappedEnd is not null)
            {
                TryThrowElementException(mappedEnd, e);
            }
            throw;
        }
    }
}