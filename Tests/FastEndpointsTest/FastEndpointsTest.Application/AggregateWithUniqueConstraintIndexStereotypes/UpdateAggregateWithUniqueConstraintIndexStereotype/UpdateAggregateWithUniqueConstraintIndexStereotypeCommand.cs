using System;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes.UpdateAggregateWithUniqueConstraintIndexStereotype
{
    public class UpdateAggregateWithUniqueConstraintIndexStereotypeCommand : IRequest, ICommand
    {
        public UpdateAggregateWithUniqueConstraintIndexStereotypeCommand(string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB,
            Guid id)
        {
            SingleUniqueField = singleUniqueField;
            CompUniqueFieldA = compUniqueFieldA;
            CompUniqueFieldB = compUniqueFieldB;
            Id = id;
        }

        public string SingleUniqueField { get; set; }
        public string CompUniqueFieldA { get; set; }
        public string CompUniqueFieldB { get; set; }
        public Guid Id { get; set; }
    }
}