using System;
using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.Departments.GetDepartmentById
{
    public class GetDepartmentByIdQuery : IRequest<DepartmentDto>, IQuery
    {
        public GetDepartmentByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}