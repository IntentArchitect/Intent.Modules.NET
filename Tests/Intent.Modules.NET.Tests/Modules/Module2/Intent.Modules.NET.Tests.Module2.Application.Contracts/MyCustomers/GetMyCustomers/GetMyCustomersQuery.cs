using Intent.Modules.NET.Tests.Application.Core.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers.GetMyCustomers
{
    public class GetMyCustomersQuery : IRequest<List<MyCustomerDto>>, IQuery
    {
        public GetMyCustomersQuery()
        {
        }
    }
}