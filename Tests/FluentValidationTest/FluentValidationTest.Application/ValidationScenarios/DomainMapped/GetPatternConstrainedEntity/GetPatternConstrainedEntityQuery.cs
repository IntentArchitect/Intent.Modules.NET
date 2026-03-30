using FluentValidationTest.Application.Common.Interfaces;
using FluentValidationTest.Application.ValidationScenarios.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.GetPatternConstrainedEntity
{
    public class GetPatternConstrainedEntityQuery : IRequest<SimplePayloadDto>, IQuery
    {
        public GetPatternConstrainedEntityQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}