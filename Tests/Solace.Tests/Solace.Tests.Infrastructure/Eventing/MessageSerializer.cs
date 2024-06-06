using System;
using System.Text;
using System.Text.Json;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Application;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.MessageSerializer", Version = "1.0")]

namespace Solace.Tests.Infrastructure.Eventing
{
    public class MessageSerializer
    {
        private readonly JsonSerializerOptions _serializationOptions;

        public MessageSerializer(MessageRegistry messageRegistry)
        {
            _serializationOptions = new JsonSerializerOptions();
            _serializationOptions.Converters.Add(new BaseMessageConverter(messageRegistry));
        }

        public byte[] SerializeMessage(object message)
        {
            return Encoding.ASCII.GetBytes(JsonSerializer.Serialize(message, _serializationOptions));
        }

        public BaseMessage DeserializeMessage(byte[] binary)
        {
            string jsonString = Encoding.ASCII.GetString(binary);
            var result = JsonSerializer.Deserialize<BaseMessage>(jsonString, _serializationOptions);
            if (result == null)
                throw new Exception($"Unable to deserialize message as `BaseMessage`. ({jsonString})");
            return result;
        }
    }
}