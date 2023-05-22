using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArchDapr.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Application.Orders.OrderConfirmed
{
    public class OrderConfirmed : IRequest, ICommand
    {
        public OrderConfirmed(Guid id, string refNo)
        {
            Id = id;
            RefNo = refNo;
        }
        public Guid Id { get; set; }

        public string RefNo { get; set; }

    }
}