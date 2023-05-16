using Intent.Metadata.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Intent.Modules.Application.MediatR.Templates
{
    internal class TemplateHelper
    {
        internal static string GetXmlDocComments(string plainTextComment, string indentation)
        {
            if (HasXmlDocComments(plainTextComment, indentation, out var comments))
                return comments;
            return string.Empty;
        }

        private static bool HasXmlDocComments(string plainTextComment, string indentation, out string comments)
        {
            comments = null;
            var comment = plainTextComment?.Trim();

            if (string.IsNullOrWhiteSpace(comment))
            {
                return false;
            }

            //if (Containers summary dont add summary)
            comments = string.Concat(Enumerable.Empty<string>()
                .Append("<summary>")
                .Concat(comment.Replace("\r\n", "\n").Split('\n'))
                .Append("</summary>")
                .Select(line => $"/// {line}{Environment.NewLine}{indentation}")
            );
            return true;
        }
    }
}
