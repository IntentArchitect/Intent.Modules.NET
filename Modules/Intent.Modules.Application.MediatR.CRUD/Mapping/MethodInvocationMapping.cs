using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class MethodInvocationMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public MethodInvocationMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children)
    {
        _template = template;
    }

    public override CSharpStatement GetFromStatement()
    {
        var invocation = new CSharpInvocationStatement(GetToPath(_fromReplacements));

        foreach (var child in Children.OrderBy(x => ((IElement)x.Model).Order))
        {
            invocation.AddArgument(child.GetFromStatement());
        }

        return invocation;
    }

    public override CSharpStatement GetToStatement()
    {
        return GetPath(Children.First(x => x.Mapping != null).Mapping.ToPath.SkipLast(1).ToList(), _toReplacements);
    }

    public override IEnumerable<CSharpStatement> GetMappingStatement()
    {
        yield return GetFromStatement();
    }
}