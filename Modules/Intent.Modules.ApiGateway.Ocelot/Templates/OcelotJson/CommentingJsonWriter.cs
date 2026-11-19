using System;
using Intent.Modules.Common.FileBuilders.DataFileBuilder;
using Intent.Modules.Common.FileBuilders.DataFileBuilder.Writers;

namespace Intent.Modules.ApiGateway.Ocelot.Templates.OcelotJson
{
    /// <summary>
    /// <see cref="DataFileJsonWriter"/> that also writes the <see cref="IDataFileValue.Comment"/> of each field of the
    /// root object, as a <c>// comment</c> line above it.
    /// </summary>
    /// <remarks>
    /// The Data File Builder's own JSON writer ignores <see cref="IDataFileValue.Comment"/> (Intent.Modules.Common 3.11.9).
    /// The template needs it to write the JSON weaver's <c>// IntentMatchBy(...)</c> instruction above <c>"Routes"</c>.
    /// Retire this class once <see cref="DataFileJsonWriter"/> writes comments itself.
    /// </remarks>
    internal class CommentingJsonWriter : DataFileJsonWriter
    {
        private const string Tab = "  ";

        public CommentingJsonWriter() : base(Tab)
        {
        }

        public override void Visit(IDataFileObjectValue @object)
        {
            var start = StringBuilder.Length;
            base.Visit(@object);

            if (@object.Parent != null)
            {
                return;
            }

            // The root object's fields are written one per line at a single indentation
            foreach (var (name, value) in @object)
            {
                if (string.IsNullOrWhiteSpace(value.Comment))
                {
                    continue;
                }

                var fieldLine = $"{Environment.NewLine}{Tab}\"{name}\": ";
                var index = StringBuilder.ToString(start, StringBuilder.Length - start).IndexOf(fieldLine, StringComparison.Ordinal);
                if (index < 0)
                {
                    continue;
                }

                StringBuilder.Insert(start + index + Environment.NewLine.Length, $"{Tab}// {value.Comment}{Environment.NewLine}");
            }
        }
    }
}
