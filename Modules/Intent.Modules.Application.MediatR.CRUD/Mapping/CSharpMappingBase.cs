using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public abstract class CSharpMappingBase : ICSharpMapping
{
    protected Dictionary<ICanBeReferencedType, string> _fromReplacements = new();
    protected Dictionary<ICanBeReferencedType, string> _toReplacements = new();

    public ICanBeReferencedType Model { get; }
    public IList<ICSharpMapping> Children { get; }
    public IElementToElementMappingConnection Mapping { get; set; }

    protected CSharpMappingBase(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children)
    {
        Model = model;
        Mapping = mapping;
        Children = children;
    }

    public virtual IEnumerable<CSharpStatement> GetMappingStatement(IDictionary<ICanBeReferencedType, string> fromReplacements, IDictionary<ICanBeReferencedType, string> toReplacements)
    {
        throw new NotImplementedException();
    }

    public virtual CSharpStatement GetFromStatement()
    {
        throw new NotImplementedException();
    }

    public virtual CSharpStatement GetToStatement()
    {
        if (Mapping != null)
        {
            return GetToPath(_toReplacements);
        }

        var mapping = Children.First(x => x.Mapping != null).Mapping;
        var toPath = mapping.ToPath.Take(mapping.ToPath.IndexOf(mapping.ToPath.Single(x => x.Element == Model)) + 1).ToList();
        return GetPath(GetToPath(), _toReplacements);
    }

    protected IList<IElementMappingPathTarget> GetFromPath()
    {
        if (Mapping != null)
        {
            return Mapping.FromPath;
        }

        var mapping = Children.First(x => x.Mapping != null).Mapping;
        if (Children.Count(x => x.Mapping != null) == 1)
        {
            return mapping.FromPath.Take(mapping.FromPath.Count - 1).ToList();
        }
        var toPath = mapping.FromPath.Take(mapping.FromPath.IndexOf(mapping.FromPath
            .Last(x => Children.Where(c => c.Mapping != null).All(c => c.Mapping.FromPath.Contains(x)))) + 1)
            .ToList();
        return toPath;
    }

    protected IList<IElementMappingPathTarget> GetToPath()
    {
        if (Mapping != null)
        {
            return Mapping.ToPath;
        }
        var mapping = Children.First(x => x.Mapping != null).Mapping;
        var toPath = mapping.ToPath.Take(mapping.ToPath.IndexOf(mapping.ToPath.Single(x => x.Element == Model)) + 1).ToList();
        return toPath;
    }

    public void SetFromReplacement(ICanBeReferencedType type, string replacement)
    {
        if (_fromReplacements.ContainsKey(type))
        {
            return;
        }
        _fromReplacements.Add(type, replacement);
        foreach (var child in Children)
        {
            child.SetFromReplacement(type, replacement);
        }
    }

    public void SetToReplacement(ICanBeReferencedType type, string replacement)
    {
        if (_toReplacements.ContainsKey(type))
        {
            return;
        }
        _toReplacements.Add(type, replacement);
        foreach (var child in Children)
        {
            child.SetToReplacement(type, replacement);
        }
    }

    protected string GetFromPath(IDictionary<ICanBeReferencedType, string> replacements)
    {
        return GetPath(Mapping.FromPath, replacements);
    }

    protected string GetToPath(IDictionary<ICanBeReferencedType, string> replacements)
    {
        return GetPath(Mapping.ToPath, replacements);
    }

    protected string GetPath(IList<IElementMappingPathTarget> mappingPath, IDictionary<ICanBeReferencedType, string> replacements)
    {
        var result = "";
        foreach (var mappingPathTarget in mappingPath)
        {
            if (replacements.ContainsKey(mappingPathTarget.Element))
            {
                result = replacements[mappingPathTarget.Element] ?? ""; // if map to null, ignore
            }
            else
            {
                result += $"{(result.Length > 0 ? "." : "")}{mappingPathTarget.Element.Name.ToPascalCase()}";
                if (mappingPathTarget.Element.TypeReference?.IsNullable == true && mappingPath.Last() != mappingPathTarget)
                {
                    result += "?";
                }
            }
        }
        return result;
    }
}