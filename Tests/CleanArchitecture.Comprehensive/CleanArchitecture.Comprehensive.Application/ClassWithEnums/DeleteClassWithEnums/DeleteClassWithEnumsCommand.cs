using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ClassWithEnums.DeleteClassWithEnums
{
    public class DeleteClassWithEnumsCommand : IRequest, ICommand
    {
        public DeleteClassWithEnumsCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}