using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Intent.Modules.AI.Blazor.Tasks.Config
{
    public sealed class DictionaryStringObjectJsonConverter
        : JsonConverter<Dictionary<string, object>>
    {
        public override Dictionary<string, object> Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return dict;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException();

                var propName = reader.GetString()!;
                reader.Read();
                dict[propName] = ReadValue(ref reader, options);
            }

            return dict;
        }

        private static object? ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    // recurse using this same converter
                    return JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options);

                case JsonTokenType.StartArray:
                    {
                        var list = new List<object?>();
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndArray) break;
                            list.Add(ReadValue(ref reader, options));
                        }
                        return list;
                    }

                case JsonTokenType.String:
                    if (reader.TryGetDateTime(out var dt)) return dt; // optional
                    return reader.GetString();

                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var l)) return l;
                    if (reader.TryGetDouble(out var d)) return d;
                    return reader.GetDecimal();

                case JsonTokenType.True: return true;
                case JsonTokenType.False: return false;
                case JsonTokenType.Null: return null;
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kv in value)
            {
                writer.WritePropertyName(kv.Key);
                WriteValue(writer, kv.Value, options);
            }
            writer.WriteEndObject();
        }

        private static void WriteValue(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
        {
            if (value is null) { writer.WriteNullValue(); return; }

            switch (value)
            {
                case string s: writer.WriteStringValue(s); return;
                case bool b: writer.WriteBooleanValue(b); return;
                case int i: writer.WriteNumberValue(i); return;
                case long l: writer.WriteNumberValue(l); return;
                case double d: writer.WriteNumberValue(d); return;
                case decimal m: writer.WriteNumberValue(m); return;
                case DateTime dt: writer.WriteStringValue(dt); return;

                case Dictionary<string, object> dict:
                    JsonSerializer.Serialize(writer, dict, options); return;

                case IEnumerable<object?> seq when value is not string:
                    writer.WriteStartArray();
                    foreach (var item in seq) WriteValue(writer, item, options);
                    writer.WriteEndArray();
                    return;

                default:
                    JsonSerializer.Serialize(writer, value, value.GetType(), options);
                    return;
            }
        }
    }
}
