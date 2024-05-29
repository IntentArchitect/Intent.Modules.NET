using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.CreateAggregateWithUniqueConstraintIndexElement
{
    public class CreateAggregateWithUniqueConstraintIndexElementCommand : IRequest<Guid>, ICommand
    {
        public CreateAggregateWithUniqueConstraintIndexElementCommand(string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB)
        {
            SingleUniqueField = singleUniqueField;
            CompUniqueFieldA = compUniqueFieldA;
            CompUniqueFieldB = compUniqueFieldB;
        }

        public string SingleUniqueField { get; set; }
        public string CompUniqueFieldA { get; set; }
        public string CompUniqueFieldB { get; set; }
    }
}