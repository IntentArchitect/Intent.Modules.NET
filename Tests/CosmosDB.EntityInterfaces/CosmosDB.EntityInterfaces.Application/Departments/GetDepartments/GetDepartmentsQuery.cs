using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Departments.GetDepartments
{
    public class GetDepartmentsQuery : IRequest<List<DepartmentDto>>, IQuery
    {
        public GetDepartmentsQuery()
        {
        }
    }
}