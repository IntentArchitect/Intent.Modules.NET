using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomersWithParams
{
    public class GetCustomersWithParamsQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public GetCustomersWithParamsQuery(bool isActive, string? name, string? surname)
        {
            IsActive = isActive;
            Name = name;
            Surname = surname;
        }

        public bool IsActive { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
}