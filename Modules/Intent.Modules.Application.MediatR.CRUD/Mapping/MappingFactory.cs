using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public abstract class MappingFactoryBase
{
    protected MappingFactoryBase(IDictionary<string, Func<ICanBeReferencedType, IElementToElementMappingConnection, IList<ICSharpMapping>, ICSharpMapping>> factoryRegistry)
    {

    }
    public ICSharpMapping Create(ICanBeReferencedType model, IList<IElementToElementMappingConnection> mappings, int level = 1)
    {
        var mapping = mappings.SingleOrDefault(x => x.ToPath.Last().Element == model);
        var children = mappings.Where(x => x.ToPath.Count > level)
            .GroupBy(x => x.ToPath.Skip(level).First(), x => x)
            .Select(x => Create(x.Key.Element, x.ToList(), level + 1))
            .ToList();
        return CreateMappingType(model, mapping, children.OrderBy(x => ((IElement)x.Model).Order).ToList());
    }

    public abstract ICSharpMapping CreateMappingType(ICanBeReferencedType model, IElementToElementMappingConnection mapping, List<ICSharpMapping> children);
}

public class CreateClassMappingFactory : MappingFactoryBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CreateClassMappingFactory(IDictionary<string, Func<ICanBeReferencedType, IElementToElementMappingConnection, IList<ICSharpMapping>, ICSharpMapping>> factoryRegistry, ICSharpFileBuilderTemplate template) : base(factoryRegistry)
    {
        _template = template;
    }

    public override ICSharpMapping CreateMappingType(ICanBeReferencedType model, IElementToElementMappingConnection mapping, List<ICSharpMapping> children)
    {
        if (model.TypeReference?.Element.SpecializationType == "Value Object")
        {
            return new ImplicitConstructorMapping(model, mapping, children, _template);
        }
        if (model.SpecializationType == "Class Constructor")
        {
            return new ImplicitConstructorMapping(((IElement)model).ParentElement, mapping, children, _template);
        }
        if (model.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(((IElement)model).ParentElement, mapping, children, _template);
        }
        return new ObjectInitializationMapping(model, mapping, children, _template);
    }
}