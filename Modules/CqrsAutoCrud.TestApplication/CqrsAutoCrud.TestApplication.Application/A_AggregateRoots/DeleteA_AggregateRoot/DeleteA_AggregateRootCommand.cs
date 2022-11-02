using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.DeleteA_AggregateRoot
{
    public class DeleteA_AggregateRootCommand : IRequest, ICommand
    {
        public Guid Id { get; set; }

    }
}