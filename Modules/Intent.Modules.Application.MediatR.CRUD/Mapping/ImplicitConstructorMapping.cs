using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class ImplicitConstructorMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public ImplicitConstructorMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
        _template = template;
    }

    public override CSharpStatement GetFromStatement()
    {
        var init = (Model.TypeReference != null)
            ? new CSharpInvocationStatement($"new {_template.GetTypeName((IElement)Model.TypeReference.Element)}").WithoutSemicolon()
            : new CSharpInvocationStatement($"new {_template.GetTypeName(((IElement)Model))}").WithoutSemicolon();

        foreach (var child in Children.OrderBy(x => ((IElement)x.Model).Order))
        {
            init.AddArgument(child.GetFromStatement());
        }

        return init;
    }

    public override CSharpStatement GetToStatement()
    {
        return GetPath(Children.First(x => x.Mapping != null).Mapping.ToPath.SkipLast(1).ToList(), _toReplacements);
    }

    public override IEnumerable<CSharpStatement> GetMappingStatement()
    {
        yield return new CSharpAssignmentStatement(GetToStatement(), GetFromStatement());
    }
}