using System;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.RequestResponse.CQRS.CommandGuidReturn
{
    public class CommandGuidReturn : IRequest<Guid>, ICommand
    {
        public CommandGuidReturn(string input)
        {
            Input = input;
        }

        public string Input { get; set; }
    }
}