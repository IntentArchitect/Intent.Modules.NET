using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.UpdateBasket
{
    public class UpdateBasketCommand : IRequest, ICommand
    {
        public UpdateBasketCommand(Guid id, string number)
        {
            Id = id;
            Number = number;
        }

        public Guid Id { get; set; }
        public string Number { get; set; }
    }
}