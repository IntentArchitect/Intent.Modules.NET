using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SignalR.Application
{
    public class MessageToUserDto
    {
        public MessageToUserDto()
        {
            Message = null!;
        }

        public string Message { get; set; }

        public static MessageToUserDto Create(string message)
        {
            return new MessageToUserDto
            {
                Message = message
            };
        }
    }
}