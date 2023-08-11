using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class ImplicitConstructorMapping : ObjectInitializationMapping
{
    public ImplicitConstructorMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children) : base(model, mapping, children)
    {
    }

    public override CSharpStatement GetFromMappingStatement(IDictionary<ICanBeReferencedType, string> fromReplacements)
    {
        if (Mapping == null)
        {
            var init = new CSharpInvocationStatement($"new {Model.TypeReference.Element.Name.ToPascalCase()}").WithoutSemicolon();

            foreach (var child in Children)
            {
                init.AddArgument(GetFromPath(child.Mapping.FromPath, fromReplacements));
            }

            return init;
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}