using System;
using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.Departments.DeleteDepartment
{
    public class DeleteDepartmentCommand : IRequest, ICommand
    {
        public DeleteDepartmentCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}