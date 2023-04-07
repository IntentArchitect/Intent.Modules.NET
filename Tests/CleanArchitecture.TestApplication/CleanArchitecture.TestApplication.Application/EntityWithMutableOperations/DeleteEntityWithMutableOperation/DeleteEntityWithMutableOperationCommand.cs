using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.DeleteEntityWithMutableOperation
{
    public class DeleteEntityWithMutableOperationCommand : IRequest, ICommand
    {
        public Guid Id { get; set; }

    }
}