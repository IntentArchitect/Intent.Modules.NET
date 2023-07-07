using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SignalR.Application
{
    public class SendToUsersDto
    {
        public SendToUsersDto()
        {
            Message = null!;
        }

        public string Message { get; set; }

        public static SendToUsersDto Create(string message)
        {
            return new SendToUsersDto
            {
                Message = message
            };
        }
    }
}