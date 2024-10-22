using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
    {
        public UpdateOrderCommand(string refNo, Guid countryId, Guid id)
        {
            RefNo = refNo;
            CountryId = countryId;
            Id = id;
        }

        public string RefNo { get; set; }
        public Guid CountryId { get; set; }
        public Guid Id { get; set; }
    }
}