using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>, ICommand
    {
        public CreateOrderCommand(string refNo)
        {
            RefNo = refNo;
        }

        public string RefNo { get; set; }
    }
}