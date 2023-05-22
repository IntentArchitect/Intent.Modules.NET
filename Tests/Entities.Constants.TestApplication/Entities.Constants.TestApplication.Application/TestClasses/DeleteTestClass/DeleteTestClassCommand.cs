using System;
using System.Collections.Generic;
using Entities.Constants.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.DeleteTestClass
{
    public class DeleteTestClassCommand : IRequest, ICommand
    {
        public DeleteTestClassCommand(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }

    }
}