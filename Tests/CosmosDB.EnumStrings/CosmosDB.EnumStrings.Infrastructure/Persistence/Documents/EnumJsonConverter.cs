using System;
using Intent.RoslynWeaver.Attributes;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.EnumJsonConverter", Version = "1.0")]

namespace CosmosDB.EnumStrings.Infrastructure.Persistence.Documents
{
    public class EnumJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value is null || reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Enum.Parse(objectType.GenericTypeArguments[0], reader.Value.ToString()!);
            }

            return Enum.Parse(objectType, reader.Value.ToString()!);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }
    }
}