using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.DeleteAggregateWithUniqueConstraintIndexElement
{
    public class DeleteAggregateWithUniqueConstraintIndexElementCommand : IRequest, ICommand
    {
        public DeleteAggregateWithUniqueConstraintIndexElementCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}