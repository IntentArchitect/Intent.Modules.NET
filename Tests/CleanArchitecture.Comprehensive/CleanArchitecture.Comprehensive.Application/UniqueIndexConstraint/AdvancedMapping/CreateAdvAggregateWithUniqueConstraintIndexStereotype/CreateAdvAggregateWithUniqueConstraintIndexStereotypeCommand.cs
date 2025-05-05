using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.AdvancedMapping.CreateAdvAggregateWithUniqueConstraintIndexStereotype
{
    public class CreateAdvAggregateWithUniqueConstraintIndexStereotypeCommand : IRequest<Guid>, ICommand
    {
        public CreateAdvAggregateWithUniqueConstraintIndexStereotypeCommand(string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB,
            List<CreateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDto> uniqueConstraintIndexCompositeEntityForStereotypes)
        {
            SingleUniqueField = singleUniqueField;
            CompUniqueFieldA = compUniqueFieldA;
            CompUniqueFieldB = compUniqueFieldB;
            UniqueConstraintIndexCompositeEntityForStereotypes = uniqueConstraintIndexCompositeEntityForStereotypes;
        }

        public string SingleUniqueField { get; set; }
        public string CompUniqueFieldA { get; set; }
        public string CompUniqueFieldB { get; set; }
        public List<CreateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDto> UniqueConstraintIndexCompositeEntityForStereotypes { get; set; }
    }
}