using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SignalR.Application
{
    public class MessageToAllDto
    {
        public MessageToAllDto()
        {
            Message = null!;
        }

        public string Message { get; set; }

        public static MessageToAllDto Create(string message)
        {
            return new MessageToAllDto
            {
                Message = message
            };
        }
    }
}