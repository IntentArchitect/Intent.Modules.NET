using System.Collections.Generic;
using DtoSettings.Class.Protected.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.Customers.GetCustomers
{
    public class GetCustomersQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public GetCustomersQuery()
        {
        }
    }
}