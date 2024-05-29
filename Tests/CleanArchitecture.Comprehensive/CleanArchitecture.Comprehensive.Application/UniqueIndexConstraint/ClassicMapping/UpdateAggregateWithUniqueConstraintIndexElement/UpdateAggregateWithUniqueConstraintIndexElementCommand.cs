using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.UpdateAggregateWithUniqueConstraintIndexElement
{
    public class UpdateAggregateWithUniqueConstraintIndexElementCommand : IRequest, ICommand
    {
        public UpdateAggregateWithUniqueConstraintIndexElementCommand(Guid id,
            string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB)
        {
            Id = id;
            SingleUniqueField = singleUniqueField;
            CompUniqueFieldA = compUniqueFieldA;
            CompUniqueFieldB = compUniqueFieldB;
        }

        public Guid Id { get; set; }
        public string SingleUniqueField { get; set; }
        public string CompUniqueFieldA { get; set; }
        public string CompUniqueFieldB { get; set; }
    }
}