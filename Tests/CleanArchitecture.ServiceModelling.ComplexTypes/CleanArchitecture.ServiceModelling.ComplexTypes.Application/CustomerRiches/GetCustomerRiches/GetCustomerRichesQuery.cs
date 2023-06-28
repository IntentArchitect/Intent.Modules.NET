using System.Collections.Generic;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.GetCustomerRiches
{
    public class GetCustomerRichesQuery : IRequest<List<CustomerRichDto>>, IQuery
    {
        public GetCustomerRichesQuery()
        {
        }
    }
}