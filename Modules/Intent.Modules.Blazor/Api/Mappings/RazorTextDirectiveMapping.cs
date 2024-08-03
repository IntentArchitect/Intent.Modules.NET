using System;
using System.Linq;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using NuGet.Versioning;

namespace Intent.Modules.Blazor.Api.Mappings;

public class RazorTextDirectiveMapping : CSharpMappingBase
{
    public RazorTextDirectiveMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
    }

    public override CSharpStatement GetSourceStatement(bool? targetIsNullable = default)
    {
        var result = Mapping?.MappingExpression ?? throw new Exception($"Could not resolve source path. Mapping expected on '{Model.DisplayText ?? Model.Name}' [{Model.SpecializationType}]. Check that you have a MappingTypeResolver that addresses this scenario.");
        foreach (var map in GetParsedExpressionMap(Mapping?.MappingExpression, path => GetSourcePathText(Mapping.GetSource(path).Path, true)))
        {
            result = result.Replace(map.Key, map.Value.Contains(' ') ? $"@({map.Value})" : $"@{map.Value}");
        }
        return result;
    }
}

public class RazorPropertyBindingMapping : CSharpMappingBase
{
    public RazorPropertyBindingMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
    }
}

public class RazorEventBindingMapping : CSharpMappingBase
{
    public RazorEventBindingMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
    }

    public override CSharpStatement GetSourceStatement(bool? targetIsNullable = default)
    {
        var invocation = Mapping.SourceElement.IsEventEmitterModel()
            ? new CSharpInvocationStatement(base.GetSourceStatement(targetIsNullable), "InvokeAsync")
                .WithoutSemicolon()
            : new CSharpInvocationStatement(base.GetSourceStatement(targetIsNullable))
                .WithoutSemicolon();
        foreach (var argumentMapping in Children)
        {
            var argument = argumentMapping.GetSourceStatement();
            invocation.AddArgument(argument);
        }

        //if (Mapping.SourceElement.IsEventEmitterModel() && (Template is IRazorComponentTemplate razorComponentTemplate | Template is ICSharpFileBuilderTemplate cSharpFileBuilderTemplate))
        //{
        //    (cSharpFileBuilderTemplate?.CSharpFile.Classes.First() ?? razorComponentTemplate.GetCode()).AddMethod("void", $"Invoke{Mapping.SourceElement.Name.ToCSharpIdentifier()}", method =>
        //    {
        //        method.Async();
        //        method.AddStatement(new CSharpAwaitExpression(invocation).WithSemicolon());
        //    });
        //    return $"Invoke{Mapping.SourceElement.Name.ToCSharpIdentifier()}()";
        //}

        return invocation;
    }
}