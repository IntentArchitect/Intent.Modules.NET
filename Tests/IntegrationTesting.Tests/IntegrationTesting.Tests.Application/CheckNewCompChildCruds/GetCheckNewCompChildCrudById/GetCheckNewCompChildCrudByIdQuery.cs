using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCheckNewCompChildCrudById
{
    public class GetCheckNewCompChildCrudByIdQuery : IRequest<CheckNewCompChildCrudDto>, IQuery
    {
        public GetCheckNewCompChildCrudByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}