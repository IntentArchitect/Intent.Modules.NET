using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AsyncOperationsClasses.ExplicitWithReturn
{
    public class ExplicitWithReturnCommand : IRequest<object>, ICommand
    {
        public ExplicitWithReturnCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}