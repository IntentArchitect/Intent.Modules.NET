using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class UpdateClassMappingFactory : MappingFactoryBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public UpdateClassMappingFactory(IDictionary<string, Func<ICanBeReferencedType, IElementToElementMappingConnection, IList<ICSharpMapping>, ICSharpMapping>> factoryRegistry, ICSharpFileBuilderTemplate template) : base(factoryRegistry)
    {
        _template = template;
    }

    public override ICSharpMapping CreateMappingType(ICanBeReferencedType model, IElementToElementMappingConnection mapping, List<ICSharpMapping> children)
    {
        if (model.TypeReference?.Element.SpecializationType == "Value Object")
        {
            return new ImplicitConstructorMapping(model, mapping, children);
        }
        return new ObjectUpdateMapping(model, mapping, children, _template);
    }
}

public class ObjectUpdateMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public ObjectUpdateMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children)
    {
        _template = template;
    }

    public override IEnumerable<CSharpStatement> GetMappingStatement(IDictionary<ICanBeReferencedType, string> fromReplacements, IDictionary<ICanBeReferencedType, string> toReplacements)
    {
        if (Mapping == null) // is traversal
        {
            //var init = (Model.TypeReference != null)
            //    ? new CSharpObjectInitializerBlock($"new {Model.TypeReference.Element.Name.ToPascalCase()}")
            //    : new CSharpObjectInitializerBlock($"new {Model.Name.ToPascalCase()}").WithSemicolon();

            //init.AddStatements(Children.SelectMany(x => x.GetMappingStatement(fromReplacements)));

            //yield return init;
            if (Model.TypeReference == null)
            {
                foreach (var statement in Children.SelectMany(x => x.GetMappingStatement(fromReplacements, toReplacements)))
                {
                    yield return statement;
                }
            }
            else
            {
            }
        }
        else
        {
            if (Children.Count == 0)
            {
                yield return new CSharpAssignmentStatement($"{GetToPath(fromReplacements)}", $"{GetFromPath(fromReplacements)}").WithSemicolon();
            }
        }
    }
}