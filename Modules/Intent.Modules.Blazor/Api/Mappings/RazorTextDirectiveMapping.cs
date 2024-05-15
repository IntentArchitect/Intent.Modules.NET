using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Api.Mappings;

public class RazorTextDirectiveMapping : CSharpMappingBase
{
    public RazorTextDirectiveMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
    }

    public override CSharpStatement GetSourceStatement()
    {
        var result = Mapping?.MappingExpression ?? throw new Exception($"Could not resolve source path. Mapping expected on '{Model.DisplayText ?? Model.Name}' [{Model.SpecializationType}]. Check that you have a MappingTypeResolver that addresses this scenario.");
        foreach (var map in GetParsedExpressionMap(Mapping?.MappingExpression, path => GetSourcePathText(Mapping.GetSource(path).Path)))
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
        ApplyNullConditionalOperators = false;
    }
}

public class RazorEventBindingMapping : CSharpMappingBase
{
    public RazorEventBindingMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
        ApplyNullConditionalOperators = false;
    }

    public override CSharpStatement GetSourceStatement()
    {
        var invocation = Mapping.SourceElement.IsEventEmitterModel()
            ? new CSharpInvocationStatement(base.GetSourceStatement(), "InvokeAsync")
                .WithoutSemicolon()
            : new CSharpInvocationStatement(base.GetSourceStatement())
                .WithoutSemicolon();
        foreach (var argumentMapping in Children)
        {
            var argument = argumentMapping.GetSourceStatement();
            invocation.AddArgument(argument);
        }
        return invocation;
    }
}