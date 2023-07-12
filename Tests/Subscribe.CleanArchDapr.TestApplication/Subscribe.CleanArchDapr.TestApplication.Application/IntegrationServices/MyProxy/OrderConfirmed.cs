using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices.MyProxy
{
    public class OrderConfirmed
    {
        public static OrderConfirmed Create(Guid id, string refNo)
        {
            return new OrderConfirmed
            {
                Id = id,
                RefNo = refNo
            };
        }

        public Guid Id { get; set; }

        public string RefNo { get; set; }
    }
}