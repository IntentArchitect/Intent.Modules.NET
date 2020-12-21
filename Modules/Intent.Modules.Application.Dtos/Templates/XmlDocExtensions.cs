using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.Application.Dtos.Templates
{
    public static class XmlDocExtensions
    {
        public static IEnumerable<string> GetXmlDocLines(this IHasStereotypes hasStereotypes)
        {
            var text = hasStereotypes.GetStereotypeProperty<string>("XmlDoc", "Content");

            return string.IsNullOrWhiteSpace(text)
                ? new string[0]
                : text
                    .Replace("\r\n", "\r")
                    .Replace("\n", "\r")
                    .Split('\r');
        }
    }
}