using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.CreateBasket
{
    public class CreateBasketCommand : IRequest<Guid>, ICommand
    {
        public CreateBasketCommand(string number)
        {
            Number = number;
        }

        public string Number { get; set; }
    }
}