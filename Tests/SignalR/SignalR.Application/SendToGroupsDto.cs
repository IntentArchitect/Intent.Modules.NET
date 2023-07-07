using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SignalR.Application
{
    public class SendToGroupsDto
    {
        public SendToGroupsDto()
        {
            Message = null!;
        }

        public string Message { get; set; }

        public static SendToGroupsDto Create(string message)
        {
            return new SendToGroupsDto
            {
                Message = message
            };
        }
    }
}