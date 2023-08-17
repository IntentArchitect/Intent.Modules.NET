using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public abstract class MappingManagerBase
{
    protected Dictionary<ICanBeReferencedType, string> _fromReplacements = new();
    protected Dictionary<ICanBeReferencedType, string> _toReplacements = new();

    protected MappingManagerBase()
    {
    }

    public CSharpStatement GetCreationStatement(IElementToElementMapping model)
    {
        var mapping = CreateMapping(model.ToElement, model.Connections, GetCreateMappingType);
        ApplyReplacements(mapping);

        return mapping.GetFromStatement();
    }

    public IList<CSharpStatement> GetUpdateStatements(IElementToElementMapping model)
    {
        var mapping = CreateMapping(model.ToElement, model.Connections, GetUpdateMappingType);
        ApplyReplacements(mapping);

        return mapping.GetMappingStatement().ToList();
    }

    protected abstract ICSharpMapping GetCreateMappingType(ICanBeReferencedType model, IElementToElementMappingConnection mapping, List<ICSharpMapping> children);
    protected abstract ICSharpMapping GetUpdateMappingType(ICanBeReferencedType model, IElementToElementMappingConnection mapping, List<ICSharpMapping> children);

    public void SetFromReplacement(ICanBeReferencedType type, string replacement)
    {
        if (_fromReplacements.ContainsKey(type))
        {
            _fromReplacements.Remove(type);
        }
        _fromReplacements.Add(type, replacement);
    }

    public void SetToReplacement(ICanBeReferencedType type, string replacement)
    {
        if (_toReplacements.ContainsKey(type))
        {
            _toReplacements.Remove(type);
        }
        _toReplacements.Add(type, replacement);
    }

    private ICSharpMapping CreateMapping(
        ICanBeReferencedType model, 
        IList<IElementToElementMappingConnection> mappings, 
        Func<ICanBeReferencedType, IElementToElementMappingConnection, List<ICSharpMapping>, ICSharpMapping> mappingResolver, 
        int level = 1)
    {
        var mapping = mappings.SingleOrDefault(x => x.ToPath.Last().Element == model);
        var children = mappings.Where(x => x.ToPath.Count > level)
            .GroupBy(x => x.ToPath.Skip(level).First(), x => x)
            .Select(x => CreateMapping(x.Key.Element, x.ToList(), mappingResolver, level + 1))
            .ToList();
        return mappingResolver(model, mapping, children.OrderBy(x => ((IElement)x.Model).Order).ToList());
    }

    private void ApplyReplacements(ICSharpMapping mapping)
    {
        foreach (var fromReplacement in _fromReplacements)
        {
            mapping.SetFromReplacement(fromReplacement.Key, fromReplacement.Value);
        }

        foreach (var toReplacement in _toReplacements)
        {
            mapping.SetToReplacement(toReplacement.Key, toReplacement.Value);
        }
    }
}