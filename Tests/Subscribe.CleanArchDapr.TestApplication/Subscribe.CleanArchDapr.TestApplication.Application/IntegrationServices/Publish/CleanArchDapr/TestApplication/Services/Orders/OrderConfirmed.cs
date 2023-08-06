using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices.Publish.CleanArchDapr.TestApplication.Services.Orders
{
    public class OrderConfirmed
    {
        public Guid Id { get; set; }
        public string RefNo { get; set; }

        public static OrderConfirmed Create(Guid id, string refNo)
        {
            return new OrderConfirmed
            {
                Id = id,
                RefNo = refNo
            };
        }
    }
}