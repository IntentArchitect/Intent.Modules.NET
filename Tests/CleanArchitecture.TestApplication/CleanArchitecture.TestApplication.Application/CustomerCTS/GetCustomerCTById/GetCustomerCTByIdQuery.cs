using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.CustomerCTS.GetCustomerCTById
{
    public class GetCustomerCTByIdQuery : IRequest<CustomerCTDto>, IQuery
    {
        public GetCustomerCTByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}