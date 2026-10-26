using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Global-namespace support for File Config "No Namespace".
    /// <para>
    /// Deliberate, documented exception to "never emit structural C# as raw strings": Intent.Modules.Common.CSharp
    /// (3.10.5) has no global-namespace support. <c>CSharpFile.ToString()</c> always wraps the types in
    /// <c>namespace {Namespace}</c> plus braces, which for an empty namespace is invalid C#. This post-processes
    /// that exact output; anything that does not have the exact expected shape is a module bug and throws.
    /// </para>
    /// </summary>
    public static class GlobalNamespaceOutput
    {
        private const string NamespaceKeyword = "namespace";
        private const string Indentation = "    ";

        /// <summary>
        /// Removes the empty <c>namespace</c> wrapper that <c>CSharpFile.ToString()</c> writes: the column-0
        /// <c>namespace</c> line, the <c>{</c> line after it and the final <c>}</c>, and de-indents the enclosed
        /// lines by one level (4 spaces). Everything before the wrapper (usings, assembly attributes, blank
        /// lines) and every line terminator is kept byte-for-byte.
        /// </summary>
        /// <exception cref="InvalidOperationException">The text is not in the shape <c>CSharpFile.ToString()</c> produces for an empty namespace.</exception>
        public static string UnwrapGlobalNamespace(string output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var lines = SplitLines(output);

            var namespaceLine = -1;
            for (var i = 0; i < lines.Count; i++)
            {
                if (IsNamespaceDeclaration(lines[i].Content))
                {
                    if (namespaceLine >= 0)
                    {
                        throw Unexpected("more than one column-0 namespace declaration was found");
                    }

                    namespaceLine = i;
                }
            }

            if (namespaceLine < 0)
            {
                throw Unexpected("no column-0 namespace declaration was found");
            }

            if (lines[namespaceLine].Content.Trim() != NamespaceKeyword)
            {
                throw Unexpected($"the namespace declaration '{lines[namespaceLine].Content.Trim()}' is not empty");
            }

            var openingBrace = namespaceLine + 1;
            var closingBrace = lines.Count - 1;
            if (openingBrace >= lines.Count || lines[openingBrace].Content != "{")
            {
                throw Unexpected("the namespace declaration is not followed by a line containing only '{'");
            }

            if (closingBrace <= openingBrace || lines[closingBrace].Content != "}")
            {
                throw Unexpected("the output does not end with a line containing only '}'");
            }

            var result = new StringBuilder(output.Length);
            for (var i = 0; i < namespaceLine; i++)
            {
                result.Append(lines[i].Content).Append(lines[i].Terminator);
            }

            for (var i = openingBrace + 1; i < closingBrace; i++)
            {
                var content = lines[i].Content;
                if (content.StartsWith(Indentation, StringComparison.Ordinal))
                {
                    content = content.Substring(Indentation.Length);
                }
                else if (content.Trim().Length != 0)
                {
                    throw Unexpected($"line {i + 1} inside the namespace is not indented by four spaces");
                }

                result.Append(content).Append(lines[i].Terminator);
            }

            return result.ToString();
        }

        private static bool IsNamespaceDeclaration(string content)
        {
            return content.StartsWith(NamespaceKeyword, StringComparison.Ordinal) &&
                   (content.Length == NamespaceKeyword.Length || char.IsWhiteSpace(content[NamespaceKeyword.Length]));
        }

        private static InvalidOperationException Unexpected(string detail)
        {
            return new InvalidOperationException(
                $"Cannot remove the namespace wrapper for a No Namespace file: {detail}. " +
                "The output no longer matches the shape CSharpFile.ToString() produces; update GlobalNamespaceOutput.");
        }

        private static List<(string Content, string Terminator)> SplitLines(string text)
        {
            var lines = new List<(string Content, string Terminator)>();
            var start = 0;
            var i = 0;
            while (i < text.Length)
            {
                var c = text[i];
                if (c == '\r' || c == '\n')
                {
                    var terminatorLength = c == '\r' && i + 1 < text.Length && text[i + 1] == '\n' ? 2 : 1;
                    lines.Add((text.Substring(start, i - start), text.Substring(i, terminatorLength)));
                    i += terminatorLength;
                    start = i;
                }
                else
                {
                    i++;
                }
            }

            if (start < text.Length)
            {
                lines.Add((text.Substring(start), string.Empty));
            }

            return lines;
        }
    }
}
