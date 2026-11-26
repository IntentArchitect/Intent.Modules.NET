using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Intent.Modules.ApiGateway.Ocelot.Templates.OcelotJson
{
    /// <summary>
    /// A JSON document with its comments (JSONC), as .NET configuration and Ocelot read it. Just enough of one for
    /// <see cref="OcelotJsonMerger"/> to merge the generated <c>ocelot.json</c> into the one on disk without losing what was
    /// written there by hand: values keep their original text, and comments stay with the member they sit above (or beside).
    /// </summary>
    internal abstract class JsoncValue
    {
    }

    internal sealed class JsoncScalar : JsoncValue
    {
        public JsoncScalar(string rawText, JsonValueKind kind, string? stringValue)
        {
            RawText = rawText;
            Kind = kind;
            StringValue = stringValue;
        }

        /// <summary>The value exactly as written, including the quotes of a string.</summary>
        public string RawText { get; }

        public JsonValueKind Kind { get; }

        /// <summary>The unescaped value of a string, otherwise <see langword="null"/>.</summary>
        public string? StringValue { get; }

        public static JsoncScalar String(string value) => new(JsonSerializer.Serialize(value), JsonValueKind.String, value);

        public static JsoncScalar Boolean(bool value) => new(value ? "true" : "false", value ? JsonValueKind.True : JsonValueKind.False, null);
    }

    internal sealed class JsoncObject : JsoncValue
    {
        public List<JsoncMember> Members { get; } = new();

        /// <summary>Comments after the last member, before the closing brace.</summary>
        public List<string> TrailingComments { get; } = new();

        public JsoncMember? Find(string name) => Members.FirstOrDefault(member => member.Name == name);

        public JsoncValue? Get(string name) => Find(name)?.Value;

        public string? GetString(string name) => (Get(name) as JsoncScalar)?.StringValue;
    }

    internal sealed class JsoncArray : JsoncValue
    {
        public List<JsoncMember> Items { get; } = new();

        /// <summary>Comments after the last item, before the closing bracket.</summary>
        public List<string> TrailingComments { get; } = new();
    }

    /// <summary>A property of an object (with a <see cref="Name"/>) or an item of an array (without one).</summary>
    internal sealed class JsoncMember
    {
        public JsoncMember(string? name, JsoncValue value)
        {
            Name = name;
            Value = value;
        }

        public string? Name { get; }

        public JsoncValue Value { get; set; }

        /// <summary>Comments on the lines above the member, each as written (<c>// ...</c> or <c>/* ... */</c>).</summary>
        public List<string> Comments { get; } = new();

        /// <summary>A comment on the same line, after the member.</summary>
        public string? LineComment { get; set; }

        /// <summary>
        /// <c>// IntentIgnore</c> above a member, the same marker Intent Architect's JSON code management uses: the member is
        /// left exactly as it is on disk, value and all.
        /// </summary>
        public bool IsIgnored => Comments.Any(comment => CommentText(comment) == "IntentIgnore");

        private static string CommentText(string comment)
        {
            return comment.StartsWith("/*", StringComparison.Ordinal)
                ? comment.Substring(2, Math.Max(0, comment.Length - 4)).Trim()
                : comment.Substring(2).Trim();
        }
    }

    internal sealed class JsoncDocument
    {
        public JsoncDocument(JsoncValue root)
        {
            Root = root;
        }

        public List<string> LeadingComments { get; } = new();

        public JsoncValue Root { get; }

        public List<string> TrailingComments { get; } = new();
    }

    internal static class JsoncParser
    {
        /// <summary>
        /// Parses JSON with comments and trailing commas. Returns <see langword="null"/> for a file with no value in it.
        /// </summary>
        /// <exception cref="JsonException">The content is not valid JSON.</exception>
        public static JsoncDocument? Parse(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content.TrimStart('﻿'));
            var reader = new Utf8JsonReader(bytes, new JsonReaderOptions
            {
                CommentHandling = JsonCommentHandling.Allow,
                AllowTrailingCommas = true
            });
            var parser = new Parser(bytes);

            var leading = new List<string>();
            JsoncValue? root = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.Comment)
                {
                    leading.Add(parser.RawComment(ref reader));
                    continue;
                }

                root = parser.ReadValue(ref reader);
                break;
            }

            if (root is null)
            {
                return null;
            }

            var document = new JsoncDocument(root);
            document.LeadingComments.AddRange(leading);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.Comment)
                {
                    document.TrailingComments.Add(parser.RawComment(ref reader));
                }
            }

            return document;
        }

        private sealed class Parser
        {
            private readonly byte[] _bytes;
            private long _lastValueEnd;

            public Parser(byte[] bytes)
            {
                _bytes = bytes;
            }

            public JsoncValue ReadValue(ref Utf8JsonReader reader)
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        return ReadObject(ref reader);
                    case JsonTokenType.StartArray:
                        return ReadArray(ref reader);
                    case JsonTokenType.String:
                        var stringValue = reader.GetString();
                        return Scalar(ref reader, JsonValueKind.String, stringValue);
                    case JsonTokenType.Number:
                        return Scalar(ref reader, JsonValueKind.Number, null);
                    case JsonTokenType.True:
                        return Scalar(ref reader, JsonValueKind.True, null);
                    case JsonTokenType.False:
                        return Scalar(ref reader, JsonValueKind.False, null);
                    case JsonTokenType.Null:
                        return Scalar(ref reader, JsonValueKind.Null, null);
                    default:
                        throw new JsonException($"Unexpected {reader.TokenType} at byte {reader.TokenStartIndex}.");
                }
            }

            public string RawComment(ref Utf8JsonReader reader)
            {
                var start = (int)reader.TokenStartIndex;
                var length = (int)(reader.BytesConsumed - reader.TokenStartIndex);
                return Encoding.UTF8.GetString(_bytes, start, length).TrimEnd('\r', '\n');
            }

            private JsoncScalar Scalar(ref Utf8JsonReader reader, JsonValueKind kind, string? stringValue)
            {
                var start = (int)reader.TokenStartIndex;
                var length = (int)(reader.BytesConsumed - reader.TokenStartIndex);
                _lastValueEnd = reader.BytesConsumed;
                return new JsoncScalar(Encoding.UTF8.GetString(_bytes, start, length), kind, stringValue);
            }

            private JsoncObject ReadObject(ref Utf8JsonReader reader)
            {
                var result = new JsoncObject();
                _lastValueEnd = reader.BytesConsumed;
                var pending = new List<string>();
                JsoncMember? previous = null;

                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.EndObject:
                            result.TrailingComments.AddRange(pending);
                            _lastValueEnd = reader.BytesConsumed;
                            return result;
                        case JsonTokenType.Comment:
                            AddComment(ref reader, previous, pending);
                            break;
                        case JsonTokenType.PropertyName:
                            var name = reader.GetString()!;
                            JsoncValue? value = null;
                            while (value is null && reader.Read())
                            {
                                if (reader.TokenType == JsonTokenType.Comment)
                                {
                                    // A comment between a name and its value: keep it above the member.
                                    pending.Add(RawComment(ref reader));
                                    continue;
                                }

                                value = ReadValue(ref reader);
                            }

                            var member = new JsoncMember(name, value ?? throw new JsonException($"Property '{name}' has no value."));
                            member.Comments.AddRange(pending);
                            pending.Clear();
                            result.Members.Add(member);
                            previous = member;
                            break;
                        default:
                            throw new JsonException($"Unexpected {reader.TokenType} at byte {reader.TokenStartIndex}.");
                    }
                }

                throw new JsonException("The object is not closed.");
            }

            private JsoncArray ReadArray(ref Utf8JsonReader reader)
            {
                var result = new JsoncArray();
                _lastValueEnd = reader.BytesConsumed;
                var pending = new List<string>();
                JsoncMember? previous = null;

                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.EndArray:
                            result.TrailingComments.AddRange(pending);
                            _lastValueEnd = reader.BytesConsumed;
                            return result;
                        case JsonTokenType.Comment:
                            AddComment(ref reader, previous, pending);
                            break;
                        default:
                            var item = new JsoncMember(null, ReadValue(ref reader));
                            item.Comments.AddRange(pending);
                            pending.Clear();
                            result.Items.Add(item);
                            previous = item;
                            break;
                    }
                }

                throw new JsonException("The array is not closed.");
            }

            /// <summary>
            /// A comment on the same line as the end of the previous member stays beside it; any other comment goes above the
            /// next member.
            /// </summary>
            private void AddComment(ref Utf8JsonReader reader, JsoncMember? previous, List<string> pending)
            {
                var comment = RawComment(ref reader);
                var sameLine = previous is not null &&
                    previous.LineComment is null &&
                    pending.Count == 0 &&
                    !comment.Contains('\n') &&
                    Array.IndexOf(_bytes, (byte)'\n', (int)_lastValueEnd, (int)(reader.TokenStartIndex - _lastValueEnd)) < 0;
                if (sameLine)
                {
                    previous!.LineComment = comment;
                }
                else
                {
                    pending.Add(comment);
                }
            }
        }
    }

    internal static class JsoncWriter
    {
        private const string Tab = "  ";

        public static string Write(JsoncDocument document)
        {
            var builder = new StringBuilder();
            foreach (var comment in document.LeadingComments)
            {
                builder.AppendLine(comment);
            }

            WriteValue(builder, document.Root, string.Empty);
            builder.AppendLine();
            foreach (var comment in document.TrailingComments)
            {
                builder.AppendLine(comment);
            }

            return builder.ToString();
        }

        private static void WriteValue(StringBuilder builder, JsoncValue value, string indentation)
        {
            switch (value)
            {
                case JsoncScalar scalar:
                    builder.Append(scalar.RawText);
                    break;
                case JsoncObject @object:
                    WriteMembers(builder, @object.Members, @object.TrailingComments, indentation, '{', '}');
                    break;
                case JsoncArray array:
                    WriteMembers(builder, array.Items, array.TrailingComments, indentation, '[', ']');
                    break;
                default:
                    throw new InvalidOperationException($"Unknown JSON value type {value.GetType()}.");
            }
        }

        private static void WriteMembers(StringBuilder builder, IReadOnlyList<JsoncMember> members, IReadOnlyList<string> trailingComments, string indentation, char open, char close)
        {
            if (members.Count == 0 && trailingComments.Count == 0)
            {
                builder.Append(open).Append(close);
                return;
            }

            var inner = indentation + Tab;
            builder.Append(open).AppendLine();
            for (var index = 0; index < members.Count; index++)
            {
                var member = members[index];
                foreach (var comment in member.Comments)
                {
                    builder.Append(inner).AppendLine(comment);
                }

                builder.Append(inner);
                if (member.Name is not null)
                {
                    builder.Append(JsonSerializer.Serialize(member.Name)).Append(": ");
                }

                WriteValue(builder, member.Value, inner);
                if (index < members.Count - 1)
                {
                    builder.Append(',');
                }

                if (member.LineComment is not null)
                {
                    builder.Append(' ').Append(member.LineComment);
                }

                builder.AppendLine();
            }

            foreach (var comment in trailingComments)
            {
                builder.Append(inner).AppendLine(comment);
            }

            builder.Append(indentation).Append(close);
        }
    }
}
