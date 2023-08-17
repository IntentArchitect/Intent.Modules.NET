using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class CSharpClassMappingManager : MappingManagerBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CSharpClassMappingManager(ICSharpFileBuilderTemplate template) : base()
    {
        _template = template;
    }

    protected override ICSharpMapping GetCreateMappingType(ICanBeReferencedType model, IElementToElementMappingConnection mapping, List<ICSharpMapping> children)
    {
        if (model.TypeReference?.Element.SpecializationType == "Value Object")
        {
            return new ImplicitConstructorMapping(model, mapping, children, _template);
        }
        if (model.SpecializationType == "Domain Event")
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

    protected override ICSharpMapping GetUpdateMappingType(ICanBeReferencedType model, IElementToElementMappingConnection mapping, List<ICSharpMapping> children)
    {
        if (model.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(model, mapping, children, _template);
        }

        if (model.TypeReference?.Element.SpecializationType == "Value Object")
        {
            return new ImplicitConstructorMapping(model, mapping, children, _template);
        }
        return new ObjectUpdateMapping(model, mapping, children.Where(x => !StereotypeExtensions.HasStereotype(x.Model, "Primary Key")).ToList(), _template);
    }
}