using System;
using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Departments.UpdateDepartment
{
    public class UpdateDepartmentCommand : IRequest, ICommand
    {
        public UpdateDepartmentCommand(Guid? universityId, string name, Guid id)
        {
            UniversityId = universityId;
            Name = name;
            Id = id;
        }

        public Guid? UniversityId { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}