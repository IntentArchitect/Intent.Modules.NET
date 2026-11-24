using System;
using System.Globalization;
using System.Text.Json;
using Intent.Modules.Common.FileBuilders.DataFileBuilder;
using Intent.Modules.Common.FileBuilders.DataFileBuilder.Writers;

namespace Intent.Modules.ApiGateway.Ocelot.Templates.OcelotJson
{
    /// <summary>
    /// Writes <c>ocelot.json</c> by merging the generated content into the file on disk (see <see cref="OcelotJsonMerger"/>),
    /// so the template keeps hand edits itself rather than through the Intent.Code.Weaving.Json module.
    /// </summary>
    /// <remarks>
    /// The Data File Builder hands the writer the root object of the generated file; this writer converts that whole tree and
    /// writes the merged result, so it is only ever visited at the root.
    /// </remarks>
    internal class OcelotJsonWriter : DataFileWriter
    {
        private readonly Func<JsoncDocument?> _existing;

        public OcelotJsonWriter(Func<JsoncDocument?> existing) : base("  ", "//")
        {
            _existing = existing;
        }

        public override void Visit(IDataFileObjectValue @object)
        {
            if (@object.Parent is not null)
            {
                throw new InvalidOperationException($"{nameof(OcelotJsonWriter)} writes whole files only.");
            }

            var merged = OcelotJsonMerger.Merge(_existing(), (JsoncObject)Convert(@object));
            StringBuilder.Append(JsoncWriter.Write(merged));
        }

        public override void Visit(IDataFileArrayValue array)
        {
            throw new InvalidOperationException($"{nameof(OcelotJsonWriter)} writes whole files only.");
        }

        public override void Visit(IDataFileScalarValue scalar)
        {
            throw new InvalidOperationException($"{nameof(OcelotJsonWriter)} writes whole files only.");
        }

        private static JsoncValue Convert(IDataFileValue value)
        {
            switch (value)
            {
                case IDataFileObjectValue @object:
                    var result = new JsoncObject();
                    foreach (var (name, member) in @object)
                    {
                        result.Members.Add(WithComment(new JsoncMember(name, Convert(member)), member));
                    }

                    return result;
                case IDataFileArrayValue array:
                    var items = new JsoncArray();
                    for (var index = 0; index < array.Count; index++)
                    {
                        items.Items.Add(WithComment(new JsoncMember(null, Convert(array[index])), array[index]));
                    }

                    return items;
                case IDataFileScalarValue scalar:
                    return scalar.Value switch
                    {
                        string text => JsoncScalar.String(text),
                        bool flag => JsoncScalar.Boolean(flag),
                        IFormattable number => new JsoncScalar(number.ToString(null, CultureInfo.InvariantCulture), JsonValueKind.Number, null),
                        _ => throw new InvalidOperationException($"Unsupported value type {scalar.Value?.GetType()} in ocelot.json.")
                    };
                default:
                    throw new InvalidOperationException($"Unknown value type {value.GetType()} in ocelot.json.");
            }
        }

        private static JsoncMember WithComment(JsoncMember member, IDataFileValue value)
        {
            if (!string.IsNullOrWhiteSpace(value.Comment))
            {
                member.Comments.Add($"// {value.Comment}");
            }

            return member;
        }
    }
}
