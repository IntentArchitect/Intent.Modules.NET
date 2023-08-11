using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class MappingFactory
{
    public MappingFactory(IDictionary<string, Func<ICanBeReferencedType, IElementToElementMappingConnection, IList<ICSharpMapping>, ICSharpMapping>> factoryRegistry)
    {

    }
    public ICSharpMapping Create(ICanBeReferencedType model, IList<IElementToElementMappingConnection> mappings, int level = 1)
    {
        var mapping = mappings.SingleOrDefault(x => x.ToPath.Last().Element == model);
        var children = mappings.Where(x => x.ToPath.Count > level)
            .GroupBy(x => x.ToPath.Skip(level).First(), x => x)
            .Select(x => Create(x.Key.Element, x.ToList(), level + 1))
            .ToList();

        if (model.TypeReference?.Element.SpecializationType == "Value Object")
        {
            return new ImplicitConstructorMapping(model, mapping, children);
        }
        return new ObjectInitializationMapping(model, mapping, children);
    }
}