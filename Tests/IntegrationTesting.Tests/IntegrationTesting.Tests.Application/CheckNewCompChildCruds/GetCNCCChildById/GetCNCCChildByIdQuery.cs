using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCNCCChildById
{
    public class GetCNCCChildByIdQuery : IRequest<CNCCChildDto>, IQuery
    {
        public GetCNCCChildByIdQuery(Guid checkNewCompChildCrudId, Guid id)
        {
            CheckNewCompChildCrudId = checkNewCompChildCrudId;
            Id = id;
        }

        public Guid CheckNewCompChildCrudId { get; set; }
        public Guid Id { get; set; }
    }
}