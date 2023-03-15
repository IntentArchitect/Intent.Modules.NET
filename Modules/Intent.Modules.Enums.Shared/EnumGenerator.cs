using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Enums.Shared;

public class EnumGenerator
{
    private readonly CSharpTemplateBase<EnumModel> _template;

    private EnumGenerator(CSharpTemplateBase<EnumModel> template)
    {
        _template = template;
    }

    public static string Generate(CSharpTemplateBase<EnumModel> template) => new EnumGenerator(template).Generate();

    private string Generate()
    {
        return $@"
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace {_template.Namespace}
{{
    {GetEnumAttributes()}public enum {_template.ClassName}
    {{
        {GetEnumLiterals(_template.Model.Literals)}
    }}
}}";
    }

    private string GetEnumAttributes()
    {
        return _template.Model.HasStereotype("Flags") ? $@"[{_template.UseType("System.Flags")}]
        " : string.Empty;
    }

    private static string GetEnumLiterals(IEnumerable<EnumLiteralModel> literals)
    {
        return string.Join(@",
            ", literals.Select(GetEnumLiteral));
    }

    private static string GetEnumLiteral(EnumLiteralModel literal)
    {
        return $"{literal.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper)}{(string.IsNullOrWhiteSpace(literal.Value) ? "" : $" = {literal.Value}")}";
    }
}