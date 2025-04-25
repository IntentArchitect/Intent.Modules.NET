using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.AdvancedMapping.CreateAdvAggregateWithUniqueConstraintIndexElement
{
    public class CreateAdvAggregateWithUniqueConstraintIndexElementCommand : IRequest<Guid>, ICommand
    {
        public CreateAdvAggregateWithUniqueConstraintIndexElementCommand(string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB,
            List<CreateAdvAggregateWithUniqueConstraintIndexElementCommandUniqueConstraintIndexCompositeEntityForElementsDto> uniqueConstraintIndexCompositeEntityForElements)
        {
            SingleUniqueField = singleUniqueField;
            CompUniqueFieldA = compUniqueFieldA;
            CompUniqueFieldB = compUniqueFieldB;
            UniqueConstraintIndexCompositeEntityForElements = uniqueConstraintIndexCompositeEntityForElements;
        }

        public string SingleUniqueField { get; set; }
        public string CompUniqueFieldA { get; set; }
        public string CompUniqueFieldB { get; set; }
        public List<CreateAdvAggregateWithUniqueConstraintIndexElementCommandUniqueConstraintIndexCompositeEntityForElementsDto> UniqueConstraintIndexCompositeEntityForElements { get; set; }
    }
}