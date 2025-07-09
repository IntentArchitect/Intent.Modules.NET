using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Publish.CleanArchDapr.TestApplication.Application.Orders.OrderConfirmed
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderConfirmedHandler : IRequestHandler<OrderConfirmed>
    {
        [IntentManaged(Mode.Merge)]
        public OrderConfirmedHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(OrderConfirmed request, CancellationToken cancellationToken)
        {

        }
    }
}