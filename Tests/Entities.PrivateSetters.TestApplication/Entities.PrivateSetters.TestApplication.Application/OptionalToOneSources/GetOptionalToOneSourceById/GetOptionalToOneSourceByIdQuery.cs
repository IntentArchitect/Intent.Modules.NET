using System;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneSources.GetOptionalToOneSourceById
{
    public class GetOptionalToOneSourceByIdQuery : IRequest<OptionalToOneSourceDto>, IQuery
    {
        public GetOptionalToOneSourceByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}