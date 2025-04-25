using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.AdvancedMapping.UpdateAdvAggregateWithUniqueConstraintIndexStereotype
{
    public class UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommand : IRequest, ICommand
    {
        public UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommand(Guid id,
            string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB,
            List<UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDto> uniqueConstraintIndexCompositeEntityForStereotypes)
        {
            Id = id;
            SingleUniqueField = singleUniqueField;
            CompUniqueFieldA = compUniqueFieldA;
            CompUniqueFieldB = compUniqueFieldB;
            UniqueConstraintIndexCompositeEntityForStereotypes = uniqueConstraintIndexCompositeEntityForStereotypes;
        }

        public Guid Id { get; set; }
        public string SingleUniqueField { get; set; }
        public string CompUniqueFieldA { get; set; }
        public string CompUniqueFieldB { get; set; }
        public List<UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDto> UniqueConstraintIndexCompositeEntityForStereotypes { get; set; }
    }
}