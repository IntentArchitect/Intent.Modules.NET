using System;
using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Departments.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<Guid>, ICommand
    {
        public CreateDepartmentCommand(Guid? universityId, string name)
        {
            UniversityId = universityId;
            Name = name;
        }

        public Guid? UniversityId { get; set; }
        public string Name { get; set; }
    }
}