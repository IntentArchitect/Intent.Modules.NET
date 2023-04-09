using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
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
    {GetComments(_template.Model.InternalElement, "    ")}{GetEnumAttributes()}public enum {_template.ClassName}
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
        return $"{GetComments(literal.InternalElement, "        ")}{literal.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper)}{(string.IsNullOrWhiteSpace(literal.Value) ? "" : $" = {literal.Value}")}";
    }

    private static string GetComments(IElement element, string indentation)
    {
        var comment = element?.Comment?.Trim();

        if (string.IsNullOrWhiteSpace(comment))
        {
            return string.Empty;
        }

        return string.Concat(Enumerable.Empty<string>()
            .Append("<summary>")
            .Concat(comment.Replace("\r\n", "\n").Split('\n'))
            .Append("</summary>")
            .Select(line => $"/// {line}{Environment.NewLine}{indentation}")
        );
    }
}