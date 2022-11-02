using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.UpdateA_AggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateA_AggregateRootCommandHandler : IRequestHandler<UpdateA_AggregateRootCommand>
    {
        private IA_AggregateRootRepository _a_AggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateA_AggregateRootCommandHandler(IA_AggregateRootRepository a_AggregateRootRepository)
        {
            _a_AggregateRootRepository = a_AggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateA_AggregateRootCommand request, CancellationToken cancellationToken)
        {
            var existingA_AggregateRoot = await _a_AggregateRootRepository.FindByIdAsync(request.Id, cancellationToken);
            existingA_AggregateRoot.AggregateAttr = request.AggregateAttr;
            existingA_AggregateRoot.Composite = request.Composite != null
                ? new A_Composite_Single
                {
                    Id = request.Composite.Id,
                    CompositeAttr = request.Composite.CompositeAttr,
                    Composite = request.Composite.Composite != null
                        ? new AA1_Composite_Single
                        {
                            Id = request.Composite.Composite.Id,
                            CompositeAttr = request.Composite.Composite.CompositeAttr,
                        }
                        : null,
                    Composites = request.Composite.Composites?.Select(composites =>
                        new AA1_Composite_Many
                        {
                            Id = composites.Id,
                            CompositeAttr = composites.CompositeAttr,
                        }).ToList(),
                    Aggregation = request.Composite.Aggregation != null
                        ? new AA1_Aggregation_Single
                        {
                            Id = request.Composite.Aggregation.Id,
                            AggregationAttr = request.Composite.Aggregation.AggregationAttr,
                        }
                        : null,
                }
                : null;
            existingA_AggregateRoot.Composites = request.Composites?.Select(composites =>
                new A_Composite_Many
                {
                    Id = composites.Id,
                    CompositeAttr = composites.CompositeAttr,
                    Composite = composites.Composite != null
                        ? new AA2_Composite_Single
                        {
                            Id = composites.Composite.Id,
                            CompositeAttr = composites.Composite.CompositeAttr,
                        }
                        : null,
                    Composites = composites.Composites?.Select(composites =>
                        new AA2_Composite_Many
                        {
                            Id = composites.Id,
                            CompositeAttr = composites.CompositeAttr,
                        }).ToList(),
                    Aggregation = composites.Aggregation != null
                        ? new AA2_Aggregation_Single
                        {
                            Id = composites.Aggregation.Id,
                            AggregationAttr = composites.Aggregation.AggregationAttr,
                        }
                        : null,
                }).ToList();
            existingA_AggregateRoot.Aggregation = request.Aggregation != null
                ? new A_Aggregation_Single
                {
                    Id = request.Aggregation.Id,
                    AggregationAttr = request.Aggregation.AggregationAttr,
                    Composite = request.Aggregation.Composite != null
                        ? new AA3_Composite_Single
                        {
                            Id = request.Aggregation.Composite.Id,
                            CompositeAttr = request.Aggregation.Composite.CompositeAttr,
                        }
                        : null,
                    Composites = request.Aggregation.Composites?.Select(composites =>
                        new AA3_Composite_Many
                        {
                            Id = composites.Id,
                            CompositeAttr = composites.CompositeAttr,
                        }).ToList(),
                    Aggregation = request.Aggregation.Aggregation != null
                        ? new AA3_Aggregation_Single
                        {
                            Id = request.Aggregation.Aggregation.Id,
                            AggregationAttr = request.Aggregation.Aggregation.AggregationAttr,
                        }
                        : null,
                    Aggregations = request.Aggregation.Aggregations?.Select(aggregations =>
                        new AA3_Aggregation_Many
                        {
                            Id = aggregations.Id,
                            AggregationAttr = aggregations.AggregationAttr,
                        }).ToList(),
                }
                : null;
            existingA_AggregateRoot.Aggregations = request.Aggregations?.Select(aggregations =>
                new A_Aggregation_Many
                {
                    Id = aggregations.Id,
                    AggregationAttr = aggregations.AggregationAttr,
                    Composite = aggregations.Composite != null
                        ? new AA4_Composite_Single
                        {
                            Id = aggregations.Composite.Id,
                            CompositeAttr = aggregations.Composite.CompositeAttr,
                        }
                        : null,
                    Composites = aggregations.Composites?.Select(composites =>
                        new AA4_Composite_Many
                        {
                            Id = composites.Id,
                            CompositeAttr = composites.CompositeAttr,
                        }).ToList(),
                    Aggregation = aggregations.Aggregation != null
                        ? new AA4_Aggregation_Single
                        {
                            Id = aggregations.Aggregation.Id,
                            AggregationAttr = aggregations.Aggregation.AggregationAttr,
                        }
                        : null,
                    Aggregations = aggregations.Aggregations?.Select(aggregations =>
                        new AA4_Aggregation_Many
                        {
                            Id = aggregations.Id,
                            AggregationAttr = aggregations.AggregationAttr,
                        }).ToList(),
                }).ToList();

            return Unit.Value;
        }
    }
}