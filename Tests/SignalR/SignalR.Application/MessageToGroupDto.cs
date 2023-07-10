using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SignalR.Application
{
    public class MessageToGroupDto
    {
        public MessageToGroupDto()
        {
            Message = null!;
        }

        public string Message { get; set; }

        public static MessageToGroupDto Create(string message)
        {
            return new MessageToGroupDto
            {
                Message = message
            };
        }
    }
}