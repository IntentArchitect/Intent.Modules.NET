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
    public static string Generate<TEnumModel>(CSharpTemplateBase<TEnumModel> template)
        where TEnumModel : EnumModel
    {
        return new EnumGeneratorOfTEnumModel<TEnumModel>(template).Generate();
    }

    private class EnumGeneratorOfTEnumModel<TEnumModel>
        where TEnumModel : EnumModel
    {
        private readonly CSharpTemplateBase<TEnumModel> _template;

        public EnumGeneratorOfTEnumModel(CSharpTemplateBase<TEnumModel> template)
        {
            _template = template;
        }

        public string Generate()
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
}