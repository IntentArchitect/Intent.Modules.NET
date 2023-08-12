using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class ImplicitConstructorMapping : CSharpMappingBase
{
    public ImplicitConstructorMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children) : base(model, mapping, children)
    {
    }

    public override IEnumerable<CSharpStatement> GetMappingStatement(IDictionary<ICanBeReferencedType, string> fromReplacements, IDictionary<ICanBeReferencedType, string> toReplacements)
    {
        // implicit / traversal into value object:

        var init = new CSharpInvocationStatement($"new {Model.TypeReference.Element.Name.ToPascalCase()}").WithoutSemicolon();

        foreach (var child in Children)
        {
            init.AddArgument(GetPath(child.Mapping.FromPath, fromReplacements));
        }

        yield return new CSharpAssignmentStatement(GetPath(Children.First(x => x.Mapping != null).Mapping.ToPath.SkipLast(1).ToList(), toReplacements), init);
    }
}