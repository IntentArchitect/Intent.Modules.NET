using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Customers.GetCustomersLookup
{
    public class GetCustomersLookupQuery : IRequest<List<CustomerLookupDto>>, IQuery
    {
        public GetCustomersLookupQuery()
        {
        }
    }
}