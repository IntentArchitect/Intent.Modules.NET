using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Application;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.BaseMessageConverter", Version = "1.0")]

namespace Solace.Tests.Infrastructure.Eventing
{
    public class BaseMessageConverter : JsonConverter<BaseMessage>
    {
        private readonly Dictionary<string, Type> _typeLookup;

        public BaseMessageConverter(MessageRegistry messageRegistry)
        {
            _typeLookup = new Dictionary<string, Type>();

            foreach (var message in messageRegistry.MessageTypes)
            {
                _typeLookup.Add(message.Value, message.Key);
            }
        }

        public override BaseMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;
                var messageTypeString = root.GetProperty("MessageType").GetString();
                if (messageTypeString is null)
                    throw new Exception($"No `MessageType` property found in message {root.GetRawText()}");

                if (!_typeLookup.TryGetValue(messageTypeString, out var messageType))
                {
                    throw new InvalidOperationException("Unknown message type");
                }
                var result = JsonSerializer.Deserialize(root.GetRawText(), messageType, options) as BaseMessage;
                if (result is null)
                    throw new Exception($"Unable to deserialize {root.GetRawText()} as {messageType.Name}");
                return result;
            }
        }

        public override void Write(Utf8JsonWriter writer, BaseMessage value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}