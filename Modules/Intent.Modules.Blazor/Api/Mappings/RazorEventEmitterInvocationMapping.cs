using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Api.Mappings;

public class RazorEventEmitterInvocationMapping : CSharpMappingBase
{
    public RazorEventEmitterInvocationMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
    }

    public override CSharpStatement GetSourceStatement(bool? targetIsNullable = default)
    {
        var invocation = new CSharpInvocationStatement(base.GetSourceStatement(targetIsNullable), "InvokeAsync");
        foreach (var argumentMapping in Children)
        {
            var argument = argumentMapping.GetSourceStatement();
            invocation.AddArgument(argument);
        }

        return new CSharpAwaitExpression(invocation);
    }
}