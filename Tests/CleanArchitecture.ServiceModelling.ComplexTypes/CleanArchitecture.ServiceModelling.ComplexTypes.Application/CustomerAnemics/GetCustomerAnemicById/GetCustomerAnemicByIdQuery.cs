using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics.GetCustomerAnemicById
{
    public class GetCustomerAnemicByIdQuery : IRequest<CustomerAnemicDto>, IQuery
    {
        public GetCustomerAnemicByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}